// ----------------------------------------------------------------------------
// <copyright file="PhotonView.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using ExitGames.Client.Photon;

#if UNITY_EDITOR
using UnityEditor;
#endif


public enum ViewSynchronization { Off, ReliableDeltaCompressed, Unreliable, UnreliableOnChange }
public enum OnSerializeTransform { OnlyPosition, OnlyRotation, OnlyScale, PositionAndRotation, All }
public enum OnSerializeRigidBody { OnlyVelocity, OnlyAngularVelocity, All }

/// <summary>
/// Options to define how Ownership Transfer is handled per PhotonView.
/// </summary>
/// <remarks>
/// This setting affects how RequestOwnership and TransferOwnership work at runtime.
/// </remarks>
public enum OwnershipOption 
{ 
    /// <summary>
    /// Ownership is fixed. Instantiated objects stick with their creator, scene objects always belong to the Master Client.
    /// </summary>
    Fixed, 
    /// <summary>
    /// Ownership can be taken away from the current owner who can't object. 
    /// </summary>
    Takeover, 
    /// <summary>
    /// Ownership can be requested with PhotonView.RequestOwnership but the current owner has to agree to give up ownership.
    /// </summary>
    /// <remarks>The current owner has to implement IPunCallbacks.OnOwnershipRequest to react to the ownership request.</remarks>
    Request 
}


/// <summary>
/// PUN's NetworkView replacement class for networking. Use it like a NetworkView.
/// </summary>
/// \ingroup publicApi
[AddComponentMenu("Photon Networking/Photon View &v")]
public class PhotonView : Photon.MonoBehaviour
{
    #if UNITY_EDITOR
    [ContextMenu("Open PUN Wizard")]
    void OpenPunWizard()
    {
        EditorApplication.ExecuteMenuItem("Window/Photon Unity Networking");
    }
    #endif

    public int ownerId;

    public int group = 0;

    protected internal bool mixedModeIsReliable = false;

    // NOTE: this is now an integer because unity won't serialize short (needed for instantiation). we SEND only a short though!
    // NOTE: prefabs have a prefixBackup of -1. this is replaced with any currentLevelPrefix that's used at runtime. instantiated GOs get their prefix set pre-instantiation (so those are not -1 anymore)
    public int prefix
    {
        get
        {
            if (this.prefixBackup == -1 && PhotonNetwork.networkingPeer != null)
            {
                this.prefixBackup = PhotonNetwork.networkingPeer.currentLevelPrefix;
            }

            return this.prefixBackup;
        }
        set { this.prefixBackup = value; }
    }

    // this field is serialized by unity. that means it is copied when instantiating a persistent obj into the scene
    public int prefixBackup = -1;

    /// <summary>
    /// This is the instantiationData that was passed when calling PhotonNetwork.Instantiate* (if that was used to spawn this prefab)
    /// </summary>
    public object[] instantiationData
    {
        get
        {
            if (!this.didAwake)
            {
                // even though viewID and instantiationID are setup before the GO goes live, this data can't be set. as workaround: fetch it if needed
                this.instantiationDataField = PhotonNetwork.networkingPeer.FetchInstantiationData(this.instantiationId);
            }
            return this.instantiationDataField;
        }
        set { this.instantiationDataField = value; }
    }

    private object[] instantiationDataField;

    /// <summary>
    /// For internal use only, don't use
    /// </summary>
    protected internal object[] lastOnSerializeDataSent = null;

    /// <summary>
    /// For internal use only, don't use
    /// </summary>
    protected internal object[] lastOnSerializeDataReceived = null;

    public Component observed;

    public ViewSynchronization synchronization;

    public OnSerializeTransform onSerializeTransformOption = OnSerializeTransform.PositionAndRotation;

    public OnSerializeRigidBody onSerializeRigidBodyOption = OnSerializeRigidBody.All;

    /// <summary>Defines if ownership of this PhotonView is fixed, can be requested or simply taken.</summary>
    /// <remarks>
    /// Note that you can't edit this value at runtime. 
    /// The options are described in enum OwnershipOption.
    /// The current owner has to implement IPunCallbacks.OnOwnershipRequest to react to the ownership request.
    /// </remarks>
    public OwnershipOption ownershipTransfer = OwnershipOption.Fixed;

    public List<Component> ObservedComponents;
    Dictionary<Component, MethodInfo> m_OnSerializeMethodInfos = new Dictionary<Component, MethodInfo>();

    //These fields are only used in the CustomEditor for this script and would trigger a 
    //"this variable is never used" warning, which I am suppressing here
#pragma warning disable 0414
    [SerializeField]
    bool ObservedComponentsFoldoutOpen = true;
#pragma warning restore 0414

    [SerializeField]
    private int viewIdField = 0;

    /// <summary>
    /// The ID of the PhotonView. Identifies it in a networked game (per room).
    /// </summary>
    /// <remarks>See: [Network Instantiation](@ref instantiateManual)</remarks>
    public int viewID
    {
        get { return this.viewIdField; }
        set
        {
            // if ID was 0 for an awakened PhotonView, the view should add itself into the networkingPeer.photonViewList after setup
            bool viewMustRegister = this.didAwake && this.viewIdField == 0;

            // TODO: decide if a viewID can be changed once it wasn't 0. most likely that is not a good idea
            // check if this view is in networkingPeer.photonViewList and UPDATE said list (so we don't keep the old viewID with a reference to this object)
            // PhotonNetwork.networkingPeer.RemovePhotonView(this, true);

            this.ownerId = value / PhotonNetwork.MAX_VIEW_IDS;

            this.viewIdField = value;

            if (viewMustRegister)
            {
                PhotonNetwork.networkingPeer.RegisterPhotonView(this);
            }
            //Debug.Log("Set viewID: " + value + " ->  owner: " + this.ownerId + " subId: " + this.subId);
        }
    }

    public int instantiationId; // if the view was instantiated with a GO, this GO has a instantiationID (first view's viewID)

    /// <summary>True if the PhotonView was loaded with the scene (game object) or instantiated with InstantiateSceneObject.</summary>
    /// <remarks>
    /// Scene objects are not owned by a particular player but belong to the scene. Thus they don't get destroyed when their
    /// creator leaves the game and the current Master Client can control them (whoever that is).
    /// The ownerId is 0 (player IDs are 1 and up).
    /// </remarks>
    public bool isSceneView
    {
        get { return this.CreatorActorNr == 0; }
    }

    /// <summary>
    /// The owner of a PhotonView is the player who created the GameObject with that view. Objects in the scene don't have an owner.
    /// </summary>
    /// <remarks>
    /// The owner/controller of a PhotonView is also the client which sends position updates of the GameObject.
    /// 
    /// Ownership can be transferred to another player with PhotonView.TransferOwnership or any player can request 
    /// ownership by calling the PhotonView's RequestOwnership method.
    /// The current owner has to implement IPunCallbacks.OnOwnershipRequest to react to the ownership request.
    /// </remarks>
    public PhotonPlayer owner
    {
        get { return PhotonPlayer.Find(this.ownerId); }
    }

    public int OwnerActorNr
    {
        get { return this.ownerId; }
    }

    public bool isOwnerActive
    {
        get { return this.ownerId != 0 && PhotonNetwork.networkingPeer.mActors.ContainsKey(this.ownerId); }
    }

    public int CreatorActorNr
    {
        get { return this.viewIdField / PhotonNetwork.MAX_VIEW_IDS; }
    }

    /// <summary>
    /// True if the PhotonView is "mine" and can be controlled by this client.
    /// </summary>
    /// <remarks>
    /// PUN has an ownership concept that defines who can control and destroy each PhotonView.
    /// True in case the owner matches the local PhotonPlayer.
    /// True if this is a scene photonview on the Master client.
    /// </remarks>
    public bool isMine
    {
        get
        {
            return (this.ownerId == PhotonNetwork.player.ID) || (!this.isOwnerActive && PhotonNetwork.isMasterClient);
        }
    }
    
    protected internal bool didAwake;

    [SerializeField]
    protected internal bool isRuntimeInstantiated;

    protected internal bool destroyedByPhotonNetworkOrQuit;

    /// <summary>Called by Unity on start of the application and does a setup the PhotonView.</summary>
    protected internal void Awake()
    {
        // registration might be too late when some script (on this GO) searches this view BUT GetPhotonView() can search ALL in that case
        PhotonNetwork.networkingPeer.RegisterPhotonView(this);

        this.instantiationDataField = PhotonNetwork.networkingPeer.FetchInstantiationData(this.instantiationId);
        this.didAwake = true;
    }

    /// <summary>
    /// Depending on the PhotonView's ownershipTransfer setting, any client can request to become owner of the PhotonView.
    /// </summary>
    /// <remarks>
    /// Requesting ownership can give you control over a PhotonView, if the ownershipTransfer setting allows that.
    /// The current owner might have to implement IPunCallbacks.OnOwnershipRequest to react to the ownership request.
    /// 
    /// The owner/controller of a PhotonView is also the client which sends position updates of the GameObject.
    /// </remarks>
    public void RequestOwnership()
    {
        PhotonNetwork.networkingPeer.RequestOwnership(this.viewID, this.ownerId);
    }

    /// <summary>
    /// Transfers the ownership of this PhotonView (and GameObject) to another player.
    /// </summary>
    /// <remarks>
    /// The owner/controller of a PhotonView is also the client which sends position updates of the GameObject.
    /// </remarks>
    public void TransferOwnership(PhotonPlayer newOwner)
    {
        this.TransferOwnership(newOwner.ID);
    }

    /// <summary>
    /// Transfers the ownership of this PhotonView (and GameObject) to another player.
    /// </summary>
    /// <remarks>
    /// The owner/controller of a PhotonView is also the client which sends position updates of the GameObject.
    /// </remarks>
    public void TransferOwnership(int newOwnerId)
    {
        PhotonNetwork.networkingPeer.TransferOwnership(this.viewID, newOwnerId);
        this.ownerId = newOwnerId;  // immediately switch ownership locally, to avoid more updates sent from this client.
    }


    protected internal void OnApplicationQuit()
    {
        destroyedByPhotonNetworkOrQuit = true;	// on stop-playing its ok Destroy is being called directly (not by PN.Destroy())
    }

    protected internal void OnDestroy()
    {
        if (!this.destroyedByPhotonNetworkOrQuit)
        {
            PhotonNetwork.networkingPeer.LocalCleanPhotonView(this);
        }

        if (!this.destroyedByPhotonNetworkOrQuit && !Application.isLoadingLevel)
        {
            if (this.instantiationId > 0)
            {
                // if this viewID was not manually assigned (and we're not shutting down or loading a level), you should use PhotonNetwork.Destroy() to get rid of GOs with PhotonViews
                Debug.LogError("OnDestroy() seems to be called without PhotonNetwork.Destroy()?! GameObject: " + this.gameObject + " Application.isLoadingLevel: " + Application.isLoadingLevel);
            }
            else
            {
                // this seems to be a manually instantiated PV. if it's local, we could warn if the ID is not in the allocated-list
                if (this.viewID <= 0)
                {
                    Debug.LogWarning(string.Format("OnDestroy manually allocated PhotonView {0}. The viewID is 0. Was it ever (manually) set?", this));
                }
                else if (this.isMine && !PhotonNetwork.manuallyAllocatedViewIds.Contains(this.viewID))
                {
                    Debug.LogWarning(string.Format("OnDestroy manually allocated PhotonView {0}. The viewID is local (isMine) but not in manuallyAllocatedViewIds list. Use UnAllocateViewID() after you destroyed the PV.", this));
                }
            }
        }
    }

    private MethodInfo OnSerializeMethodInfo;

    private bool failedToFindOnSerialize;

    public void SerializeView( PhotonStream stream, PhotonMessageInfo info )
    {
        SerializeComponent( observed, stream, info );

        for( int i = 0; i < ObservedComponents.Count; ++i )
        {
            SerializeComponent( ObservedComponents[ i ], stream, info );
        }
    }

    public void DeserializeView( PhotonStream stream, PhotonMessageInfo info )
    {
        DeserializeComponent( observed, stream, info );

        for( int i = 0; i < ObservedComponents.Count; ++i )
        {
            DeserializeComponent( ObservedComponents[ i ], stream, info );
        }
    }

    internal protected void DeserializeComponent( Component component, PhotonStream stream, PhotonMessageInfo info )
    {
        if( component == null )
        {
            return;
        }

        // Use incoming data according to observed type
        if( component is MonoBehaviour )
        {
            ExecuteComponentOnSerialize( component, stream, info );
        }
        else if( component is Transform )
        {
            Transform trans = (Transform)component;

            switch( onSerializeTransformOption )
            {
            case OnSerializeTransform.All:
                trans.localPosition = (Vector3)stream.ReceiveNext();
                trans.localRotation = (Quaternion)stream.ReceiveNext();
                trans.localScale = (Vector3)stream.ReceiveNext();
                break;
            case OnSerializeTransform.OnlyPosition:
                trans.localPosition = (Vector3)stream.ReceiveNext();
                break;
            case OnSerializeTransform.OnlyRotation:
                trans.localRotation = (Quaternion)stream.ReceiveNext();
                break;
            case OnSerializeTransform.OnlyScale:
                trans.localScale = (Vector3)stream.ReceiveNext();
                break;
            case OnSerializeTransform.PositionAndRotation:
                trans.localPosition = (Vector3)stream.ReceiveNext();
                trans.localRotation = (Quaternion)stream.ReceiveNext();
                break;
            }
        }
        else if( component is Rigidbody )
        {
            Rigidbody rigidB = (Rigidbody)component;

            switch( onSerializeRigidBodyOption )
            {
            case OnSerializeRigidBody.All:
                rigidB.velocity = (Vector3)stream.ReceiveNext();
                rigidB.angularVelocity = (Vector3)stream.ReceiveNext();
                break;
            case OnSerializeRigidBody.OnlyAngularVelocity:
                rigidB.angularVelocity = (Vector3)stream.ReceiveNext();
                break;
            case OnSerializeRigidBody.OnlyVelocity:
                rigidB.velocity = (Vector3)stream.ReceiveNext();
                break;
            }
        }
        else if( component is Rigidbody2D )
        {
            Rigidbody2D rigidB = (Rigidbody2D)component;

            switch( onSerializeRigidBodyOption )
            {
            case OnSerializeRigidBody.All:
                rigidB.velocity = (Vector2)stream.ReceiveNext();
                rigidB.angularVelocity = (float)stream.ReceiveNext();
                break;
            case OnSerializeRigidBody.OnlyAngularVelocity:
                rigidB.angularVelocity = (float)stream.ReceiveNext();
                break;
            case OnSerializeRigidBody.OnlyVelocity:
                rigidB.velocity = (Vector2)stream.ReceiveNext();
                break;
            }
        }
        else
        {
            Debug.LogError( "Type of observed is unknown when receiving." );
        }
    }

    internal protected void SerializeComponent( Component component, PhotonStream stream, PhotonMessageInfo info )
    {
        if( component == null )
        {
            return;
        }

        if( component is MonoBehaviour )
        {
            ExecuteComponentOnSerialize( component, stream, info );
        }
        else if( component is Transform )
        {
            Transform trans = (Transform)component;

            switch( onSerializeTransformOption )
            {
            case OnSerializeTransform.All:
                stream.SendNext( trans.localPosition );
                stream.SendNext( trans.localRotation );
                stream.SendNext( trans.localScale );
                break;
            case OnSerializeTransform.OnlyPosition:
                stream.SendNext( trans.localPosition );
                break;
            case OnSerializeTransform.OnlyRotation:
                stream.SendNext( trans.localRotation );
                break;
            case OnSerializeTransform.OnlyScale:
                stream.SendNext( trans.localScale );
                break;
            case OnSerializeTransform.PositionAndRotation:
                stream.SendNext( trans.localPosition );
                stream.SendNext( trans.localRotation );
                break;
            }
        }
        else if( component is Rigidbody )
        {
            Rigidbody rigidB = (Rigidbody)component;

            switch( onSerializeRigidBodyOption )
            {
            case OnSerializeRigidBody.All:
                stream.SendNext( rigidB.velocity );
                stream.SendNext( rigidB.angularVelocity );
                break;
            case OnSerializeRigidBody.OnlyAngularVelocity:
                stream.SendNext( rigidB.angularVelocity );
                break;
            case OnSerializeRigidBody.OnlyVelocity:
                stream.SendNext( rigidB.velocity );
                break;
            }
        }
        else if( component is Rigidbody2D )
        {
            Rigidbody2D rigidB = (Rigidbody2D)component;

            switch( onSerializeRigidBodyOption )
            {
            case OnSerializeRigidBody.All:
                stream.SendNext( rigidB.velocity );
                stream.SendNext( rigidB.angularVelocity );
                break;
            case OnSerializeRigidBody.OnlyAngularVelocity:
                stream.SendNext( rigidB.angularVelocity );
                break;
            case OnSerializeRigidBody.OnlyVelocity:
                stream.SendNext( rigidB.velocity );
                break;
            }
        }
        else
        {
            Debug.LogError( "Observed type is not serializable: " + component.GetType() );
        }
    }

    internal protected void ExecuteComponentOnSerialize( Component component, PhotonStream stream, PhotonMessageInfo info )
    {
        if( component != null )
        {
            if( m_OnSerializeMethodInfos.ContainsKey( component ) == false )
            {
                MethodInfo newMethod = null;
                bool foundMethod = NetworkingPeer.GetMethod( component as MonoBehaviour, PhotonNetworkingMessage.OnPhotonSerializeView.ToString(), out newMethod );

                if( foundMethod == false )
                {
                    Debug.LogError( "The observed monobehaviour (" + component.name + ") of this PhotonView does not implement OnPhotonSerializeView()!" );
                    newMethod = null;
                }

                m_OnSerializeMethodInfos.Add( component, newMethod );
            }

            if( m_OnSerializeMethodInfos[ component ] != null )
            {
                m_OnSerializeMethodInfos[ component ].Invoke( component, new object[] { stream, info } );
            }
        }
    }

    /// <summary>
    /// Call a RPC method of this GameObject on remote clients of this room (or on all, inclunding this client).
    /// </summary>
    /// <remarks>
    /// [Remote Procedure Calls](@ref rpcManual) are an essential tool in making multiplayer games with PUN.
    /// It enables you to make every client in a room call a specific method.
    ///
    /// RPC calls can target "All" or the "Others".
    /// Usually, the target "All" gets executed locally immediately after sending the RPC.
    /// The "*ViaServer" options send the RPC to the server and execute it on this client when it's sent back.
    /// Of course, calls are affected by this client's lag and that of remote clients.
    ///
    /// Each call automatically is routed to the same PhotonView (and GameObject) that was used on the
    /// originating client.
    ///
    /// See: [Remote Procedure Calls](@ref rpcManual).
    /// </remarks>
    /// <param name="methodName">The name of a fitting method that was has the RPC attribute.</param>
    /// <param name="target">The group of targets and the way the RPC gets sent.</param>
    /// <param name="parameters">The parameters that the RPC method has (must fit this call!).</param>
    public void RPC(string methodName, PhotonTargets target, params object[] parameters)
    {
        RpcSecure(methodName, target, false, parameters);
    }

    /// <summary>
    /// Call a RPC method of this GameObject on remote clients of this room (or on all, inclunding this client).
    /// </summary>
    /// <remarks>
    /// [Remote Procedure Calls](@ref rpcManual) are an essential tool in making multiplayer games with PUN.
    /// It enables you to make every client in a room call a specific method.
    ///
    /// RPC calls can target "All" or the "Others".
    /// Usually, the target "All" gets executed locally immediately after sending the RPC.
    /// The "*ViaServer" options send the RPC to the server and execute it on this client when it's sent back.
    /// Of course, calls are affected by this client's lag and that of remote clients.
    ///
    /// Each call automatically is routed to the same PhotonView (and GameObject) that was used on the
    /// originating client.
    ///
    /// See: [Remote Procedure Calls](@ref rpcManual).
    /// </remarks>
    ///<param name="methodName">The name of a fitting method that was has the RPC attribute.</param>
    ///<param name="target">The group of targets and the way the RPC gets sent.</param>
    ///<param name="encrypt"> </param>
    ///<param name="parameters">The parameters that the RPC method has (must fit this call!).</param>
    public void RpcSecure(string methodName, PhotonTargets target, bool encrypt, params object[] parameters)
    {
        if(PhotonNetwork.networkingPeer.hasSwitchedMC && target == PhotonTargets.MasterClient)
        {
            PhotonNetwork.RPC(this, methodName, PhotonNetwork.masterClient, encrypt, parameters);
        }
        else
        {
            PhotonNetwork.RPC(this, methodName, target, encrypt, parameters);
        }
    }

    /// <summary>
    /// Call a RPC method of this GameObject on remote clients of this room (or on all, inclunding this client).
    /// </summary>
    /// <remarks>
    /// [Remote Procedure Calls](@ref rpcManual) are an essential tool in making multiplayer games with PUN.
    /// It enables you to make every client in a room call a specific method.
    ///
    /// This method allows you to make an RPC calls on a specific player's client.
    /// Of course, calls are affected by this client's lag and that of remote clients.
    ///
    /// Each call automatically is routed to the same PhotonView (and GameObject) that was used on the
    /// originating client.
    ///
    /// See: [Remote Procedure Calls](@ref rpcManual).
    /// </remarks>
    /// <param name="methodName">The name of a fitting method that was has the RPC attribute.</param>
    /// <param name="targetPlayer">The group of targets and the way the RPC gets sent.</param>
    /// <param name="parameters">The parameters that the RPC method has (must fit this call!).</param>
    public void RPC(string methodName, PhotonPlayer targetPlayer, params object[] parameters)
    {
        PhotonNetwork.RPC(this, methodName, targetPlayer, false, parameters);
    }

    /// <summary>
    /// Call a RPC method of this GameObject on remote clients of this room (or on all, inclunding this client).
    /// </summary>
    /// <remarks>
    /// [Remote Procedure Calls](@ref rpcManual) are an essential tool in making multiplayer games with PUN.
    /// It enables you to make every client in a room call a specific method.
    ///
    /// This method allows you to make an RPC calls on a specific player's client.
    /// Of course, calls are affected by this client's lag and that of remote clients.
    ///
    /// Each call automatically is routed to the same PhotonView (and GameObject) that was used on the
    /// originating client.
    ///
    /// See: [Remote Procedure Calls](@ref rpcManual).
    /// </remarks>
    ///<param name="methodName">The name of a fitting method that was has the RPC attribute.</param>
    ///<param name="targetPlayer">The group of targets and the way the RPC gets sent.</param>
    ///<param name="encrypt"> </param>
    ///<param name="parameters">The parameters that the RPC method has (must fit this call!).</param>
    public void RpcSecure(string methodName, PhotonPlayer targetPlayer, bool encrypt, params object[] parameters)
    {
        PhotonNetwork.RPC(this, methodName, targetPlayer, encrypt, parameters);
    }

    public static PhotonView Get(Component component)
    {
        return component.GetComponent<PhotonView>();
    }

    public static PhotonView Get(GameObject gameObj)
    {
        return gameObj.GetComponent<PhotonView>();
    }

    public static PhotonView Find(int viewID)
    {
        return PhotonNetwork.networkingPeer.GetPhotonView(viewID);
    }

    public override string ToString()
    {
        return string.Format("View ({3}){0} on {1} {2}", this.viewID, (this.gameObject != null) ? this.gameObject.name : "GO==null", (this.isSceneView) ? "(scene)" : string.Empty, this.prefix);
    }
}

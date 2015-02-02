﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkingPeer.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking (PUN)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Lite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// Implements Photon LoadBalancing used in PUN.
/// This class is used internally by PhotonNetwork and not intended as public API.
/// </summary>
internal class NetworkingPeer : LoadbalancingPeer, IPhotonPeerListener
{
    /// <summary>The GameVersion as set in the Connect-methods.</summary>
    protected internal string mAppVersion;

    /// <summary>Combination of GameVersion+"_"+PunVersion. Separates players per app by version.</summary>
    protected internal string mAppVersionPun
    {
        get { return string.Format("{0}_{1}", mAppVersion, PhotonNetwork.versionPUN); }
    }

    /// <summary>Contains the AppId for the Photon Cloud (ignored by Photon Servers).</summary>
    protected internal string mAppId;

    /// <summary>
    /// A user's authentication values used during connect for Custom Authentication with Photon (and a custom service/community).
    /// Set these before calling Connect if you want custom authentication.
    /// </summary>
    public AuthenticationValues CustomAuthenticationValues { get; set; }

    public string MasterServerAddress { get; protected internal set; }

    private string playername = "";

    private IPhotonPeerListener externalListener;

    private JoinType mLastJoinType;

    private bool mPlayernameHasToBeUpdated;

    public string PlayerName
    {
        get
        {
            return this.playername;
        }

        set
        {
            if (string.IsNullOrEmpty(value) || value.Equals(this.playername))
            {
                return;
            }

            if (this.mLocalActor != null)
            {
                this.mLocalActor.name = value;
            }

            this.playername = value;
            if (this.mCurrentGame != null)
            {
                // Only when in a room
                this.SendPlayerName();
            }
        }
    }

    public PeerState State { get; internal set; }

    // "public" access to the current game - is null unless a room is joined on a gameserver
    public Room mCurrentGame
    {
        get
        {
            if (this.mRoomToGetInto != null && this.mRoomToGetInto.isLocalClientInside)
            {
                return this.mRoomToGetInto;
            }

            return null;
        }
    }

    /// <summary>
    /// keeps the custom properties, gameServer address and anything else about the room we want to get into
    /// </summary>
    internal Room mRoomToGetInto { get; set; }
    internal RoomOptions mRoomOptionsForCreate { get; set; }
    internal TypedLobby mRoomToEnterLobby { get; set; }

    public Dictionary<int, PhotonPlayer> mActors = new Dictionary<int, PhotonPlayer>();

    public PhotonPlayer[] mOtherPlayerListCopy = new PhotonPlayer[0];
    public PhotonPlayer[] mPlayerListCopy = new PhotonPlayer[0];

    public PhotonPlayer mLocalActor { get; internal set; }

    public PhotonPlayer mMasterClient = null;

    public bool hasSwitchedMC = false;

    public string mGameserver { get; internal set; }

    public bool requestSecurity = true;

    private Dictionary<Type, List<MethodInfo>> monoRPCMethodsCache = new Dictionary<Type, List<MethodInfo>>();

    public static bool UsePrefabCache = true;

    public static Dictionary<string, GameObject> PrefabCache = new Dictionary<string, GameObject>();

    public Dictionary<string, RoomInfo> mGameList = new Dictionary<string, RoomInfo>();
    public RoomInfo[] mGameListCopy = new RoomInfo[0];

    public TypedLobby lobby { get; set; }

    public bool insideLobby = false;

    /// <summary>Stat value: Count of players on Master (looking for rooms)</summary>
    public int mPlayersOnMasterCount { get; internal set; }

    /// <summary>Stat value: Count of Rooms</summary>
    public int mGameCount { get; internal set; }

    /// <summary>Stat value: Count of Players in rooms</summary>
    public int mPlayersInRoomsCount { get; internal set; }
    
    private HashSet<int> allowedReceivingGroups = new HashSet<int>();

    private HashSet<int> blockSendingGroups = new HashSet<int>();

    internal protected Dictionary<int, PhotonView> photonViewList = new Dictionary<int, PhotonView>(); //TODO: make private again

    private readonly Dictionary<int, Hashtable> dataPerGroupReliable = new Dictionary<int, Hashtable>();    // only used in RunViewUpdate()
    private readonly Dictionary<int, Hashtable> dataPerGroupUnreliable = new Dictionary<int, Hashtable>();  // only used in RunViewUpdate()

    internal protected short currentLevelPrefix = 0;

    private readonly Dictionary<string, int> rpcShortcuts;  // lookup "table" for the index (shortcut) of an RPC name

    /// <summary>The server this client is currently connected or connecting to.</summary>
    internal protected ServerConnection server { get; private set; }

    public bool IsInitialConnect = false;

    /// <summary>True if this client uses a NameServer to get the Master Server address.</summary>
    public bool IsUsingNameServer { get; protected internal set; }

    /// <summary>Name Server Address that this client uses. You can use the default values and usually won't have to set this value.</summary>
    public string NameServerAddress = "ns.exitgamescloud.com";
    public string NameServerAddressHttp = "http://ns.exitgamescloud.com:80/photon/n";

    private static readonly Dictionary<ConnectionProtocol, int> ProtocolToNameServerPort = new Dictionary<ConnectionProtocol, int>() { {ConnectionProtocol.Udp, 5058}, {ConnectionProtocol.Tcp, 4533} }; //, { ConnectionProtocol.RHttp, 6063 } };

    /// <summary>A list of region names for the Photon Cloud. Set by the result of OpGetRegions().</summary>
    /// <remarks>Put a "case OperationCode.GetRegions:" into your OnOperationResponse method to notice when the result is available.</remarks>
    public List<Region> AvailableRegions { get; protected internal set; }

    /// <summary>The cloud region this client connects to. Set by ConnectToRegionMaster().</summary>
    public CloudRegionCode CloudRegion { get; protected internal set; }

    /// <summary>Internally used to check if a "Secret" is available to use. Sent by Photon Cloud servers, it simplifies authentication when switching servers.</summary>
    public bool IsAuthorizeSecretAvailable
    {
        get
        {
            // TODO: comment in the code again, when the new auth-workflow is available in the Cloud
            return false; // this.CustomAuthenticationValues != null && !String.IsNullOrEmpty(this.CustomAuthenticationValues.Secret);
        }
    }

    /// <summary>Internally used to trigger OpAuthenticate when encryption was established after a connect.</summary>
    private bool didAuthenticate;

    public NetworkingPeer(IPhotonPeerListener listener, string playername, ConnectionProtocol connectionProtocol) : base(listener, connectionProtocol)
    {
        #if !UNITY_EDITOR && (UNITY_WINRT)
        // this automatically uses a separate assembly-file with Win8-style Socket usage (not possible in Editor)
        Debug.LogWarning("Using PingWindowsStore");
        PhotonHandler.PingImplementation = typeof(PingWindowsStore);    // but for ping, we have to set the implementation explicitly to Win 8 Store/Phone
        #endif

        #pragma warning disable 0162    // the library variant defines if we should use PUN's SocketUdp variant (at all)
        if (PhotonPeer.NoSocket)
        {
            #if !UNITY_EDITOR && (UNITY_PS3 || UNITY_ANDROID)
            Debug.Log("Using class SocketUdpNativeDynamic");
            this.SocketImplementation = typeof(SocketUdpNativeDynamic);
            PhotonHandler.PingImplementation = typeof(PingNativeDynamic);
            #elif !UNITY_EDITOR && UNITY_IPHONE
            Debug.Log("Using class SocketUdpNativeStatic");
            this.SocketImplementation = typeof(SocketUdpNativeStatic);
            PhotonHandler.PingImplementation = typeof(PingNativeStatic);
            #elif !UNITY_EDITOR && (UNITY_WINRT)
            // this automatically uses a separate assembly-file with Win8-style Socket usage (not possible in Editor)
            #else
            this.SocketImplementation = typeof (SocketUdp);
            PhotonHandler.PingImplementation = typeof(PingMonoEditor);
            #endif

            if (this.SocketImplementation == null)
            {
                Debug.Log("No socket implementation set for 'NoSocket' assembly. Please contact Exit Games.");
            }
        }
        #pragma warning restore 0162

        if (PhotonHandler.PingImplementation == null)
        {
            PhotonHandler.PingImplementation = typeof(PingMono);
        }

        this.Listener = this;
        this.lobby = TypedLobby.Default;
        this.LimitOfUnreliableCommands = 40;

        // don't set the field directly! the listener is passed on to other classes, which get updated by the property set method
        this.externalListener = listener;
        this.PlayerName = playername;
        this.mLocalActor = new PhotonPlayer(true, -1, this.playername);
        this.AddNewPlayer(this.mLocalActor.ID, this.mLocalActor);

        // RPC shortcut lookup creation (from list of RPCs, which is updated by Editor scripts)
        rpcShortcuts = new Dictionary<string, int>(PhotonNetwork.PhotonServerSettings.RpcList.Count);
        for (int index = 0; index < PhotonNetwork.PhotonServerSettings.RpcList.Count; index++)
        {
            var name = PhotonNetwork.PhotonServerSettings.RpcList[index];
            rpcShortcuts[name] = index;
        }

        this.State = global::PeerState.PeerCreated;
    }

    #region Operations and Connection Methods


    public override bool Connect(string serverAddress, string applicationName)
    {
        Debug.LogError("Avoid using this directly. Thanks.");
        return false;
    }

    public bool Connect(string serverAddress, ServerConnection type)
    {
        if (PhotonHandler.AppQuits)
        {
            Debug.LogWarning("Ignoring Connect() because app gets closed. If this is an error, check PhotonHandler.AppQuits.");
            return false;
        }

        if (PhotonNetwork.connectionStateDetailed == global::PeerState.Disconnecting)
        {
            Debug.LogError("Connect() failed. Can't connect while disconnecting (still). Current state: " + PhotonNetwork.connectionStateDetailed);
            return false;
        }

        // connect might fail, if the DNS name can't be resolved or if no network connection is available
        bool connecting = base.Connect(serverAddress, "");
        if (connecting)
        {
            switch (type)
            {
                case ServerConnection.NameServer:
                    State = global::PeerState.ConnectingToNameServer;
                    break;
                case ServerConnection.MasterServer:
                    State = global::PeerState.ConnectingToMasterserver;
                    break;
                case ServerConnection.GameServer:
                    State = global::PeerState.ConnectingToGameserver;
                    break;
            }
        }

        return connecting;
    }


    /// <summary>
    /// Connects to the NameServer for Photon Cloud, where a region and server list can be obtained.
    /// </summary>
    /// <see cref="OpGetRegions"/>
    /// <returns>If the workflow was started or failed right away.</returns>
    public bool ConnectToNameServer()
    {
        if (PhotonHandler.AppQuits)
        {
            Debug.LogWarning("Ignoring Connect() because app gets closed. If this is an error, check PhotonHandler.AppQuits.");
            return false;
        }

        IsUsingNameServer = true;
        this.CloudRegion = CloudRegionCode.none;

        if (this.State == global::PeerState.ConnectedToNameServer)
        {
            return true;
        }

        #if RHTTP
        string address = (this.UsedProtocol == ConnectionProtocol.RHttp) ? this.NameServerAddressHttp : this.NameServerAddress;
        #else
        string address = this.NameServerAddress;
        #endif

        if (!address.Contains(":"))
        {
            int port = 0;
            ProtocolToNameServerPort.TryGetValue(this.UsedProtocol, out port);
            address = string.Format("{0}:{1}", address, port);
            Debug.Log("Server to connect to: " + address + " settings protocol: " + PhotonNetwork.PhotonServerSettings.Protocol);
        }
        if (!base.Connect(address, "ns"))
        {
            return false;
        }

        this.State = global::PeerState.ConnectingToNameServer;
        return true;
    }

    /// <summary>
    /// Connects you to a specific region's Master Server, using the Name Server to find the IP.
    /// </summary>
    /// <returns>If the operation could be sent. If false, no operation was sent.</returns>
    public bool ConnectToRegionMaster(CloudRegionCode region)
    {
        if (PhotonHandler.AppQuits)
        {
            Debug.LogWarning("Ignoring Connect() because app gets closed. If this is an error, check PhotonHandler.AppQuits.");
            return false;
        }

        IsUsingNameServer = true;
        this.CloudRegion = region;

        if (this.State == global::PeerState.ConnectedToNameServer)
        {
            return this.OpAuthenticate(this.mAppId, this.mAppVersionPun, this.PlayerName, this.CustomAuthenticationValues, region.ToString());
        }

        #if RHTTP
        string address = (this.UsedProtocol == ConnectionProtocol.RHttp) ? this.NameServerAddressHttp : this.NameServerAddress;
        #else
        string address = this.NameServerAddress;
        #endif

        if (!address.Contains(":"))
        {
            int port = 0;
            ProtocolToNameServerPort.TryGetValue(this.UsedProtocol, out port);
            address = string.Format("{0}:{1}", address, port);
            //Debug.Log("Server to connect to: "+ address + " settings protocol: " + PhotonNetwork.PhotonServerSettings.Protocol);
        }
        if (!base.Connect(address, "ns"))
        {
            return false;
        }

        this.State = global::PeerState.ConnectingToNameServer;
        return true;
    }

    /// <summary>
    /// While on the NameServer, this gets you the list of regional servers (short names and their IPs to ping them).
    /// </summary>
    /// <returns>If the operation could be sent. If false, no operation was sent (e.g. while not connected to the NameServer).</returns>
    public bool GetRegions()
    {
        if (this.server != ServerConnection.NameServer)
        {
            return false;
        }

        bool sent = this.OpGetRegions(this.mAppId);
        if (sent)
        {
            this.AvailableRegions = null;
        }

        return sent;
    }

    /// <summary>
    /// Complete disconnect from photon (and the open master OR game server)
    /// </summary>
    public override void Disconnect()
    {
        if (this.PeerState == PeerStateValue.Disconnected)
        {
            if (!PhotonHandler.AppQuits)
            {
                Debug.LogWarning(string.Format("Can't execute Disconnect() while not connected. Nothing changed. State: {0}", this.State));
            }
            return;
        }

        this.State = global::PeerState.Disconnecting;
        base.Disconnect();

        //this.LeftRoomCleanup();
        //this.LeftLobbyCleanup();
    }


    /// <summary>
    /// Internally used only. Triggers OnStateChange with "Disconnect" in next dispatch which is the signal to re-connect (if at all).
    /// </summary>
    private void DisconnectToReconnect()
    {
        switch (this.server)
        {
            case ServerConnection.NameServer:
                this.State = global::PeerState.DisconnectingFromNameServer;
                base.Disconnect();
                break;
            case ServerConnection.MasterServer:
                this.State = global::PeerState.DisconnectingFromMasterserver;
                base.Disconnect();
                //LeftLobbyCleanup();
                break;
            case ServerConnection.GameServer:
                this.State = global::PeerState.DisconnectingFromGameserver;
                base.Disconnect();
                //this.LeftRoomCleanup();
                break;
        }
    }

    /// <summary>
    /// Called at disconnect/leavelobby etc. This CAN also be called when we are not in a lobby (e.g. disconnect from room)
    /// </summary>
    /// <remarks>Calls callback method OnLeftLobby if this client was in a lobby initially. Clears the lobby's game lists.</remarks>
    private void LeftLobbyCleanup()
    {
        this.mGameList = new Dictionary<string, RoomInfo>();
        this.mGameListCopy = new RoomInfo[0];

        if (insideLobby)
        {
            this.insideLobby = false;
            SendMonoMessage(PhotonNetworkingMessage.OnLeftLobby);
        }
    }

    /// <summary>
    /// Called when "this client" left a room to clean up.
    /// </summary>
    private void LeftRoomCleanup()
    {
        bool wasInRoom = mRoomToGetInto != null;
        // when leaving a room, we clean up depending on that room's settings.
        bool autoCleanupSettingOfRoom = (this.mRoomToGetInto != null) ? this.mRoomToGetInto.autoCleanUp : PhotonNetwork.autoCleanUpPlayerObjects;

        this.hasSwitchedMC = false;
        this.mRoomToGetInto = null;
        this.mActors = new Dictionary<int, PhotonPlayer>();
        this.mPlayerListCopy = new PhotonPlayer[0];
        this.mOtherPlayerListCopy = new PhotonPlayer[0];
        this.mMasterClient = null;
        this.allowedReceivingGroups = new HashSet<int>();
        this.blockSendingGroups = new HashSet<int>();
        this.mGameList = new Dictionary<string, RoomInfo>();
        this.mGameListCopy = new RoomInfo[0];
        this.isFetchingFriends = false;

        this.ChangeLocalID(-1);

        // Cleanup all network objects (all spawned PhotonViews, local and remote)
        if (autoCleanupSettingOfRoom)
        {
            this.LocalCleanupAnythingInstantiated(true);
            PhotonNetwork.manuallyAllocatedViewIds = new List<int>();       // filled and easier to replace completely
        }

        if (wasInRoom)
        {
            SendMonoMessage(PhotonNetworkingMessage.OnLeftRoom);
        }
    }

    /// <summary>
    /// Cleans up anything that was instantiated in-game (not loaded with the scene).
    /// </summary>
    protected internal void LocalCleanupAnythingInstantiated(bool destroyInstantiatedGameObjects)
    {
        if (tempInstantiationData.Count > 0)
        {
            Debug.LogWarning("It seems some instantiation is not completed, as instantiation data is used. You should make sure instantiations are paused when calling this method. Cleaning now, despite this.");
        }

        // Destroy GO's (if we should)
        if (destroyInstantiatedGameObjects)
        {
            // Fill list with Instantiated objects
            HashSet<GameObject> instantiatedGos = new HashSet<GameObject>();
            foreach (PhotonView view in this.photonViewList.Values)
            {
                if (view.isRuntimeInstantiated)
                {
                    instantiatedGos.Add(view.gameObject); // HashSet keeps each object only once
                }
            }
            
            foreach (GameObject go in instantiatedGos)
            {
                this.RemoveInstantiatedGO(go, true);
            }
        }

        // photonViewList is cleared of anything instantiated (so scene items are left inside)
        // any other lists can be
        this.tempInstantiationData.Clear(); // should be empty but to be safe we clear (no new list needed)
        PhotonNetwork.lastUsedViewSubId = 0;
        PhotonNetwork.lastUsedViewSubIdStatic = 0;
    }

    // gameID can be null (optional). The server assigns a unique name if no name is set

    // joins a room and sets your current username as custom actorproperty (will broadcast that)

    #endregion

    #region Helpers

    private void ReadoutProperties(Hashtable gameProperties, Hashtable pActorProperties, int targetActorNr)
    {
        // Debug.LogWarning("ReadoutProperties gameProperties: " + gameProperties.ToStringFull() + " pActorProperties: " + pActorProperties.ToStringFull() + " targetActorNr: " + targetActorNr);
        // read game properties and cache them locally
        if (this.mCurrentGame != null && gameProperties != null)
        {
            this.mCurrentGame.CacheProperties(gameProperties);
            SendMonoMessage(PhotonNetworkingMessage.OnPhotonCustomRoomPropertiesChanged, gameProperties);
            if (PhotonNetwork.automaticallySyncScene)
            {
                this.LoadLevelIfSynced();   // will load new scene if sceneName was changed
            }
        }

        if (pActorProperties != null && pActorProperties.Count > 0)
        {
            if (targetActorNr > 0)
            {
                // we have a single entry in the pActorProperties with one
                // user's name
                // targets MUST exist before you set properties
                PhotonPlayer target = this.GetPlayerWithID(targetActorNr);
                if (target != null)
                {
                    Hashtable props = this.GetActorPropertiesForActorNr(pActorProperties, targetActorNr);
                    target.InternalCacheProperties(props);
                    SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, target, props);
                }
            }
            else
            {
                // in this case, we've got a key-value pair per actor (each
                // value is a hashtable with the actor's properties then)
                int actorNr;
                Hashtable props;
                string newName;
                PhotonPlayer target;

                foreach (object key in pActorProperties.Keys)
                {
                    actorNr = (int)key;
                    props = (Hashtable)pActorProperties[key];
                    newName = (string)props[ActorProperties.PlayerName];

                    target = this.GetPlayerWithID(actorNr);
                    if (target == null)
                    {
                        target = new PhotonPlayer(false, actorNr, newName);
                        this.AddNewPlayer(actorNr, target);
                    }

                    target.InternalCacheProperties(props);
                    SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, target, props);
                }
            }
        }
    }

    private void AddNewPlayer(int ID, PhotonPlayer player)
    {
        if (!this.mActors.ContainsKey(ID))
        {
            this.mActors[ID] = player;
            RebuildPlayerListCopies();
        }
        else
        {
            Debug.LogError("Adding player twice: " + ID);
        }
    }

    void RemovePlayer(int ID, PhotonPlayer player)
    {
        this.mActors.Remove(ID);
        if (!player.isLocal)
        {
            RebuildPlayerListCopies();
        }
    }

    void RebuildPlayerListCopies()
    {
        this.mPlayerListCopy = new PhotonPlayer[this.mActors.Count];
        this.mActors.Values.CopyTo(this.mPlayerListCopy, 0);

        List<PhotonPlayer> otherP = new List<PhotonPlayer>();
        foreach (PhotonPlayer player in this.mPlayerListCopy)
        {
            if (!player.isLocal)
            {
                otherP.Add(player);
            }
        }

        this.mOtherPlayerListCopy = otherP.ToArray();
    }

    /// <summary>
    /// Resets the PhotonView "lastOnSerializeDataSent" so that "OnReliable" synched PhotonViews send a complete state to new clients (if the state doesnt change, no messages would be send otherwise!).
    /// Note that due to this reset, ALL other players will receive the full OnSerialize.
    /// </summary>
    private void ResetPhotonViewsOnSerialize()
    {
        foreach (PhotonView photonView in this.photonViewList.Values)
        {
            photonView.lastOnSerializeDataSent = null;
        }
    }

    /// <summary>
    /// Called when the event Leave (of some other player) arrived.
    /// Cleans game objects, views locally. The master will also clean the
    /// </summary>
    /// <param name="actorID">ID of player who left.</param>
    private void HandleEventLeave(int actorID)
    {
        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            Debug.Log("HandleEventLeave for player ID: " + actorID);


        // actorNr is fetched out of event above
        if (actorID < 0 || !this.mActors.ContainsKey(actorID))
        {
            Debug.LogError(String.Format("Received event Leave for unknown player ID: {0}", actorID));
            return;
        }

        PhotonPlayer player = this.GetPlayerWithID(actorID);
        if (player == null)
        {
            Debug.LogError("HandleEventLeave for player ID: " + actorID + " has no PhotonPlayer!");
        }

        // having a new master before calling destroy for the leaving player is important!
        // so we elect a new masterclient and ignore the leaving player (who is still in playerlists).
        this.CheckMasterClient(actorID);


        // destroy objects & buffered messages
        if (this.mCurrentGame != null && this.mCurrentGame.autoCleanUp)
        {
            this.DestroyPlayerObjects(actorID, true);
        }

        RemovePlayer(actorID, player);

        // finally, send notification (the playerList and masterclient are now updated)
        SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerDisconnected, player);
    }

    /// <summary>Picks the new master client from player list, if the current Master is leaving (leavingPlayerId) or if no master was assigned so far.</summary>
    /// <param name="leavingPlayerId">
    /// The ignored player is the one who's leaving and should not become master (again). Pass -1 to select any player from the list.
    /// </param>
    private void CheckMasterClient(int leavingPlayerId)
    {
        bool currentMasterIsLeaving = this.mMasterClient != null && this.mMasterClient.ID == leavingPlayerId;
        bool someoneIsLeaving = leavingPlayerId > 0;

        // return early if SOME player (leavingId > 0) is leaving AND it's NOT the current master
        if (someoneIsLeaving && !currentMasterIsLeaving)
        {
            return;
        }

        // picking the player with lowest ID (longest in game).
        if (this.mActors.Count <= 1)
        {
            this.mMasterClient = this.mLocalActor;
        }
        else
        {
            // keys in mActors are their actorNumbers
            int lowestActorNumber = Int32.MaxValue;
            foreach (int key in this.mActors.Keys)
            {
                if (key < lowestActorNumber && key != leavingPlayerId)
                {
                    lowestActorNumber = key;
                }
            }

            this.mMasterClient = this.mActors[lowestActorNumber];
        }

        // make a callback ONLY when a player/Master left
        if (someoneIsLeaving)
        {
            SendMonoMessage(PhotonNetworkingMessage.OnMasterClientSwitched, this.mMasterClient);
        }
    }

    /// <summary>
    /// Returns the lowest player.ID - used for Master Client picking.
    /// </summary>
    /// <remarks></remarks>
    private static int ReturnLowestPlayerId(PhotonPlayer[] players, int playerIdToIgnore)
    {
        if (players == null || players.Length == 0)
        {
            return -1;
        }

        int lowestActorNumber = Int32.MaxValue;
        for (int i = 0; i < players.Length; i++)
        {
            PhotonPlayer photonPlayer = players[i];
            if (photonPlayer.ID == playerIdToIgnore)
            {
                continue;
            }

            if (photonPlayer.ID < lowestActorNumber)
            {
                lowestActorNumber = photonPlayer.ID;
            }
        }

        return lowestActorNumber;
    }

    internal protected bool SetMasterClient(int playerId, bool sync)
    {
        bool masterReplaced = this.mMasterClient != null && this.mMasterClient.ID != playerId;
        if (!masterReplaced || !this.mActors.ContainsKey(playerId))
        {
            return false;
        }

        if (sync)
        {
            bool sent = this.OpRaiseEvent(PunEvent.AssignMaster, new Hashtable() { { (byte)1, playerId } }, true, null);
            if (!sent)
            {
                return false;
            }
        }

        this.hasSwitchedMC = true;
        this.mMasterClient = this.mActors[playerId];
        SendMonoMessage(PhotonNetworkingMessage.OnMasterClientSwitched, this.mMasterClient);    // we only callback when an actual change is done
        return true;
    }

    private Hashtable GetActorPropertiesForActorNr(Hashtable actorProperties, int actorNr)
    {
        if (actorProperties.ContainsKey(actorNr))
        {
            return (Hashtable)actorProperties[actorNr];
        }

        return actorProperties;
    }

    private PhotonPlayer GetPlayerWithID(int number)
    {
        if (this.mActors != null && this.mActors.ContainsKey(number))
        {
            return this.mActors[number];
        }

        return null;
    }

    private void SendPlayerName()
    {
        if (this.State == global::PeerState.Joining)
        {
            // this means, the join on the gameServer is sent (with an outdated name). send the new when in game
            this.mPlayernameHasToBeUpdated = true;
            return;
        }

        if (this.mLocalActor != null)
        {
            this.mLocalActor.name = this.PlayerName;
            Hashtable properties = new Hashtable();
            properties[ActorProperties.PlayerName] = this.PlayerName;
            if (this.mLocalActor.ID > 0)
            {
                this.OpSetPropertiesOfActor(this.mLocalActor.ID, properties, true, (byte)0);
                this.mPlayernameHasToBeUpdated = false;
            }
        }
    }

    private void GameEnteredOnGameServer(OperationResponse operationResponse)
    {
        if (operationResponse.ReturnCode != 0)
        {
            switch (operationResponse.OperationCode)
            {
                case OperationCode.CreateGame:
                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                    {
                        Debug.Log("Create failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage);
                    }
                    SendMonoMessage(PhotonNetworkingMessage.OnPhotonCreateRoomFailed, operationResponse.ReturnCode, operationResponse.DebugMessage);
                    break;
                case OperationCode.JoinGame:
                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                    {
                        Debug.Log("Join failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage);
                        if (operationResponse.ReturnCode == ErrorCode.GameDoesNotExist)
                        {
                            Debug.Log("Most likely the game became empty during the switch to GameServer.");
                        }
                    }
                    SendMonoMessage(PhotonNetworkingMessage.OnPhotonJoinRoomFailed, operationResponse.ReturnCode, operationResponse.DebugMessage);
                    break;
                case OperationCode.JoinRandomGame:
                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                    {
                        Debug.Log("Join failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage);
                        if (operationResponse.ReturnCode == ErrorCode.GameDoesNotExist)
                        {
                            Debug.Log("Most likely the game became empty during the switch to GameServer.");
                        }
                    }
                    SendMonoMessage(PhotonNetworkingMessage.OnPhotonRandomJoinFailed, operationResponse.ReturnCode, operationResponse.DebugMessage);
                    break;
            }

            this.DisconnectToReconnect();
            return;
        }

        this.State = global::PeerState.Joined;
        this.mRoomToGetInto.isLocalClientInside = true;

        Hashtable actorProperties = (Hashtable)operationResponse[ParameterCode.PlayerProperties];
        Hashtable gameProperties = (Hashtable)operationResponse[ParameterCode.GameProperties];
        this.ReadoutProperties(gameProperties, actorProperties, 0);

        // the local player's actor-properties are not returned in join-result. add this player to the list
        int localActorNr = (int)operationResponse[ParameterCode.ActorNr];

        this.ChangeLocalID(localActorNr);
        this.CheckMasterClient(-1);

        if (this.mPlayernameHasToBeUpdated)
        {
            this.SendPlayerName();
        }

        switch (operationResponse.OperationCode)
        {
            case OperationCode.CreateGame:
                SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom);
                break;
            case OperationCode.JoinGame:
            case OperationCode.JoinRandomGame:
                // the mono message for this is sent at another place
                break;
        }
    }

    private Hashtable GetLocalActorProperties()
    {
        if (PhotonNetwork.player != null)
        {
            return PhotonNetwork.player.allProperties;
        }

        Hashtable actorProperties = new Hashtable();
        actorProperties[ActorProperties.PlayerName] = this.PlayerName;
        return actorProperties;
    }

    public void ChangeLocalID(int newID)
    {
        if (this.mLocalActor == null)
        {
            Debug.LogWarning(
                string.Format(
                    "Local actor is null or not in mActors! mLocalActor: {0} mActors==null: {1} newID: {2}",
                    this.mLocalActor,
                    this.mActors == null,
                    newID));
        }

        if (this.mActors.ContainsKey(this.mLocalActor.ID))
        {
            this.mActors.Remove(this.mLocalActor.ID);
        }

        this.mLocalActor.InternalChangeLocalID(newID);
        this.mActors[this.mLocalActor.ID] = this.mLocalActor;
        this.RebuildPlayerListCopies();
    }

    #endregion

    #region Operations

    /// <summary>NetworkingPeer.OpCreateGame</summary>
    public bool OpCreateGame(string roomName, RoomOptions roomOptions, TypedLobby typedLobby)
    {
        bool onGameServer = this.server == ServerConnection.GameServer;
        if (!onGameServer)
        {
            this.mRoomOptionsForCreate = roomOptions;
            this.mRoomToGetInto = new Room(roomName, roomOptions);
            this.mRoomToEnterLobby = typedLobby ?? ((this.insideLobby) ? this.lobby : null);  // use given lobby, or active lobby (if any active) or none
        }

        this.mLastJoinType = JoinType.CreateGame;
        return base.OpCreateRoom(roomName, roomOptions, this.mRoomToEnterLobby, this.GetLocalActorProperties(), onGameServer);
    }

    /// <summary>NetworkingPeer.OpJoinRoom</summary>
    public bool OpJoinRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby, bool createIfNotExists)
    {
        bool onGameServer = this.server == ServerConnection.GameServer;
        if (!onGameServer)
        {
            // roomOptions and typedLobby will be null, unless createIfNotExists is true
            this.mRoomOptionsForCreate = roomOptions;
            this.mRoomToGetInto = new Room(roomName, roomOptions);
            this.mRoomToEnterLobby = null;
            if (createIfNotExists)
            {
                this.mRoomToEnterLobby = typedLobby ?? ((this.insideLobby) ? this.lobby : null);  // use given lobby, or active lobby (if any active) or none
            }
        }

        this.mLastJoinType = (createIfNotExists) ? JoinType.JoinOrCreateOnDemand : JoinType.JoinGame;
        return base.OpJoinRoom(roomName, roomOptions, this.mRoomToEnterLobby, createIfNotExists, this.GetLocalActorProperties(), onGameServer);
    }

    /// <summary>NetworkingPeer.OpJoinRandomRoom</summary>
    /// <remarks>this override just makes sure we have a mRoomToGetInto, even if it's blank (the properties provided in this method are filters. they are not set when we join the game)</remarks>
    public override bool OpJoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, Hashtable playerProperties, MatchmakingMode matchingType, TypedLobby typedLobby, string sqlLobbyFilter)
    {
        this.mRoomToGetInto = new Room(null, null);
        this.mRoomToEnterLobby = null;  // join random never stores the lobby. the following join will not affect the room lobby
        // if typedLobby is null, the server will automatically use the active lobby or default, which is what we want anyways

        this.mLastJoinType = JoinType.JoinRandomGame;
        return base.OpJoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, playerProperties, matchingType, typedLobby, sqlLobbyFilter);
    }

    /// <summary>
    /// Operation Leave will exit any current room.
    /// </summary>
    /// <remarks>
    /// This also happens when you disconnect from the server.
    /// Disconnect might be a step less if you don't want to create a new room on the same server.
    /// </remarks>
    /// <returns></returns>
    public virtual bool OpLeave()
    {
        if (this.State != global::PeerState.Joined)
        {
            Debug.LogWarning("Not sending leave operation. State is not 'Joined': " + this.State);
            return false;
        }

        return this.OpCustom((byte)OperationCode.Leave, null, true, 0);
    }

    public override bool OpRaiseEvent(byte eventCode, object customEventContent, bool sendReliable, RaiseEventOptions raiseEventOptions)
    {
        if (PhotonNetwork.offlineMode)
        {
            return false;
        }

        return base.OpRaiseEvent(eventCode, customEventContent, sendReliable, raiseEventOptions);
    }

    #endregion

    #region Implementation of IPhotonPeerListener

    public void DebugReturn(DebugLevel level, string message)
    {
        this.externalListener.DebugReturn(level, message);
    }

    public void OnOperationResponse(OperationResponse operationResponse)
    {
        if (PhotonNetwork.networkingPeer.State == global::PeerState.Disconnecting)
        {
            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            {
                Debug.Log("OperationResponse ignored while disconnecting. Code: " + operationResponse.OperationCode);
            }
            return;
        }

        // extra logging for error debugging (helping developers with a bit of automated analysis)
        if (operationResponse.ReturnCode == 0)
        {
            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                Debug.Log(operationResponse.ToString());
        }
        else
        {
            if (operationResponse.ReturnCode == ErrorCode.OperationNotAllowedInCurrentState)
            {
                Debug.LogError("Operation " + operationResponse.OperationCode + " could not be executed (yet). Wait for state JoinedLobby or ConnectedToMaster and their callbacks before calling operations. WebRPCs need a server-side configuration. Enum OperationCode helps identify the operation.");
            }
            else if (operationResponse.ReturnCode == ErrorCode.WebHookCallFailed)
            {
                Debug.LogError("Operation " + operationResponse.OperationCode + " failed in a server-side plugin. Check the configuration in the Dashboard. Message from server-plugin: " + operationResponse.DebugMessage);
            }
            else if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            {
                Debug.LogError("Operation failed: " + operationResponse.ToStringFull() + " Server: " + this.server);
            }
        }

        // use the "secret" or "token" whenever we get it. doesn't really matter if it's in AuthResponse.
        if (operationResponse.Parameters.ContainsKey(ParameterCode.Secret))
        {
            if (this.CustomAuthenticationValues == null)
            {
                this.CustomAuthenticationValues = new AuthenticationValues();
                // this.DebugReturn(DebugLevel.ERROR, "Server returned secret. Created CustomAuthenticationValues.");
            }

            this.CustomAuthenticationValues.Secret = operationResponse[ParameterCode.Secret] as string;
        }

        switch (operationResponse.OperationCode)
        {
            case OperationCode.Authenticate:
                {
                    // PeerState oldState = this.State;

                    if (operationResponse.ReturnCode != 0)
                    {
                        if (operationResponse.ReturnCode == ErrorCode.InvalidOperationCode)
                        {
                            Debug.LogError(string.Format("If you host Photon yourself, make sure to start the 'Instance LoadBalancing' "+ this.ServerAddress));
                        }
                        else if (operationResponse.ReturnCode == ErrorCode.InvalidAuthentication)
                        {
                            Debug.LogError(string.Format("The appId this client sent is unknown on the server (Cloud). Check settings. If using the Cloud, check account."));
                            SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, DisconnectCause.InvalidAuthentication);
                        }
                        else if (operationResponse.ReturnCode == ErrorCode.CustomAuthenticationFailed)
                        {
                            Debug.LogError(string.Format("Custom Authentication failed (either due to user-input or configuration or AuthParameter string format). Calling: OnCustomAuthenticationFailed()"));
                            SendMonoMessage(PhotonNetworkingMessage.OnCustomAuthenticationFailed, operationResponse.DebugMessage);
                        }
                        else
                        {
                            Debug.LogError(string.Format("Authentication failed: '{0}' Code: {1}", operationResponse.DebugMessage, operationResponse.ReturnCode));
                        }

                        this.State = global::PeerState.Disconnecting;
                        this.Disconnect();

                        if (operationResponse.ReturnCode == ErrorCode.MaxCcuReached)
                        {
                            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                                Debug.LogWarning(string.Format("Currently, the limit of users is reached for this title. Try again later. Disconnecting"));
                            SendMonoMessage(PhotonNetworkingMessage.OnPhotonMaxCccuReached);
                            SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, DisconnectCause.MaxCcuReached);
                        }
                        else if (operationResponse.ReturnCode == ErrorCode.InvalidRegion)
                        {
                            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                                Debug.LogError(string.Format("The used master server address is not available with the subscription currently used. Got to Photon Cloud Dashboard or change URL. Disconnecting."));
                            SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, DisconnectCause.InvalidRegion);
                        }
                        else if (operationResponse.ReturnCode == ErrorCode.AuthenticationTicketExpired)
                        {
                            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                                Debug.LogError(string.Format("The authentication ticket expired. You need to connect (and authenticate) again. Disconnecting."));
                            SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, DisconnectCause.AuthenticationTicketExpired);
                        }
                        break;
                    }
                    else
                    {
                        if (this.server == ServerConnection.NameServer)
                        {
                            // on the NameServer, authenticate returns the MasterServer address for a region and we hop off to there
                            this.MasterServerAddress = operationResponse[ParameterCode.Address] as string;
                            this.DisconnectToReconnect();
                        }
                        else if (this.server == ServerConnection.MasterServer)
                        {
                            if (PhotonNetwork.autoJoinLobby)
                            {
                                this.State = global::PeerState.Authenticated;
                                this.OpJoinLobby(this.lobby);
                            }
                            else
                            {
                                this.State = global::PeerState.ConnectedToMaster;
                                NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectedToMaster);
                            }
                        }
                        else if (this.server == ServerConnection.GameServer)
                        {
                            this.State = global::PeerState.Joining;

                            if (this.mLastJoinType == JoinType.JoinGame || this.mLastJoinType == JoinType.JoinRandomGame || this.mLastJoinType == JoinType.JoinOrCreateOnDemand)
                            {
                                // if we just "join" the game, do so. if we wanted to "create the room on demand", we have to send this to the game server as well.
                                this.OpJoinRoom(this.mRoomToGetInto.name, this.mRoomOptionsForCreate, this.mRoomToEnterLobby, this.mLastJoinType == JoinType.JoinOrCreateOnDemand);
                            }
                            else if (this.mLastJoinType == JoinType.CreateGame)
                            {
                                // on the game server, we have to apply the room properties that were chosen for creation of the room, so we use this.mRoomToGetInto
                                this.OpCreateGame(this.mRoomToGetInto.name, this.mRoomOptionsForCreate, this.mRoomToEnterLobby);
                            }

                            break;
                        }
                    }
                    break;
                }

            case OperationCode.GetRegions:
                // Debug.Log("GetRegions returned: " + operationResponse.ToStringFull());

                if (operationResponse.ReturnCode == ErrorCode.InvalidAuthentication)
                {
                    Debug.LogError(string.Format("The appId this client sent is unknown on the server (Cloud). Check settings. If using the Cloud, check account."));
                    SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, DisconnectCause.InvalidAuthentication);

                    this.State = global::PeerState.Disconnecting;
                    this.Disconnect();
                    return;
                }

                string[] regions = operationResponse[ParameterCode.Region] as string[];
                string[] servers = operationResponse[ParameterCode.Address] as string[];

                if (regions == null || servers == null || regions.Length != servers.Length)
                {
                    Debug.LogError("The region arrays from Name Server are not ok. Must be non-null and same length.");
                    break;
                }

                this.AvailableRegions = new List<Region>(regions.Length);
                for (int i = 0; i < regions.Length; i++)
                {
                    string regionCodeString = regions[i];
                    if (string.IsNullOrEmpty(regionCodeString))
                    {
                        continue;
                    }
                    regionCodeString = regionCodeString.ToLower();

                    CloudRegionCode code = Region.Parse(regionCodeString);
                    this.AvailableRegions.Add(new Region() { Code = code, HostAndPort = servers[i] });
                }

                // PUN assumes you fetch the name-server's list of regions to ping them
                if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.BestRegion)
                {
                    PhotonHandler.PingAvailableRegionsAndConnectToBest();
                }
                break;

            case OperationCode.CreateGame:
                {
                    if (this.server == ServerConnection.GameServer)
                    {
                        this.GameEnteredOnGameServer(operationResponse);
                    }
                    else
                    {
                        if (operationResponse.ReturnCode != 0)
                        {
                            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                                Debug.LogWarning(string.Format("CreateRoom failed, client stays on masterserver: {0}.", operationResponse.ToStringFull()));

                            SendMonoMessage(PhotonNetworkingMessage.OnPhotonCreateRoomFailed);
                            break;
                        }

                        string gameID = (string) operationResponse[ParameterCode.RoomName];
                        if (!string.IsNullOrEmpty(gameID))
                        {
                            // is only sent by the server's response, if it has not been
                            // sent with the client's request before!
                            this.mRoomToGetInto.name = gameID;
                        }

                        this.mGameserver = (string)operationResponse[ParameterCode.Address];
                        this.DisconnectToReconnect();
                    }

                    break;
                }

            case OperationCode.JoinGame:
                {
                    if (this.server != ServerConnection.GameServer)
                    {
                        if (operationResponse.ReturnCode != 0)
                        {
                            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                                Debug.Log(string.Format("JoinRoom failed (room maybe closed by now). Client stays on masterserver: {0}. State: {1}", operationResponse.ToStringFull(), this.State));

                            SendMonoMessage(PhotonNetworkingMessage.OnPhotonJoinRoomFailed);
                            break;
                        }

                        this.mGameserver = (string)operationResponse[ParameterCode.Address];
                        this.DisconnectToReconnect();
                    }
                    else
                    {
                        this.GameEnteredOnGameServer(operationResponse);
                    }

                    break;
                }

            case OperationCode.JoinRandomGame:
                {
                    // happens only on master. on gameserver, this is a regular join (we don't need to find a random game again)
                    // the operation OpJoinRandom either fails (with returncode 8) or returns game-to-join information
                    if (operationResponse.ReturnCode != 0)
                    {
                        if (operationResponse.ReturnCode == ErrorCode.NoRandomMatchFound)
                        {
                            if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
                                Debug.Log("JoinRandom failed: No open game. Calling: OnPhotonRandomJoinFailed() and staying on master server.");
                        }
                        else if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                        {
                            Debug.LogWarning(string.Format("JoinRandom failed: {0}.", operationResponse.ToStringFull()));
                        }

                        SendMonoMessage(PhotonNetworkingMessage.OnPhotonRandomJoinFailed, operationResponse.ReturnCode, operationResponse.DebugMessage);
                        break;
                    }

                    string roomName = (string)operationResponse[ParameterCode.RoomName];
                    this.mRoomToGetInto.name = roomName;
                    this.mGameserver = (string)operationResponse[ParameterCode.Address];
                    this.DisconnectToReconnect();
                    break;
                }

            case OperationCode.JoinLobby:
                this.State = global::PeerState.JoinedLobby;
                this.insideLobby = true;
                SendMonoMessage(PhotonNetworkingMessage.OnJoinedLobby);

                // this.mListener.joinLobbyReturn();
                break;
            case OperationCode.LeaveLobby:
                this.State = global::PeerState.Authenticated;
                this.LeftLobbyCleanup();    // will set insideLobby = false
                break;

            case OperationCode.Leave:
                this.DisconnectToReconnect();
                break;

            case OperationCode.SetProperties:
                // this.mListener.setPropertiesReturn(returnCode, debugMsg);
                break;

            case OperationCode.GetProperties:
                {
                    Hashtable actorProperties = (Hashtable)operationResponse[ParameterCode.PlayerProperties];
                    Hashtable gameProperties = (Hashtable)operationResponse[ParameterCode.GameProperties];
                    this.ReadoutProperties(gameProperties, actorProperties, 0);

                    // RemoveByteTypedPropertyKeys(actorProperties, false);
                    // RemoveByteTypedPropertyKeys(gameProperties, false);
                    // this.mListener.getPropertiesReturn(gameProperties, actorProperties, returnCode, debugMsg);
                    break;
                }

            case OperationCode.RaiseEvent:
                // this usually doesn't give us a result. only if the caching is affected the server will send one.
                break;

            case OperationCode.FindFriends:
                bool[] onlineList = operationResponse[ParameterCode.FindFriendsResponseOnlineList] as bool[];
                string[] roomList = operationResponse[ParameterCode.FindFriendsResponseRoomIdList] as string[];

                if (onlineList != null && roomList != null && this.friendListRequested != null && onlineList.Length == this.friendListRequested.Length)
                {
                    List<FriendInfo> friendList = new List<FriendInfo>(this.friendListRequested.Length);
                    for (int index = 0; index < this.friendListRequested.Length; index++)
                    {
                        FriendInfo friend = new FriendInfo();
                        friend.Name = this.friendListRequested[index];
                        friend.Room = roomList[index];
                        friend.IsOnline = onlineList[index];
                        friendList.Insert(index, friend);
                    }
                    PhotonNetwork.Friends = friendList;
                }
                else
                {
                    // any of the lists is null and shouldn't. print a error
                    Debug.LogError("FindFriends failed to apply the result, as a required value wasn't provided or the friend list length differed from result.");
                }

                this.friendListRequested = null;
                this.isFetchingFriends = false;
                this.friendListTimestamp = Environment.TickCount;
                if (this.friendListTimestamp == 0)
                {
                    this.friendListTimestamp = 1;   // makes sure the timestamp is not accidentally 0
                }

                SendMonoMessage(PhotonNetworkingMessage.OnUpdatedFriendList);
                break;

            case OperationCode.WebRpc:
                SendMonoMessage(PhotonNetworkingMessage.OnWebRpcResponse, operationResponse);
                break;

            default:
                Debug.LogWarning(string.Format("OperationResponse unhandled: {0}", operationResponse.ToString()));
                break;
        }

        this.externalListener.OnOperationResponse(operationResponse);
    }


    /// <summary>Contains the list of names of friends to look up their state on the server.</summary>
    private string[] friendListRequested;

    /// <summary>
    /// Age of friend list info (in milliseconds). It's 0 until a friend list is fetched.
    /// </summary>
    protected internal int FriendsListAge { get { return (this.isFetchingFriends || this.friendListTimestamp == 0) ? 0 : Environment.TickCount - this.friendListTimestamp; } }

    private int friendListTimestamp;

    /// <summary>Internal flag to know if the client currently fetches a friend list.</summary>
    private bool isFetchingFriends;

    /// <summary>
    /// Request the rooms and online status for a list of friends. All client must set a unique username via PlayerName property. The result is available in this.Friends.
    /// </summary>
    /// <remarks>
    /// Used on Master Server to find the rooms played by a selected list of users.
    /// The result will be mapped to LoadBalancingClient.Friends when available.
    /// The list is initialized by OpFindFriends on first use (before that, it is null).
    ///
    /// Users identify themselves by setting a PlayerName in the LoadBalancingClient instance.
    /// This in turn will send the name in OpAuthenticate after each connect (to master and game servers).
    /// Note: Changing a player's name doesn't make sense when using a friend list.
    ///
    /// The list of usernames must be fetched from some other source (not provided by Photon).
    ///
    ///
    /// Internal:
    /// The server response includes 2 arrays of info (each index matching a friend from the request):
    /// ParameterCode.FindFriendsResponseOnlineList = bool[] of online states
    /// ParameterCode.FindFriendsResponseRoomIdList = string[] of room names (empty string if not in a room)
    /// </remarks>
    /// <param name="friendsToFind">Array of friend's names (make sure they are unique).</param>
    /// <returns>If the operation could be sent (requires connection, only one request is allowed at any time). Always false in offline mode.</returns>
    public override bool OpFindFriends(string[] friendsToFind)
    {
        if (this.isFetchingFriends)
        {
            return false;   // fetching friends currently, so don't do it again (avoid changing the list while fetching friends)
        }

        this.friendListRequested = friendsToFind;
        this.isFetchingFriends = true;

        return base.OpFindFriends(friendsToFind);
    }

    public void OnStatusChanged(StatusCode statusCode)
    {
        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            Debug.Log(string.Format("OnStatusChanged: {0}", statusCode.ToString()));

        switch (statusCode)
        {
            case StatusCode.Connect:
                if (this.State == global::PeerState.ConnectingToNameServer)
                {
                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
                        Debug.Log("Connected to NameServer.");

                    this.server = ServerConnection.NameServer;
                    if (this.CustomAuthenticationValues != null)
                    {
                        this.CustomAuthenticationValues.Secret = null;     // when connecting to NameServer, invalidate any auth values
                    }
                }

                if (this.State == global::PeerState.ConnectingToGameserver)
                {
                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
                        Debug.Log("Connected to gameserver.");

                    this.server = ServerConnection.GameServer;
                    this.State = global::PeerState.ConnectedToGameserver;
                }

                if (this.State == global::PeerState.ConnectingToMasterserver)
                {
                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
                        Debug.Log("Connected to masterserver.");

                    this.server = ServerConnection.MasterServer;
                    this.State = global::PeerState.ConnectedToMaster;

                    if (this.IsInitialConnect)
                    {
                        this.IsInitialConnect = false;  // after handling potential initial-connect issues with special messages, we are now sure we can reach a server
                        SendMonoMessage(PhotonNetworkingMessage.OnConnectedToPhoton);
                    }
                }

                this.EstablishEncryption();     // always enable encryption

                if (this.IsAuthorizeSecretAvailable)
                {
                    // if we have a token we don't have to wait for encryption (it is encrypted anyways, so encryption is just optional later on)
                    this.didAuthenticate = this.OpAuthenticate(this.mAppId, this.mAppVersionPun, this.PlayerName, this.CustomAuthenticationValues, this.CloudRegion.ToString());
                    if (this.didAuthenticate)
                    {
                        this.State = global::PeerState.Authenticating;
                    }
                }
                break;

            case StatusCode.EncryptionEstablished:
                // on nameserver, the "process" is stopped here, so the developer/game can either get regions or authenticate with a specific region
                if (this.server == ServerConnection.NameServer)
                {
                    this.State = global::PeerState.ConnectedToNameServer;

                    if (!this.didAuthenticate && this.CloudRegion == CloudRegionCode.none)
                    {
                        // this client is not setup to connect to a default region. find out which regions there are!
                        this.OpGetRegions(this.mAppId);
                    }
                }

                // we might need to authenticate automatically now, so the client can do anything at all
                if (!this.didAuthenticate && (!this.IsUsingNameServer || this.CloudRegion !=  CloudRegionCode.none))
                {
                    // once encryption is availble, the client should send one (secure) authenticate. it includes the AppId (which identifies your app on the Photon Cloud)
                    this.didAuthenticate = this.OpAuthenticate(this.mAppId, this.mAppVersionPun, this.PlayerName, this.CustomAuthenticationValues, this.CloudRegion.ToString());
                    if (this.didAuthenticate)
                    {
                        this.State = global::PeerState.Authenticating;
                    }
                }
                break;

            case StatusCode.EncryptionFailedToEstablish:
                Debug.LogError("Encryption wasn't established: " + statusCode + ". Going to authenticate anyways.");
                this.OpAuthenticate(this.mAppId, this.mAppVersionPun, this.PlayerName, this.CustomAuthenticationValues, this.CloudRegion.ToString());     // TODO: check if there are alternatives
                break;

            case StatusCode.Disconnect:
                this.didAuthenticate = false;
                this.isFetchingFriends = false;
                if (server == ServerConnection.GameServer) this.LeftRoomCleanup();
                if (server == ServerConnection.MasterServer) this.LeftLobbyCleanup();

                if (this.State == global::PeerState.DisconnectingFromMasterserver)
                {
                    if (this.Connect(this.mGameserver, ServerConnection.GameServer))
                    {
                        this.State = global::PeerState.ConnectingToGameserver;
                    }
                }
                else if (this.State == global::PeerState.DisconnectingFromGameserver || this.State == global::PeerState.DisconnectingFromNameServer)
                {
                    if (this.Connect(this.MasterServerAddress, ServerConnection.MasterServer))
                    {
                        this.State = global::PeerState.ConnectingToMasterserver;
                    }
                }
                else
                {
                    if (this.CustomAuthenticationValues != null)
                    {
                        this.CustomAuthenticationValues.Secret = null;  // invalidate any custom auth secrets
                    }

                    this.State = global::PeerState.PeerCreated; // if we set another state here, we could keep clients from connecting in OnDisconnectedFromPhoton right here.
                    SendMonoMessage(PhotonNetworkingMessage.OnDisconnectedFromPhoton);
                }
                break;

            case StatusCode.SecurityExceptionOnConnect:
            case StatusCode.ExceptionOnConnect:
                this.State = global::PeerState.PeerCreated;
                if (this.CustomAuthenticationValues != null)
                {
                    this.CustomAuthenticationValues.Secret = null;  // invalidate any custom auth secrets
                }

                DisconnectCause cause = (DisconnectCause)statusCode;
                SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, cause);
                break;

            case StatusCode.Exception:
                if (this.IsInitialConnect)
                {
                    Debug.LogError("Exception while connecting to: " + this.ServerAddress + ". Check if the server is available.");
                    if (this.ServerAddress == null || this.ServerAddress.StartsWith("127.0.0.1"))
                    {
                        Debug.LogWarning("The server address is 127.0.0.1 (localhost): Make sure the server is running on this machine. Android and iOS emulators have their own localhost.");
                        if (this.ServerAddress == this.mGameserver)
                        {
                            Debug.LogWarning("This might be a misconfiguration in the game server config. You need to edit it to a (public) address.");
                        }
                    }

                    this.State = global::PeerState.PeerCreated;
                    cause = (DisconnectCause)statusCode;
                    SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, cause);
                }
                else
                {
                    this.State = global::PeerState.PeerCreated;

                    cause = (DisconnectCause)statusCode;
                    SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, cause);
                }

                this.Disconnect();
                break;

            case StatusCode.TimeoutDisconnect:
            case StatusCode.ExceptionOnReceive:
            case StatusCode.DisconnectByServer:
            case StatusCode.DisconnectByServerLogic:
            case StatusCode.DisconnectByServerUserLimit:
                if (this.IsInitialConnect)
                {
                    Debug.LogWarning(statusCode + " while connecting to: " + this.ServerAddress + ". Check if the server is available.");

                    cause = (DisconnectCause)statusCode;
                    SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, cause);
                }
                else
                {
                    cause = (DisconnectCause)statusCode;
                    SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, cause);
                }
                if (this.CustomAuthenticationValues != null)
                {
                    this.CustomAuthenticationValues.Secret = null;  // invalidate any custom auth secrets
                }

                this.Disconnect();
                break;

            case StatusCode.SendError:
                // this.mListener.clientErrorReturn(statusCode);
                break;

            case StatusCode.QueueOutgoingReliableWarning:
            case StatusCode.QueueOutgoingUnreliableWarning:
            case StatusCode.QueueOutgoingAcksWarning:
            case StatusCode.QueueSentWarning:
                // this.mListener.warningReturn(statusCode);
                break;

            case StatusCode.QueueIncomingReliableWarning:
            case StatusCode.QueueIncomingUnreliableWarning:
                Debug.Log(statusCode + ". This client buffers many incoming messages. This is OK temporarily. With lots of these warnings, check if you send too much or execute messages too slow. " + (PhotonNetwork.isMessageQueueRunning? "":"Your isMessageQueueRunning is false. This can cause the issue temporarily.") );
                break;

            // // TCP "routing" is an option of Photon that's not currently needed (or supported) by PUN
            //case StatusCode.TcpRouterResponseOk:
            //    break;
            //case StatusCode.TcpRouterResponseEndpointUnknown:
            //case StatusCode.TcpRouterResponseNodeIdUnknown:
            //case StatusCode.TcpRouterResponseNodeNotReady:

            //    this.DebugReturn(DebugLevel.ERROR, "Unexpected router response: " + statusCode);
            //    break;

            default:

                // this.mListener.serverErrorReturn(statusCode.value());
                Debug.LogError("Received unknown status code: " + statusCode);
                break;
        }

        this.externalListener.OnStatusChanged(statusCode);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            Debug.Log(string.Format("OnEvent: {0}", photonEvent.ToString()));

        int actorNr = -1;
        PhotonPlayer originatingPlayer = null;

        if (photonEvent.Parameters.ContainsKey(ParameterCode.ActorNr))
        {
            actorNr = (int)photonEvent[ParameterCode.ActorNr];
            if (this.mActors.ContainsKey(actorNr))
            {
                originatingPlayer = (PhotonPlayer)this.mActors[actorNr];
            }
            //else
            //{
            //    // the actor sending this event is not in actorlist. this is usually no problem
            //    if (photonEvent.Code != (byte)LiteOpCode.Join)
            //    {
            //        Debug.LogWarning("Received event, but we do not have this actor:  " + actorNr);
            //    }
            //}
        }

        switch (photonEvent.Code)
        {
            case PunEvent.OwnershipRequest:
            {
                int[] requestValues = (int[]) photonEvent.Parameters[ParameterCode.CustomEventContent];
                int requestedViewId = requestValues[0];
                int currentOwner = requestValues[1];
                Debug.Log("Ev OwnershipRequest: " + photonEvent.Parameters.ToStringFull() + " ViewID: " + requestedViewId + " from: " + currentOwner + " Time: " + Environment.TickCount%1000);

                PhotonView requestedView = PhotonView.Find(requestedViewId);
                if (requestedView == null)
                {
                    Debug.LogWarning("Can't find PhotonView of incoming OwnershipRequest. ViewId not found: " + requestedViewId);
                    break;
                }

                Debug.Log("Ev OwnershipRequest PhotonView.ownershipTransfer: " + requestedView.ownershipTransfer + " .ownerId: " + requestedView.ownerId + " isOwnerActive: " + requestedView.isOwnerActive + ". This client's player: " + PhotonNetwork.player.ToStringFull());

                switch (requestedView.ownershipTransfer)
                {
                    case OwnershipOption.Fixed:
                        Debug.LogWarning("Ownership mode == fixed. Ignoring request.");
                        break;
                    case OwnershipOption.Takeover:
                        if (currentOwner == requestedView.ownerId)
                        {
                            // a takeover is successful automatically, if taken from current owner
                            requestedView.ownerId = actorNr;
                        }
                        break;
                    case OwnershipOption.Request:
                        if (currentOwner == PhotonNetwork.player.ID || PhotonNetwork.player.isMasterClient)
                        {
                            if ((requestedView.ownerId == PhotonNetwork.player.ID) || (PhotonNetwork.player.isMasterClient && !requestedView.isOwnerActive))
                            {
                                SendMonoMessage(PhotonNetworkingMessage.OnOwnershipRequest, new object[] {requestedView, originatingPlayer});
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
                break;

            case PunEvent.OwnershipTransfer:
                {
                    int[] transferViewToUserID = (int[]) photonEvent.Parameters[ParameterCode.CustomEventContent];
                    Debug.Log("Ev OwnershipTransfer. ViewID " + transferViewToUserID[0] + " to: " + transferViewToUserID[1] + " Time: " + Environment.TickCount%1000);

                    int requestedViewId = transferViewToUserID[0];
                    int newOwnerId = transferViewToUserID[1];

                    PhotonView pv = PhotonView.Find(requestedViewId);
                    pv.ownerId = newOwnerId;

                    break;
                }
            case EventCode.GameList:
                {
                    this.mGameList = new Dictionary<string, RoomInfo>();
                    Hashtable games = (Hashtable)photonEvent[ParameterCode.GameList];
                    foreach (DictionaryEntry game in games)
                    {
                        string gameName = (string)game.Key;
                        this.mGameList[gameName] = new RoomInfo(gameName, (Hashtable)game.Value);
                    }
                    mGameListCopy = new RoomInfo[mGameList.Count];
                    mGameList.Values.CopyTo(mGameListCopy, 0);
                    SendMonoMessage(PhotonNetworkingMessage.OnReceivedRoomListUpdate);
                    break;
                }

            case EventCode.GameListUpdate:
                {
                    Hashtable games = (Hashtable)photonEvent[ParameterCode.GameList];
                    foreach (DictionaryEntry room in games)
                    {
                        string gameName = (string)room.Key;
                        RoomInfo game = new RoomInfo(gameName, (Hashtable)room.Value);
                        if (game.removedFromList)
                        {
                            this.mGameList.Remove(gameName);
                        }
                        else
                        {
                            this.mGameList[gameName] = game;
                        }
                    }
                    this.mGameListCopy = new RoomInfo[this.mGameList.Count];
                    this.mGameList.Values.CopyTo(this.mGameListCopy, 0);
                    SendMonoMessage(PhotonNetworkingMessage.OnReceivedRoomListUpdate);
                    break;
                }

            case EventCode.QueueState:
                // not used anymore
                break;

            case EventCode.AppStats:
                // Debug.LogInfo("Received stats!");
                this.mPlayersInRoomsCount = (int)photonEvent[ParameterCode.PeerCount];
                this.mPlayersOnMasterCount = (int)photonEvent[ParameterCode.MasterPeerCount];
                this.mGameCount = (int)photonEvent[ParameterCode.GameCount];
                break;

            case EventCode.Join:
                // actorNr is fetched out of event above
                Hashtable actorProperties = (Hashtable)photonEvent[ParameterCode.PlayerProperties];
                if (originatingPlayer == null)
                {
                    bool isLocal = this.mLocalActor.ID == actorNr;
                    this.AddNewPlayer(actorNr, new PhotonPlayer(isLocal, actorNr, actorProperties));
                    this.ResetPhotonViewsOnSerialize(); // This sets the correct OnSerializeState for Reliable OnSerialize
                }

                if (actorNr == this.mLocalActor.ID)
                {
                    // in this player's 'own' join event, we get a complete list of players in the room, so check if we know all players
                    int[] actorsInRoom = (int[])photonEvent[ParameterCode.ActorList];
                    foreach (int actorNrToCheck in actorsInRoom)
                    {
                        if (this.mLocalActor.ID != actorNrToCheck && !this.mActors.ContainsKey(actorNrToCheck))
                        {
                            this.AddNewPlayer(actorNrToCheck, new PhotonPlayer(false, actorNrToCheck, string.Empty));
                        }
                    }

                    // joinWithCreateOnDemand can turn an OpJoin into creating the room. Then actorNumber is 1 and callback: OnCreatedRoom()
                    if (this.mLastJoinType == JoinType.JoinOrCreateOnDemand && this.mLocalActor.ID == 1)
                    {
                        SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom);
                    }
                    SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom); //Always send OnJoinedRoom

                }
                else
                {
                    SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerConnected, this.mActors[actorNr]);
                }
                break;

            case EventCode.Leave:
                this.HandleEventLeave(actorNr);
                break;

            case EventCode.PropertiesChanged:
                int targetActorNr = (int)photonEvent[ParameterCode.TargetActorNr];
                Hashtable gameProperties = null;
                Hashtable actorProps = null;
                if (targetActorNr == 0)
                {
                    gameProperties = (Hashtable)photonEvent[ParameterCode.Properties];
                }
                else
                {
                    actorProps = (Hashtable)photonEvent[ParameterCode.Properties];
                }

                this.ReadoutProperties(gameProperties, actorProps, targetActorNr);
                break;

            case PunEvent.RPC:
                //ts: each event now contains a single RPC. execute this
                // Debug.Log("Ev RPC from: " + originatingPlayer);
                this.ExecuteRPC(photonEvent[ParameterCode.Data] as Hashtable, originatingPlayer);
                break;

            case PunEvent.SendSerialize:
            case PunEvent.SendSerializeReliable:
                Hashtable serializeData = (Hashtable)photonEvent[ParameterCode.Data];
                //Debug.Log(serializeData.ToStringFull());

                int remoteUpdateServerTimestamp = (int)serializeData[(byte)0];
                short remoteLevelPrefix = -1;
                short initialDataIndex = 1;
                if (serializeData.ContainsKey((byte)1))
                {
                    remoteLevelPrefix = (short)serializeData[(byte)1];
                    initialDataIndex = 2;
                }

                for (short s = initialDataIndex; s < serializeData.Count; s++)
                {
                    this.OnSerializeRead(serializeData[s] as Hashtable, originatingPlayer, remoteUpdateServerTimestamp, remoteLevelPrefix);
                }
                break;

            case PunEvent.Instantiation:
                this.DoInstantiate((Hashtable)photonEvent[ParameterCode.Data], originatingPlayer, null);
                break;

            case PunEvent.CloseConnection:
                // MasterClient "requests" a disconnection from us
                if (originatingPlayer == null || !originatingPlayer.isMasterClient)
                {
                    Debug.LogError("Error: Someone else(" + originatingPlayer + ") then the masterserver requests a disconnect!");
                }
                else
                {
                    PhotonNetwork.LeaveRoom();
                }

                break;

            case PunEvent.DestroyPlayer:
                Hashtable evData = (Hashtable)photonEvent[ParameterCode.Data];
                int targetPlayerId = (int)evData[(byte)0];
                if (targetPlayerId >= 0)
                {
                    this.DestroyPlayerObjects(targetPlayerId, true);
                }
                else
                {
                    if (this.DebugOut >= DebugLevel.INFO) Debug.Log("Ev DestroyAll! By PlayerId: " + actorNr);
                    this.DestroyAll(true);
                }
                break;

            case PunEvent.Destroy:
                evData = (Hashtable)photonEvent[ParameterCode.Data];
                int instantiationId = (int)evData[(byte)0];
                // Debug.Log("Ev Destroy for viewId: " + instantiationId + " sent by owner: " + (instantiationId / PhotonNetwork.MAX_VIEW_IDS == actorNr) + " this client is owner: " + (instantiationId / PhotonNetwork.MAX_VIEW_IDS == this.mLocalActor.ID));


                PhotonView pvToDestroy = null;
                if (this.photonViewList.TryGetValue(instantiationId, out pvToDestroy))
                {
                    this.RemoveInstantiatedGO(pvToDestroy.gameObject, true);
                }
                else
                {
                    if (this.DebugOut >= DebugLevel.ERROR) Debug.LogError("Ev Destroy Failed. Could not find PhotonView with instantiationId " + instantiationId + ". Sent by actorNr: " + actorNr);
                }

                break;

            case PunEvent.AssignMaster:
                evData = (Hashtable)photonEvent[ParameterCode.Data];
                int newMaster = (int)evData[(byte)1];
                this.SetMasterClient(newMaster, false);
                break;

            default:
                if (photonEvent.Code < 200 && PhotonNetwork.OnEventCall != null)
                {
                    object content = photonEvent[ParameterCode.Data];
                    PhotonNetwork.OnEventCall(photonEvent.Code, content, actorNr);
                }
                else
                {
                    // actorNr might be null. it is fetched out of event on top of method
                    // Hashtable eventContent = (Hashtable) photonEvent[ParameterCode.Data];
                    // this.mListener.customEventAction(actorNr, eventCode, eventContent);
                    Debug.LogError("Error. Unhandled event: " + photonEvent);
                }
                break;
        }

        this.externalListener.OnEvent(photonEvent);
    }

    private void SendVacantViewIds()
    {
        Debug.Log("SendVacantViewIds()");
        List<int> vacantViews = new List<int>();
        foreach (PhotonView view in this.photonViewList.Values)
        {
            if (!view.isOwnerActive)
            {
                vacantViews.Add(view.viewID);
            }
        }

        Debug.Log("Sending vacant view IDs. Length: " + vacantViews.Count);
        //this.OpRaiseEvent(PunEvent.VacantViewIds, true, vacantViews.ToArray());
        this.OpRaiseEvent(PunEvent.VacantViewIds, vacantViews.ToArray(), true, null);
    }

    #endregion

    public static void SendMonoMessage(PhotonNetworkingMessage methodString, params object[] parameters)
    {
        HashSet<GameObject> objectsToCall;
        if (PhotonNetwork.SendMonoMessageTargets != null)
        {
            objectsToCall = PhotonNetwork.SendMonoMessageTargets;
        }
        else
        {
            objectsToCall = PhotonNetwork.FindGameObjectsWithComponent(PhotonNetwork.SendMonoMessageTargetType);
        }

        string methodName = methodString.ToString();
        object callParameter = (parameters != null && parameters.Length == 1) ? parameters[0] : parameters;
        foreach (GameObject gameObject in objectsToCall)
        {
            gameObject.SendMessage(methodName, callParameter, SendMessageOptions.DontRequireReceiver);
        }
    }

    // PHOTONVIEW/RPC related

    /// <summary>
    /// Executes a received RPC event
    /// </summary>
    public void ExecuteRPC(Hashtable rpcData, PhotonPlayer sender)
    {
        if (rpcData == null || !rpcData.ContainsKey((byte)0))
        {
            Debug.LogError("Malformed RPC; this should never occur. Content: " + SupportClass.DictionaryToString(rpcData));
            return;
        }

        // ts: updated with "flat" event data
        int netViewID = (int)rpcData[(byte)0]; // LIMITS PHOTONVIEWS&PLAYERS
        int otherSidePrefix = 0;    // by default, the prefix is 0 (and this is not being sent)
        if (rpcData.ContainsKey((byte)1))
        {
            otherSidePrefix = (short)rpcData[(byte)1];
        }

        string inMethodName;
        if (rpcData.ContainsKey((byte)5))
        {
            int rpcIndex = (byte)rpcData[(byte)5];  // LIMITS RPC COUNT
            if (rpcIndex > PhotonNetwork.PhotonServerSettings.RpcList.Count - 1)
            {
                Debug.LogError("Could not find RPC with index: " + rpcIndex + ". Going to ignore! Check PhotonServerSettings.RpcList");
                return;
            }
            else
            {
                inMethodName = PhotonNetwork.PhotonServerSettings.RpcList[rpcIndex];
            }
        }
        else
        {
            inMethodName = (string)rpcData[(byte)3];
        }

        object[] inMethodParameters = null;
        if (rpcData.ContainsKey((byte)4))
        {
            inMethodParameters = (object[])rpcData[(byte)4];
        }

        if (inMethodParameters == null)
        {
            inMethodParameters = new object[0];
        }

        PhotonView photonNetview = this.GetPhotonView(netViewID);
        if (photonNetview == null)
        {
            int viewOwnerId = netViewID/PhotonNetwork.MAX_VIEW_IDS;
            bool owningPv = (viewOwnerId == this.mLocalActor.ID);
            bool ownerSent = (viewOwnerId == sender.ID);

            if (owningPv)
            {
                Debug.LogWarning("Received RPC \"" + inMethodName + "\" for viewID " + netViewID + " but this PhotonView does not exist! View was/is ours." + (ownerSent ? " Owner called." : " Remote called.") + " By: " + sender.ID);
            }
            else
            {
                Debug.LogWarning("Received RPC \"" + inMethodName + "\" for viewID " + netViewID + " but this PhotonView does not exist! Was remote PV." + (ownerSent ? " Owner called." : " Remote called.") + " By: " + sender.ID + " Maybe GO was destroyed but RPC not cleaned up.");
            }
            return;
        }

        if (photonNetview.prefix != otherSidePrefix)
        {
            Debug.LogError(
                "Received RPC \"" + inMethodName + "\" on viewID " + netViewID + " with a prefix of " + otherSidePrefix
                + ", our prefix is " + photonNetview.prefix + ". The RPC has been ignored.");
            return;
        }

        // Get method name
        if (inMethodName == string.Empty)
        {
            Debug.LogError("Malformed RPC; this should never occur. Content: " + SupportClass.DictionaryToString(rpcData));
            return;
        }

        if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
            Debug.Log("Received RPC: " + inMethodName);


        // SetReceiving filtering
        if (photonNetview.group != 0 && !allowedReceivingGroups.Contains(photonNetview.group))
        {
            return; // Ignore group
        }

        Type[] argTypes = new Type[0];
        if (inMethodParameters.Length > 0)
        {
            argTypes = new Type[inMethodParameters.Length];
            int i = 0;
            for (int index = 0; index < inMethodParameters.Length; index++)
            {
                object objX = inMethodParameters[index];
                if (objX == null)
                {
                    argTypes[i] = null;
                }
                else
                {
                    argTypes[i] = objX.GetType();
                }

                i++;
            }
        }

        int receivers = 0;
        int foundMethods = 0;
        MonoBehaviour[] mbComponents = photonNetview.GetComponents<MonoBehaviour>();    // NOTE: we could possibly also cache MonoBehaviours per view?!
        for (int componentsIndex = 0; componentsIndex < mbComponents.Length; componentsIndex++)
        {
            MonoBehaviour monob = mbComponents[componentsIndex];
            if (monob == null)
            {
                Debug.LogError("ERROR You have missing MonoBehaviours on your gameobjects!");
                continue;
            }

            Type type = monob.GetType();

            // Get [RPC] methods from cache
            List<MethodInfo> cachedRPCMethods = null;
            if (this.monoRPCMethodsCache.ContainsKey(type))
            {
                cachedRPCMethods = this.monoRPCMethodsCache[type];
            }

            if (cachedRPCMethods == null)
            {
                List<MethodInfo> entries = SupportClass.GetMethods(type, typeof(RPC));

                this.monoRPCMethodsCache[type] = entries;
                cachedRPCMethods = entries;
            }

            if (cachedRPCMethods == null)
            {
                continue;
            }

            // Check cache for valid methodname+arguments
            for (int index = 0; index < cachedRPCMethods.Count; index++)
            {
                MethodInfo mInfo = cachedRPCMethods[index];
                if (mInfo.Name == inMethodName)
                {
                    foundMethods++;
                    ParameterInfo[] pArray = mInfo.GetParameters();
                    if (pArray.Length == argTypes.Length)
                    {
                        // Normal, PhotonNetworkMessage left out
                        if (this.CheckTypeMatch(pArray, argTypes))
                        {
                            receivers++;
                            object result = mInfo.Invoke((object)monob, inMethodParameters);
                            if (mInfo.ReturnType == typeof(IEnumerator))
                            {
                                monob.StartCoroutine((IEnumerator)result);
                            }
                        }
                    }
                    else if ((pArray.Length - 1) == argTypes.Length)
                    {
                        // Check for PhotonNetworkMessage being the last
                        if (this.CheckTypeMatch(pArray, argTypes))
                        {
                            if (pArray[pArray.Length - 1].ParameterType == typeof(PhotonMessageInfo))
                            {
                                receivers++;

                                int sendTime = (int)rpcData[(byte)2];
                                object[] deParamsWithInfo = new object[inMethodParameters.Length + 1];
                                inMethodParameters.CopyTo(deParamsWithInfo, 0);
                                deParamsWithInfo[deParamsWithInfo.Length - 1] = new PhotonMessageInfo(sender, sendTime, photonNetview);

                                object result = mInfo.Invoke((object)monob, deParamsWithInfo);
                                if (mInfo.ReturnType == typeof(IEnumerator))
                                {
                                    monob.StartCoroutine((IEnumerator)result);
                                }
                            }
                        }
                    }
                    else if (pArray.Length == 1 && pArray[0].ParameterType.IsArray)
                    {
                        receivers++;
                        object result = mInfo.Invoke((object)monob, new object[] { inMethodParameters });
                        if (mInfo.ReturnType == typeof(IEnumerator))
                        {
                            monob.StartCoroutine((IEnumerator)result);
                        }
                    }
                }
            }
        }

        // Error handling
        if (receivers != 1)
        {
            string argsString = string.Empty;
            for (int index = 0; index < argTypes.Length; index++)
            {
                Type ty = argTypes[index];
                if (argsString != string.Empty)
                {
                    argsString += ", ";
                }

                if (ty == null)
                {
                    argsString += "null";
                }
                else
                {
                    argsString += ty.Name;
                }
            }

            if (receivers == 0)
            {
                if (foundMethods == 0)
                {
                    Debug.LogError("PhotonView with ID " + netViewID + " has no method \"" + inMethodName + "\" marked with the [RPC](C#) or @RPC(JS) property! Args: " + argsString);
                }
                else
                {
                    Debug.LogError("PhotonView with ID " + netViewID + " has no method \"" + inMethodName + "\" that takes " + argTypes.Length + " argument(s): " + argsString);
                }
            }
            else
            {
                Debug.LogError("PhotonView with ID " + netViewID + " has " + receivers + " methods \"" + inMethodName + "\" that takes " + argTypes.Length + " argument(s): " + argsString + ". Should be just one?");
            }
        }
    }

    /// <summary>
    /// Check if all types match with parameters. We can have more paramters then types (allow last RPC type to be different).
    /// </summary>
    /// <param name="methodParameters"></param>
    /// <param name="callParameterTypes"></param>
    /// <returns>If the types-array has matching parameters (of method) in the parameters array (which may be longer).</returns>
    private bool CheckTypeMatch(ParameterInfo[] methodParameters, Type[] callParameterTypes)
    {
        if (methodParameters.Length < callParameterTypes.Length)
        {
            return false;
        }

        for (int index = 0; index < callParameterTypes.Length; index++)
        {
            Type type = methodParameters[index].ParameterType;
            //todo: check metro type usage
            if (callParameterTypes[index] != null && !type.Equals(callParameterTypes[index]))
            {
                return false;
            }
        }

        return true;
    }

    internal Hashtable SendInstantiate(string prefabName, Vector3 position, Quaternion rotation, int group, int[] viewIDs, object[] data, bool isGlobalObject)
    {
        // first viewID is now also the gameobject's instantiateId
        int instantiateId = viewIDs[0];   // LIMITS PHOTONVIEWS&PLAYERS

        //TODO: reduce hashtable key usage by using a parameter array for the various values
        Hashtable instantiateEvent = new Hashtable(); // This players info is sent via ActorID
        instantiateEvent[(byte)0] = prefabName;

        if (position != Vector3.zero)
        {
            instantiateEvent[(byte)1] = position;
        }

        if (rotation != Quaternion.identity)
        {
            instantiateEvent[(byte)2] = rotation;
        }

        if (group != 0)
        {
            instantiateEvent[(byte)3] = group;
        }

        // send the list of viewIDs only if there are more than one. else the instantiateId is the viewID
        if (viewIDs.Length > 1)
        {
            instantiateEvent[(byte)4] = viewIDs; // LIMITS PHOTONVIEWS&PLAYERS
        }

        if (data != null)
        {
            instantiateEvent[(byte)5] = data;
        }

        if (this.currentLevelPrefix > 0)
        {
            instantiateEvent[(byte)8] = this.currentLevelPrefix;    // photonview's / object's level prefix
        }

        instantiateEvent[(byte)6] = this.ServerTimeInMilliSeconds;
        instantiateEvent[(byte)7] = instantiateId;


        RaiseEventOptions options = new RaiseEventOptions();
        options.CachingOption = (isGlobalObject) ? EventCaching.AddToRoomCacheGlobal : EventCaching.AddToRoomCache;

        this.OpRaiseEvent(PunEvent.Instantiation, instantiateEvent, true, options);
        return instantiateEvent;
    }

    internal GameObject DoInstantiate(Hashtable evData, PhotonPlayer photonPlayer, GameObject resourceGameObject)
    {
        // some values always present:
        string prefabName = (string)evData[(byte)0];
        int serverTime = (int)evData[(byte)6];
        int instantiationId = (int)evData[(byte)7];

        Vector3 position;
        if (evData.ContainsKey((byte)1))
        {
            position = (Vector3)evData[(byte)1];
        }
        else
        {
            position = Vector3.zero;
        }

        Quaternion rotation = Quaternion.identity;
        if (evData.ContainsKey((byte)2))
        {
            rotation = (Quaternion)evData[(byte)2];
        }

        int group = 0;
        if (evData.ContainsKey((byte)3))
        {
            group = (int)evData[(byte)3];
        }

        short objLevelPrefix = 0;
        if (evData.ContainsKey((byte)8))
        {
            objLevelPrefix = (short)evData[(byte)8];
        }

        int[] viewsIDs;
        if (evData.ContainsKey((byte)4))
        {
            viewsIDs = (int[])evData[(byte)4];
        }
        else
        {
            viewsIDs = new int[1] { instantiationId };
        }

        object[] incomingInstantiationData;
        if (evData.ContainsKey((byte)5))
        {
            incomingInstantiationData = (object[])evData[(byte)5];
        }
        else
        {
            incomingInstantiationData = null;
        }

        // SetReceiving filtering
        if (group != 0 && !this.allowedReceivingGroups.Contains(group))
        {
            return null; // Ignore group
        }

        // load prefab, if it wasn't loaded before (calling methods might do this)
        if (resourceGameObject == null)
        {
            if (!NetworkingPeer.UsePrefabCache || !NetworkingPeer.PrefabCache.TryGetValue(prefabName, out resourceGameObject))
            {
                resourceGameObject = (GameObject)Resources.Load(prefabName, typeof(GameObject));
                if (NetworkingPeer.UsePrefabCache)
                {
                    NetworkingPeer.PrefabCache.Add(prefabName, resourceGameObject);
                }
            }

            if (resourceGameObject == null)
            {
                Debug.LogError("PhotonNetwork error: Could not Instantiate the prefab [" + prefabName + "]. Please verify you have this gameobject in a Resources folder.");
                return null;
            }
        }

        // now modify the loaded "blueprint" object before it becomes a part of the scene (by instantiating it)
        PhotonView[] resourcePVs = resourceGameObject.GetPhotonViewsInChildren();
        if (resourcePVs.Length != viewsIDs.Length)
        {
            throw new Exception("Error in Instantiation! The resource's PhotonView count is not the same as in incoming data.");
        }

        for (int i = 0; i < viewsIDs.Length; i++)
        {
            // NOTE instantiating the loaded resource will keep the viewID but would not copy instantiation data, so it's set below
            // so we only set the viewID and instantiationId now. the instantiationData can be fetched
            resourcePVs[i].viewID = viewsIDs[i];
            resourcePVs[i].prefix = objLevelPrefix;
            resourcePVs[i].instantiationId = instantiationId;
            resourcePVs[i].isRuntimeInstantiated = true;
        }

        this.StoreInstantiationData(instantiationId, incomingInstantiationData);

        // load the resource and set it's values before instantiating it:
        GameObject go = (GameObject)GameObject.Instantiate(resourceGameObject, position, rotation);

        for (int i = 0; i < viewsIDs.Length; i++)
        {
            // NOTE instantiating the loaded resource will keep the viewID but would not copy instantiation data, so it's set below
            // so we only set the viewID and instantiationId now. the instantiationData can be fetched
            resourcePVs[i].viewID = 0;
            resourcePVs[i].prefix = -1;
            resourcePVs[i].prefixBackup = -1;
            resourcePVs[i].instantiationId = -1;
            resourcePVs[i].isRuntimeInstantiated = false;
        }

        this.RemoveInstantiationData(instantiationId);
        
        // Send OnPhotonInstantiate callback to newly created GO.
        // GO will be enabled when instantiated from Prefab and it does not matter if the script is enabled or disabled.
        go.SendMessage(PhotonNetworkingMessage.OnPhotonInstantiate.ToString(), new PhotonMessageInfo(photonPlayer, serverTime, null), SendMessageOptions.DontRequireReceiver);
        return go;
    }

    private Dictionary<int, object[]> tempInstantiationData = new Dictionary<int, object[]>();

    private void StoreInstantiationData(int instantiationId, object[] instantiationData)
    {
        // Debug.Log("StoreInstantiationData() instantiationId: " + instantiationId + " tempInstantiationData.Count: " + tempInstantiationData.Count);
        tempInstantiationData[instantiationId] = instantiationData;
    }

    public object[] FetchInstantiationData(int instantiationId)
    {
        object[] data = null;
        if (instantiationId == 0)
        {
            return null;
        }

        tempInstantiationData.TryGetValue(instantiationId, out data);
        // Debug.Log("FetchInstantiationData() instantiationId: " + instantiationId + " tempInstantiationData.Count: " + tempInstantiationData.Count);
        return data;
    }

    private void RemoveInstantiationData(int instantiationId)
    {
        tempInstantiationData.Remove(instantiationId);
    }


    /// <summary>
    /// Destroys all Instantiates and RPCs locally and (if not localOnly) sends EvDestroy(player) and clears related events in the server buffer.
    /// </summary>
    public void DestroyPlayerObjects(int playerId, bool localOnly)
    {
        if (playerId <= 0)
        {
            Debug.LogError("Failed to Destroy objects of playerId: " + playerId);
            return;
        }

        if (!localOnly)
        {
            // clean server's Instantiate and RPC buffers
            this.OpRemoveFromServerInstantiationsOfPlayer(playerId);
            this.OpCleanRpcBuffer(playerId);

            // send Destroy(player) to anyone else
            this.SendDestroyOfPlayer(playerId);
        }

        // locally cleaning up that player's objects
        HashSet<GameObject> playersGameObjects = new HashSet<GameObject>();
        foreach (PhotonView view in this.photonViewList.Values)
        {
            if (view.CreatorActorNr == playerId)
            {
                playersGameObjects.Add(view.gameObject);
            }
        }

        // any non-local work is already done, so with the list of that player's objects, we can clean up (locally only)
        foreach (GameObject gameObject in playersGameObjects)
        {
            this.RemoveInstantiatedGO(gameObject, true);
        }

        // with ownership transfer, some objects might lose their owner.
        // in that case, the creator becomes the owner again. every client can apply this. done below.
        foreach (PhotonView view in this.photonViewList.Values)
        {
            if (view.ownerId == playerId)
            {
                view.ownerId = view.CreatorActorNr;
                //Debug.Log("Creator is: " + view.ownerId);
            }
        }
    }

    public void DestroyAll(bool localOnly)
    {
        if (!localOnly)
        {
            this.OpRemoveCompleteCache();
            this.SendDestroyOfAll();
        }

        this.LocalCleanupAnythingInstantiated(true);
    }

    /// <summary>Removes GameObject and the PhotonViews on it from local lists and optionally updates remotes. GameObject gets destroyed at end.</summary>
    /// <remarks>
    /// This method might fail and quit early due to several tests.
    /// </remarks>
    /// <param name="go">GameObject to cleanup.</param>
    /// <param name="localOnly">For localOnly, tests of control are skipped and the server is not updated.</param>
    protected internal void RemoveInstantiatedGO(GameObject go, bool localOnly)
    {
        if (go == null)
        {
            Debug.LogError("Failed to 'network-remove' GameObject because it's null.");
            return;
        }

        // Don't remove the GO if it doesn't have any PhotonView
        PhotonView[] views = go.GetComponentsInChildren<PhotonView>(true);
        if (views == null || views.Length <= 0)
        {
            Debug.LogError("Failed to 'network-remove' GameObject because has no PhotonView components: " + go);
            return;
        }

        PhotonView viewZero = views[0];
        int creatorId = viewZero.CreatorActorNr;            // creatorId of obj is needed to delete EvInstantiate (only if it's from that user)
        int instantiationId = viewZero.instantiationId;     // actual, live InstantiationIds start with 1 and go up

        // Don't remove GOs that are owned by others (unless this is the master and the remote player left)
        if (!localOnly)
        {
            if (!viewZero.isMine)
            {
                Debug.LogError("Failed to 'network-remove' GameObject. Client is neither owner nor masterClient taking over for owner who left: " + viewZero);
                return;
            }

            // Don't remove the Instantiation from the server, if it doesn't have a proper ID
            if (instantiationId < 1)
            {
                Debug.LogError("Failed to 'network-remove' GameObject because it is missing a valid InstantiationId on view: " + viewZero + ". Not Destroying GameObject or PhotonViews!");
                return;
            }
        }


        // cleanup instantiation (event and local list)
        if (!localOnly)
        {
            this.ServerCleanInstantiateAndDestroy(instantiationId, creatorId, viewZero.isRuntimeInstantiated);   // server cleaning
        }


        // cleanup PhotonViews and their RPCs events (if not localOnly)
        for (int j = views.Length - 1; j >= 0; j--)
        {
            PhotonView view = views[j];
            if (view == null)
            {
                continue;
            }

            // we only destroy/clean PhotonViews that were created by PhotonNetwork.Instantiate (and those have an instantiationId!)
            if (view.instantiationId >= 1)
            {
                this.LocalCleanPhotonView(view);
            }
            if (!localOnly)
            {
                this.OpCleanRpcBuffer(view);
            }
        }

        if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
            Debug.Log("Network destroy Instantiated GO: " + go.name);

        GameObject.Destroy(go);
    }

    /// <summary>
    /// This returns -1 if the GO could not be found in list of instantiatedObjects.
    /// </summary>
    public int GetInstantiatedObjectsId(GameObject go)
    {
        int id = -1;
        if (go == null)
        {
            Debug.LogError("GetInstantiatedObjectsId() for GO == null.");
            return id;
        }

        PhotonView[] pvs = go.GetPhotonViewsInChildren();
        if (pvs != null && pvs.Length > 0 && pvs[0] != null)
        {
            return pvs[0].instantiationId;
        }

        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            UnityEngine.Debug.Log("GetInstantiatedObjectsId failed for GO: " + go);


        return id;
    }

    /// <summary>
    /// Removes an instantiation event from the server's cache. Needs id and actorNr of player who instantiated.
    /// </summary>
    private void ServerCleanInstantiateAndDestroy(int instantiateId, int creatorId, bool isRuntimeInstantiated)
    {
        Hashtable removeFilter = new Hashtable();
        removeFilter[(byte)7] = instantiateId;

        RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, TargetActors = new int[] { creatorId } };
        this.OpRaiseEvent(PunEvent.Instantiation, removeFilter, true, options);
        //this.OpRaiseEvent(PunEvent.Instantiation, removeFilter, true, 0, new int[] { actorNr }, EventCaching.RemoveFromRoomCache);

        Hashtable evData = new Hashtable();
        evData[(byte)0] = instantiateId;
        options = null;
        if (!isRuntimeInstantiated)
        {
            // if the view got loaded with the scene, the EvDestroy must be cached (there is no Instantiate-msg which we can remove)
            // reason: joining players will load the obj and have to destroy it (too)
            options = new RaiseEventOptions();
            options.CachingOption = EventCaching.AddToRoomCacheGlobal;
            Debug.Log("Destroying GO as global. ID: " + instantiateId);
        }
        this.OpRaiseEvent(PunEvent.Destroy, evData, true, options);
    }

    private void SendDestroyOfPlayer(int actorNr)
    {
        Hashtable evData = new Hashtable();
        evData[(byte)0] = actorNr;

        this.OpRaiseEvent(PunEvent.DestroyPlayer, evData, true, null);
        //this.OpRaiseEvent(PunEvent.DestroyPlayer, evData, true, 0, EventCaching.DoNotCache, ReceiverGroup.Others);
    }

    private void SendDestroyOfAll()
    {
        Hashtable evData = new Hashtable();
        evData[(byte)0] = -1;


        this.OpRaiseEvent(PunEvent.DestroyPlayer, evData, true, null);
        //this.OpRaiseEvent(PunEvent.DestroyPlayer, evData, true, 0, EventCaching.DoNotCache, ReceiverGroup.Others);
    }

    private void OpRemoveFromServerInstantiationsOfPlayer(int actorNr)
    {
        // removes all "Instantiation" events of player actorNr. this is not an event for anyone else
        RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, TargetActors = new int[] { actorNr } };
        this.OpRaiseEvent(PunEvent.Instantiation, null, true, options);
        //this.OpRaiseEvent(PunEvent.Instantiation, null, true, 0, new int[] { actorNr }, EventCaching.RemoveFromRoomCache);
    }

    internal protected void RequestOwnership(int viewID, int fromOwner)
    {
        Debug.Log("RequestOwnership(): " + viewID + " from: " + fromOwner + " Time: " + Environment.TickCount % 1000);
        //PhotonNetwork.networkingPeer.OpRaiseEvent(PunEvent.OwnershipRequest, true, new int[] { viewID, fromOwner }, 0, EventCaching.DoNotCache, null, ReceiverGroup.All, 0);
        this.OpRaiseEvent(PunEvent.OwnershipRequest, new int[] {viewID, fromOwner}, true, new RaiseEventOptions() { Receivers = ReceiverGroup.All });   // All sends to all via server (including self)
    }

    internal protected void TransferOwnership(int viewID, int playerID)
    {
        Debug.Log("TransferOwnership() view " + viewID + " to: " + playerID + " Time: " + Environment.TickCount % 1000);
        //PhotonNetwork.networkingPeer.OpRaiseEvent(PunEvent.OwnershipTransfer, true, new int[] {viewID, playerID}, 0, EventCaching.DoNotCache, null, ReceiverGroup.All, 0);
        this.OpRaiseEvent(PunEvent.OwnershipTransfer, new int[] { viewID, playerID }, true, new RaiseEventOptions() { Receivers = ReceiverGroup.All });   // All sends to all via server (including self)
    }

    public void LocalCleanPhotonView(PhotonView view)
    {
        view.destroyedByPhotonNetworkOrQuit = true;
        this.photonViewList.Remove(view.viewID);
    }


    public PhotonView GetPhotonView(int viewID)
    {
        PhotonView result = null;
        this.photonViewList.TryGetValue(viewID, out result);

        if (result == null)
        {
            PhotonView[] views = GameObject.FindObjectsOfType(typeof(PhotonView)) as PhotonView[];

            foreach (PhotonView view in views)
            {
                if (view.viewID == viewID)
                {
                    if (view.didAwake)
                    {
                        Debug.LogWarning("Had to lookup view that wasn't in photonViewList: " + view);
                    }
                    return view;
                }
            }
        }

        return result;
    }

    public void RegisterPhotonView(PhotonView netView)
    {
        if (!Application.isPlaying)
        {
            this.photonViewList = new Dictionary<int, PhotonView>();
            return;
        }

        if (netView.viewID == 0)
        {
            // don't register views with ID 0 (not initialized). they register when a ID is assigned later on
            Debug.Log("PhotonView register is ignored, because viewID is 0. No id assigned yet to: " + netView);
            return;
        }

        if (this.photonViewList.ContainsKey(netView.viewID))
        {
            // if some other view is in the list already, we got a problem. it might be undestructible. print out error
            if (netView != photonViewList[netView.viewID])
            {
                Debug.LogError(string.Format("PhotonView ID duplicate found: {0}. New: {1} old: {2}. Maybe one wasn't destroyed on scene load?! Check for 'DontDestroyOnLoad'. Destroying old entry, adding new.", netView.viewID, netView, photonViewList[netView.viewID]));
            }

            //this.photonViewList.Remove(netView.viewID); // TODO check if we chould Destroy the GO of this view?!
            this.RemoveInstantiatedGO(photonViewList[netView.viewID].gameObject, true);
        }

        // Debug.Log("adding view to known list: " + netView);
        this.photonViewList.Add(netView.viewID, netView);
        //Debug.LogError("view being added. " + netView);	// Exit Games internal log

        if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
            Debug.Log("Registered PhotonView: " + netView.viewID);
    }

    ///// <summary>
    ///// Will remove the view from list of views (by its ID).
    ///// </summary>
    //public void RemovePhotonView(PhotonView netView)
    //{
    //    if (!Application.isPlaying)
    //    {
    //        this.photonViewList = new Dictionary<int, PhotonView>();
    //        return;
    //    }

    //    //PhotonView removedView = null;
    //    //this.photonViewList.TryGetValue(netView.viewID, out removedView);
    //    //if (removedView != netView)
    //    //{
    //    //    Debug.LogError("Detected two differing PhotonViews with same viewID: " + netView.viewID);
    //    //}

    //    this.photonViewList.Remove(netView.viewID);

    //    //if (this.DebugOut >= DebugLevel.ALL)
    //    //{
    //    //    this.DebugReturn(DebugLevel.ALL, "Removed PhotonView: " + netView.viewID);
    //    //}
    //}

    /// <summary>
    /// Removes the RPCs of someone else (to be used as master).
    /// This won't clean any local caches. It just tells the server to forget a player's RPCs and instantiates.
    /// </summary>
    /// <param name="actorNumber"></param>
    public void OpCleanRpcBuffer(int actorNumber)
    {
        RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, TargetActors = new int[] { actorNumber } };
        this.OpRaiseEvent(PunEvent.RPC, null, true, options);
        //this.OpRaiseEvent(PunEvent.RPC, null, true, 0, new int[] { actorNumber }, EventCaching.RemoveFromRoomCache);
    }

    /// <summary>
    /// Instead removing RPCs or Instantiates, this removed everything cached by the actor.
    /// </summary>
    /// <param name="actorNumber"></param>
    public void OpRemoveCompleteCacheOfPlayer(int actorNumber)
    {
        RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, TargetActors = new int[] { actorNumber } };
        this.OpRaiseEvent(0, null, true, options);
        //this.OpRaiseEvent(0, null, true, 0, new int[] { actorNumber }, EventCaching.RemoveFromRoomCache);
    }


    public void OpRemoveCompleteCache()
    {
        RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, Receivers = ReceiverGroup.MasterClient };
        this.OpRaiseEvent(0, null, true, options);
        //this.OpRaiseEvent(0, null, true, 0, EventCaching.RemoveFromRoomCache, ReceiverGroup.MasterClient);  // TODO: check who gets this event?
    }

    /// This clears the cache of any player/actor who's no longer in the room (making it a simple clean-up option for a new master)
    private void RemoveCacheOfLeftPlayers()
    {
        Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
        opParameters[ParameterCode.Code] = (byte)0;		// any event
        opParameters[ParameterCode.Cache] = (byte)EventCaching.RemoveFromRoomCacheForActorsLeft;    // option to clear the room cache of all events of players who left

        this.OpCustom((byte)OperationCode.RaiseEvent, opParameters, true, 0);
    }

    // Remove RPCs of view (if they are local player's RPCs)
    public void CleanRpcBufferIfMine(PhotonView view)
    {
        if (view.ownerId != this.mLocalActor.ID && !mLocalActor.isMasterClient)
        {
            Debug.LogError("Cannot remove cached RPCs on a PhotonView thats not ours! " + view.owner + " scene: " + view.isSceneView);
            return;
        }

        this.OpCleanRpcBuffer(view);
    }

    /// <summary>Cleans server RPCs for PhotonView (without any further checks).</summary>
    public void OpCleanRpcBuffer(PhotonView view)
    {
        Hashtable rpcFilterByViewId = new Hashtable();
        rpcFilterByViewId[(byte)0] = view.viewID;

        RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache };
        this.OpRaiseEvent(PunEvent.RPC, rpcFilterByViewId, true, options);
        //this.OpRaiseEvent(PunEvent.RPC, rpcFilterByViewId, true, 0, EventCaching.RemoveFromRoomCache, ReceiverGroup.Others);
    }

    public void RemoveRPCsInGroup(int group)
    {
        foreach (KeyValuePair<int, PhotonView> kvp in this.photonViewList)
        {
            PhotonView view = kvp.Value;
            if (view.group == group)
            {
                this.CleanRpcBufferIfMine(view);
            }
        }
    }

    public void SetLevelPrefix(short prefix)
    {
        this.currentLevelPrefix = prefix;
        // TODO: should we really change the prefix for existing PVs?! better keep it!
        //foreach (PhotonView view in this.photonViewList.Values)
        //{
        //    view.prefix = prefix;
        //}
    }

    internal void RPC(PhotonView view, string methodName, PhotonPlayer player, bool encrypt, params object[] parameters)
    {
        if (this.blockSendingGroups.Contains(view.group))
        {
            return; // Block sending on this group
        }

        if (view.viewID < 1)    //TODO: check why 0 should be illegal
        {
            Debug.LogError("Illegal view ID:" + view.viewID + " method: " + methodName + " GO:" + view.gameObject.name);
        }

        if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
        {
            Debug.Log("Sending RPC \"" + methodName + "\" to player[" + player + "]");
        }


        //ts: changed RPCs to a one-level hashtable as described in internal.txt
        Hashtable rpcEvent = new Hashtable();
        rpcEvent[(byte)0] = (int)view.viewID; // LIMITS PHOTONVIEWS&PLAYERS
        if (view.prefix > 0)
        {
            rpcEvent[(byte)1] = (short)view.prefix;
        }
        rpcEvent[(byte)2] = this.ServerTimeInMilliSeconds;

        // send name or shortcut (if available)
        int shortcut = 0;
        if (rpcShortcuts.TryGetValue(methodName, out shortcut))
        {
            rpcEvent[(byte)5] = (byte)shortcut; // LIMITS RPC COUNT
        }
        else
        {
            rpcEvent[(byte)3] = methodName;
        }

        if (parameters != null && parameters.Length > 0)
        {
            rpcEvent[(byte) 4] = (object[]) parameters;
        }

        if (this.mLocalActor == player)
        {
            this.ExecuteRPC(rpcEvent, player);
        }
        else
        {
            RaiseEventOptions options = new RaiseEventOptions() { TargetActors = new int[] { player.ID }, Encrypt = encrypt };
            this.OpRaiseEvent(PunEvent.RPC, rpcEvent, true, options);
        }
    }

    /// RPC Hashtable Structure
    /// (byte)0 -> (int) ViewId (combined from actorNr and actor-unique-id)
    /// (byte)1 -> (short) prefix (level)
    /// (byte)2 -> (int) server timestamp
    /// (byte)3 -> (string) methodname
    /// (byte)4 -> (object[]) parameters
    /// (byte)5 -> (byte) method shortcut (alternative to name)
    ///
    /// This is sent as event (code: 200) which will contain a sender (origin of this RPC).

    internal void RPC(PhotonView view, string methodName, PhotonTargets target, bool encrypt, params object[] parameters)
    {
        if (this.blockSendingGroups.Contains(view.group))
        {
            return; // Block sending on this group
        }

        if (view.viewID < 1)
        {
            Debug.LogError("Illegal view ID:" + view.viewID + " method: " + methodName + " GO:" + view.gameObject.name);
        }

        if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
            Debug.Log("Sending RPC \"" + methodName + "\" to " + target);


        //ts: changed RPCs to a one-level hashtable as described in internal.txt
        Hashtable rpcEvent = new Hashtable();
        rpcEvent[(byte)0] = (int)view.viewID; // LIMITS NETWORKVIEWS&PLAYERS
        if (view.prefix > 0)
        {
            rpcEvent[(byte)1] = (short)view.prefix;
        }
        rpcEvent[(byte)2] = this.ServerTimeInMilliSeconds;


        // send name or shortcut (if available)
        int shortcut = 0;
        if (rpcShortcuts.TryGetValue(methodName, out shortcut))
        {
            rpcEvent[(byte)5] = (byte)shortcut; // LIMITS RPC COUNT
        }
        else
        {
            rpcEvent[(byte)3] = methodName;
        }

        if (parameters != null && parameters.Length > 0)
        {
            rpcEvent[(byte)4] = (object[])parameters;
        }

        // Check scoping
        if (target == PhotonTargets.All)
        {
            RaiseEventOptions options = new RaiseEventOptions() { InterestGroup = (byte)view.group, Encrypt = encrypt };
            this.OpRaiseEvent(PunEvent.RPC, rpcEvent, true, options);

            // Execute local
            this.ExecuteRPC(rpcEvent, this.mLocalActor);
        }
        else if (target == PhotonTargets.Others)
        {
            RaiseEventOptions options = new RaiseEventOptions() { InterestGroup = (byte)view.group, Encrypt = encrypt };
            this.OpRaiseEvent(PunEvent.RPC, rpcEvent, true, options);
        }
        else if (target == PhotonTargets.AllBuffered)
        {
            RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.AddToRoomCache, Encrypt = encrypt };
            this.OpRaiseEvent(PunEvent.RPC, rpcEvent, true, options);

            // Execute local
            this.ExecuteRPC(rpcEvent, this.mLocalActor);
        }
        else if (target == PhotonTargets.OthersBuffered)
        {
            RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.AddToRoomCache, Encrypt = encrypt };
            this.OpRaiseEvent(PunEvent.RPC, rpcEvent, true, options);
        }
        else if (target == PhotonTargets.MasterClient)
        {
            if (this.mMasterClient == this.mLocalActor)
            {
                this.ExecuteRPC(rpcEvent, this.mLocalActor);
            }
            else
            {
                RaiseEventOptions options = new RaiseEventOptions() { Receivers = ReceiverGroup.MasterClient, Encrypt = encrypt };
                this.OpRaiseEvent(PunEvent.RPC, rpcEvent, true, options);
            }
        }
        else if (target == PhotonTargets.AllViaServer)
        {
            RaiseEventOptions options = new RaiseEventOptions() { InterestGroup = (byte)view.group, Receivers = ReceiverGroup.All, Encrypt = encrypt };
            this.OpRaiseEvent(PunEvent.RPC, rpcEvent, true, options);
            if (PhotonNetwork.offlineMode)
            {
                this.ExecuteRPC(rpcEvent, this.mLocalActor);
            }
        }
        else if (target == PhotonTargets.AllBufferedViaServer)
        {
            RaiseEventOptions options = new RaiseEventOptions() { InterestGroup = (byte)view.group, Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCache, Encrypt = encrypt };
            this.OpRaiseEvent(PunEvent.RPC, rpcEvent, true, options);
            if (PhotonNetwork.offlineMode)
            {
                this.ExecuteRPC(rpcEvent, this.mLocalActor);
            }
        }
        else
        {
            Debug.LogError("Unsupported target enum: " + target);
        }
    }

    // SetReceiving
    public void SetReceivingEnabled(int group, bool enabled)
    {
        if (group <= 0)
        {
            Debug.LogError("Error: PhotonNetwork.SetReceivingEnabled was called with an illegal group number: " + group + ". The group number should be at least 1.");
            return;
        }

        if (enabled)
        {
            if (!this.allowedReceivingGroups.Contains(group))
            {
                this.allowedReceivingGroups.Add(group);
                byte[] groups = new byte[1] { (byte)group };
                this.OpChangeGroups(null, groups);
            }
        }
        else
        {
            if (this.allowedReceivingGroups.Contains(group))
            {
                this.allowedReceivingGroups.Remove(group);
                byte[] groups = new byte[1] { (byte)group };
                this.OpChangeGroups(groups, null);
            }
        }
    }


    public void SetReceivingEnabled(int[] enableGroups, int[] disableGroups)
    {
        List<byte> enableList = new List<byte>();
        List<byte> disableList = new List<byte>();

        if (enableGroups != null)
        {
            for (int index = 0; index < enableGroups.Length; index++)
            {
                int i = enableGroups[index];
                if (i <= 0)
                {
                    Debug.LogError("Error: PhotonNetwork.SetReceivingEnabled was called with an illegal group number: " + i + ". The group number should be at least 1.");
                    continue;
                }
                if (!this.allowedReceivingGroups.Contains(i))
                {
                    this.allowedReceivingGroups.Add(i);
                    enableList.Add((byte)i);
                }
            }
        }
        if (disableGroups != null)
        {
            for (int index = 0; index < disableGroups.Length; index++)
            {
                int i = disableGroups[index];
                if (i <= 0)
                {
                    Debug.LogError("Error: PhotonNetwork.SetReceivingEnabled was called with an illegal group number: " + i + ". The group number should be at least 1.");
                    continue;
                }
                if (enableList.Contains((byte)i))
                {
                    Debug.LogError("Error: PhotonNetwork.SetReceivingEnabled disableGroups contains a group that is also in the enableGroups: " + i + ".");
                    continue;
                }
                if (this.allowedReceivingGroups.Contains(i))
                {
                    this.allowedReceivingGroups.Remove(i);
                    disableList.Add((byte)i);
                }
            }
        }

        this.OpChangeGroups(disableList.Count > 0 ? disableList.ToArray() : null, enableList.Count > 0 ? enableList.ToArray() : null); //Passing a 0 sized array != passing null
    }

    // SetSending
    public void SetSendingEnabled(int group, bool enabled)
    {
        if (!enabled)
        {
            this.blockSendingGroups.Add(group); // can be added to HashSet no matter if already in it
        }
        else
        {
            this.blockSendingGroups.Remove(group);
        }
    }


    public void SetSendingEnabled(int[] enableGroups, int[] disableGroups)
    {
        if(enableGroups!=null){
            foreach(int i in enableGroups){
                if(this.blockSendingGroups.Contains(i))
                    this.blockSendingGroups.Remove(i);
            }
        }
        if(disableGroups!=null){
            foreach(int i in disableGroups){
                if(!this.blockSendingGroups.Contains(i))
                    this.blockSendingGroups.Add(i);
            }
        }
    }


    public void NewSceneLoaded()
    {
        if (this.loadingLevelAndPausedNetwork)
        {
            this.loadingLevelAndPausedNetwork = false;
            PhotonNetwork.isMessageQueueRunning = true;
        }
        // Debug.Log("OnLevelWasLoaded photonViewList.Count: " + photonViewList.Count); // Exit Games internal log

        List<int> removeKeys = new List<int>();
        foreach (KeyValuePair<int, PhotonView> kvp in this.photonViewList)
        {
            PhotonView view = kvp.Value;
            if (view == null)
            {
                removeKeys.Add(kvp.Key);
            }
        }

        for (int index = 0; index < removeKeys.Count; index++)
        {
            int key = removeKeys[index];
            this.photonViewList.Remove(key);
        }

        if (removeKeys.Count > 0)
        {
            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                Debug.Log("New level loaded. Removed " + removeKeys.Count + " scene view IDs from last level.");
        }
    }


    // this is called by Update() and in Unity that means it's single threaded.
    public void RunViewUpdate()
    {
        if (!PhotonNetwork.connected || PhotonNetwork.offlineMode)
        {
            return;
        }

        if (this.mActors == null ||
#if !PHOTON_DEVELOP
            this.mActors.Count <= 1
#endif
            )
        {
            return; // No need to send OnSerialize messages (these are never buffered anyway)
        }

        dataPerGroupReliable.Clear();
        dataPerGroupUnreliable.Clear();

        /* Format of the data hashtable:
         * Hasthable dataPergroup*
         *  [(byte)0] = this.ServerTimeInMilliSeconds;
         *  OPTIONAL: [(byte)1] = currentLevelPrefix;
         *  +  data
         */

        foreach (KeyValuePair<int, PhotonView> kvp in this.photonViewList)
        {
            PhotonView view = kvp.Value;

            if (view.synchronization != ViewSynchronization.Off)
            {
                // Fetch all sending photonViews
                if (view.isMine)
                {
                    #if UNITY_2_6_1 || UNITY_2_6 || UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
                    if (!view.gameObject.active)
                    {
                        continue; // Only on actives
                    }
                    #else
                    if (!view.gameObject.activeInHierarchy)
                    {
                        continue; // Only on actives
                    }
                    #endif

                    if (this.blockSendingGroups.Contains(view.group))
                    {
                        continue; // Block sending on this group
                    }

                    // Run it trough its OnSerialize
                    Hashtable evData = this.OnSerializeWrite(view);
                    if (evData == null)
                    {
                        continue;
                    }

                    if (view.synchronization == ViewSynchronization.ReliableDeltaCompressed || view.mixedModeIsReliable)
                    {
                        if (!evData.ContainsKey((byte)1) && !evData.ContainsKey((byte)2))
                        {
                            // Everything has been removed by compression, nothing to send
                        }
                        else
                        {
                            if (!dataPerGroupReliable.ContainsKey(view.group))
                            {
                                dataPerGroupReliable[view.group] = new Hashtable();
                                dataPerGroupReliable[view.group][(byte)0] = this.ServerTimeInMilliSeconds;
                                if (currentLevelPrefix >= 0)
                                {
                                    dataPerGroupReliable[view.group][(byte)1] = this.currentLevelPrefix;
                                }
                            }
                            Hashtable groupHashtable = dataPerGroupReliable[view.group];
                            groupHashtable.Add((short)groupHashtable.Count, evData);
                        }
                    }
                    else
                    {
                        if (!dataPerGroupUnreliable.ContainsKey(view.group))
                        {
                            dataPerGroupUnreliable[view.group] = new Hashtable();
                            dataPerGroupUnreliable[view.group][(byte)0] = this.ServerTimeInMilliSeconds;
                            if (currentLevelPrefix >= 0)
                            {
                                dataPerGroupUnreliable[view.group][(byte)1] = this.currentLevelPrefix;
                            }
                        }
                        Hashtable groupHashtable = dataPerGroupUnreliable[view.group];
                        groupHashtable.Add((short)groupHashtable.Count, evData);
                    }
                }
                else
                {
                    // Debug.Log(" NO OBS on " + view.name + " " + view.owner);
                }
            }
            else
            {
            }
        }

        //Send the messages: every group is send in it's own message and unreliable and reliable are split as well
        RaiseEventOptions options = new RaiseEventOptions();

#if PHOTON_DEVELOP
        options.Receivers = ReceiverGroup.All;
#endif

        foreach (KeyValuePair<int, Hashtable> kvp in dataPerGroupReliable)
        {
            options.InterestGroup = (byte)kvp.Key;
            this.OpRaiseEvent(PunEvent.SendSerializeReliable, kvp.Value, true, options);
        }
        foreach (KeyValuePair<int, Hashtable> kvp in dataPerGroupUnreliable)
        {
            options.InterestGroup = (byte)kvp.Key;
            this.OpRaiseEvent(PunEvent.SendSerialize, kvp.Value, false, options);
        }
    }

    // calls OnPhotonSerializeView (through ExecuteOnSerialize)
    // the content created here is consumed by receivers in: ReadOnSerialize
    private Hashtable OnSerializeWrite(PhotonView view)
    {
        PhotonStream pStream = new PhotonStream( true, null );
        PhotonMessageInfo info = new PhotonMessageInfo( this.mLocalActor, this.ServerTimeInMilliSeconds, view );

        // each view creates a list of values that should be sent
        view.SerializeView( pStream, info );

        if( pStream.Count == 0 )
        {
            return null;
        }

        object[] dataArray = pStream.data.ToArray();

        if (view.synchronization == ViewSynchronization.UnreliableOnChange)
        {
            if (AlmostEquals(dataArray, view.lastOnSerializeDataSent))
            {
                if (view.mixedModeIsReliable)
                {
                    return null;
                }

                view.mixedModeIsReliable = true;
                view.lastOnSerializeDataSent = dataArray;
            }
            else
            {
                view.mixedModeIsReliable = false;
                view.lastOnSerializeDataSent = dataArray;
            }
        }

        // EVDATA:
        // 0=View ID (an int, never compressed cause it's not in the data)
        // 1=data of observed type (different per type of observed object)
        // 2=compressed data (in this case, key 1 is empty)
        // 3=list of values that are actually null (if something was changed but actually IS null)
        Hashtable evData = new Hashtable();
        evData[(byte)0] = (int)view.viewID;
        evData[(byte)1] = dataArray;    // this is the actual data (script or observed object)


        if (view.synchronization == ViewSynchronization.ReliableDeltaCompressed)
        {
            // compress content of data set (by comparing to view.lastOnSerializeDataSent)
            // the "original" dataArray is NOT modified by DeltaCompressionWrite
            // if something was compressed, the evData key 2 and 3 are used (see above)
            bool somethingLeftToSend = this.DeltaCompressionWrite(view, evData);

            // buffer the full data set (for next compression)
            view.lastOnSerializeDataSent = dataArray;

            if (!somethingLeftToSend)
            {
                return null;
            }
        }

        return evData;
    }

    /// <summary>
    /// Reads updates created by OnSerializeWrite
    /// </summary>
    private void OnSerializeRead(Hashtable data, PhotonPlayer sender, int networkTime, short correctPrefix)
    {
        // read view ID from key (byte)0: a int-array (PUN 1.17++)
        int viewID = (int)data[(byte)0];


        PhotonView view = this.GetPhotonView(viewID);
        if (view == null)
        {
            Debug.LogWarning("Received OnSerialization for view ID " + viewID + ". We have no such PhotonView! Ignored this if you're leaving a room. State: " + this.State);
            return;
        }

        if (view.prefix > 0 && correctPrefix != view.prefix)
        {
            Debug.LogError("Received OnSerialization for view ID " + viewID + " with prefix " + correctPrefix + ". Our prefix is " + view.prefix);
            return;
        }

        // SetReceiving filtering
        if (view.group != 0 && !this.allowedReceivingGroups.Contains(view.group))
        {
            return; // Ignore group
        }


        if (view.synchronization == ViewSynchronization.ReliableDeltaCompressed)
        {
            if (!this.DeltaCompressionRead(view, data))
            {
                // Skip this packet as we haven't got received complete-copy of this view yet.
                if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                    Debug.Log("Skipping packet for " + view.name + " [" + view.viewID + "] as we haven't received a full packet for delta compression yet. This is OK if it happens for the first few frames after joining a game.");
                return;
            }

            // store last received for delta-compression usage
            view.lastOnSerializeDataReceived = data[(byte)1] as object[];
        }

        if (sender.ID != view.ownerId)
        {
            if (!view.isSceneView || !sender.isMasterClient)
            {
                // obviously the owner changed and we didn't yet notice.
                Debug.Log("Adjusting owner to sender of updates. From: " + view.ownerId + " to: " + sender.ID);
                view.ownerId = sender.ID;
            }
        }

        object[] contents = data[(byte)1] as object[];
        PhotonStream pStream = new PhotonStream(false, contents);
        PhotonMessageInfo info = new PhotonMessageInfo(sender, networkTime, view);

        view.DeserializeView( pStream, info );
    }

    private bool AlmostEquals(object[] lastData, object[] currentContent)
    {
        if (lastData == null && currentContent == null)
        {
            return true;
        }

        if (lastData == null || currentContent == null || (lastData.Length != currentContent.Length))
        {
            return false;
        }

        for (int index = 0; index < currentContent.Length; index++)
        {
            object newObj = currentContent[index];
            object oldObj = lastData[index];
            if (!this.ObjectIsSameWithInprecision(newObj, oldObj))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Compares the new data with previously sent data and skips values that didn't change.
    /// </summary>
    /// <returns>True if anything has to be sent, false if nothing new or no data</returns>
    private bool DeltaCompressionWrite(PhotonView view, Hashtable data)
    {
        if (view.lastOnSerializeDataSent == null)
        {
            return true; // all has to be sent
        }

        // We can compress as we sent a full update previously (readers can re-use previous values)
        object[] lastData = view.lastOnSerializeDataSent;
        object[] currentContent = data[(byte)1] as object[];

        if (currentContent == null)
        {
            // no data to be sent
            return false;
        }

        if (lastData.Length != currentContent.Length)
        {
            // if new data isn't same length as before, we send the complete data-set uncompressed
            return true;
        }

        object[] compressedContent = new object[currentContent.Length];
        int compressedValues = 0;

        List<int> valuesThatAreChangedToNull = new List<int>();
        for (int index = 0; index < compressedContent.Length; index++)
        {
            object newObj = currentContent[index];
            object oldObj = lastData[index];
            if (this.ObjectIsSameWithInprecision(newObj, oldObj))
            {
                // compress (by using null, instead of value, which is same as before)
                compressedValues++;
                // compressedContent[index] is already null (initialized)
            }
            else
            {
                compressedContent[index] = currentContent[index];

                // value changed, we don't replace it with null
                // new value is null (like a compressed value): we have to mark it so it STAYS null instead of being replaced with previous value
                if (newObj == null)
                {
                    valuesThatAreChangedToNull.Add(index);
                }
            }
        }

        // Only send the list of compressed fields if we actually compressed 1 or more fields.
        if (compressedValues > 0)
        {
            data.Remove((byte)1); // remove the original data (we only send compressed data)

            if (compressedValues == currentContent.Length)
            {
                // all values are compressed to null, we have nothing to send
                return false;
            }

            data[(byte)2] = compressedContent; // current, compressted data is moved to key 2 to mark it as compressed
            if (valuesThatAreChangedToNull.Count > 0)
            {
                data[(byte)3] = valuesThatAreChangedToNull.ToArray(); // data that is actually null (not just cause we didn't want to send it)
            }
        }

        return true;    // some data was compressed but we need to send something
    }

    /// <summary>
    /// reads incoming messages created by "OnSerialize"
    /// </summary>
    private bool DeltaCompressionRead(PhotonView view, Hashtable data)
    {
        if (data.ContainsKey((byte)1))
        {
            // we have a full list of data (cause key 1 is used), so return "we have uncompressed all"
            return true;
        }

        // Compression was applied as data[(byte)2] exists (this is the data with some fields being compressed to null)
        // now we also need a previous "full" list of values to restore values that are null in this msg
        if (view.lastOnSerializeDataReceived == null)
        {
            return false; // We dont have a full match yet, we cannot work with missing values: skip this message
        }

        object[] compressedContents = data[(byte)2] as object[];
        if (compressedContents == null)
        {
            // despite expectation, there is no compressed data in this msg. shouldn't happen. just a null check
            return false;
        }

        int[] indexesThatAreChangedToNull = data[(byte)3] as int[];
        if (indexesThatAreChangedToNull == null)
        {
            indexesThatAreChangedToNull = new int[0];
        }

        object[] lastReceivedData = view.lastOnSerializeDataReceived;
        for (int index = 0; index < compressedContents.Length; index++)
        {
            if (compressedContents[index] == null && !indexesThatAreChangedToNull.Contains(index))
            {
                // we replace null values in this received msg unless a index is in the "changed to null" list
                object lastValue = lastReceivedData[index];
                compressedContents[index] = lastValue;
            }
        }

        data[(byte)1] = compressedContents; // compressedContents are now uncompressed...
        return true;
    }

    /// <summary>
    /// Returns true if both objects are almost identical.
    /// Used to check whether two objects are similar enough to skip an update.
    /// </summary>
    bool ObjectIsSameWithInprecision(object one, object two)
    {
        if (one == null || two == null)
        {
            return one == null && two == null;
        }

        if (!one.Equals(two))
        {
            // if A is not B, lets check if A is almost B
            if (one is Vector3)
            {
                Vector3 a = (Vector3)one;
                Vector3 b = (Vector3)two;
                if (a.AlmostEquals(b, PhotonNetwork.precisionForVectorSynchronization))
                {
                    return true;
                }
            }
            else if (one is Vector2)
            {
                Vector2 a = (Vector2)one;
                Vector2 b = (Vector2)two;
                if (a.AlmostEquals(b, PhotonNetwork.precisionForVectorSynchronization))
                {
                    return true;
                }
            }
            else if (one is Quaternion)
            {
                Quaternion a = (Quaternion)one;
                Quaternion b = (Quaternion)two;
                if (a.AlmostEquals(b, PhotonNetwork.precisionForQuaternionSynchronization))
                {
                    return true;
                }
            }
            else if (one is float)
            {
                float a = (float)one;
                float b = (float)two;
                if (a.AlmostEquals(b, PhotonNetwork.precisionForFloatSynchronization))
                {
                    return true;
                }
            }

            // one does not equal two
            return false;
        }

        return true;
    }

    internal protected static bool GetMethod(MonoBehaviour monob, string methodType, out MethodInfo mi)
    {
        mi = null;

        if (monob == null || string.IsNullOrEmpty(methodType))
        {
            return false;
        }

        List<MethodInfo> methods = SupportClass.GetMethods(monob.GetType(), null);
        for (int index = 0; index < methods.Count; index++)
        {
            MethodInfo methodInfo = methods[index];
            if (methodInfo.Name.Equals(methodType))
            {
                mi = methodInfo;
                return true;
            }
        }

        return false;
    }

    /// <summary>Internally used to flag if the message queue was disabled by a "scene sync" situation (to re-enable it).</summary>
    internal protected bool loadingLevelAndPausedNetwork = false;

    /// <summary>For automatic scene syncing, the loaded scene is put into a room property. This is the name of said prop.</summary>
    internal protected const string CurrentSceneProperty = "curScn";

    /// <summary>Internally used to detect the current scene and load it if PhotonNetwork.automaticallySyncScene is enabled.</summary>
    internal protected void LoadLevelIfSynced()
    {
        if (!PhotonNetwork.automaticallySyncScene || PhotonNetwork.isMasterClient || PhotonNetwork.room == null)
        {
            return;
        }

        // check if "current level" is set in props
        if (!PhotonNetwork.room.customProperties.ContainsKey(NetworkingPeer.CurrentSceneProperty))
        {
            return;
        }

        // if loaded level is not the one defined my master in props, load that level
        object sceneId = PhotonNetwork.room.customProperties[NetworkingPeer.CurrentSceneProperty];
        if (sceneId is int)
        {
            if (Application.loadedLevel != (int)sceneId)
                PhotonNetwork.LoadLevel((int)sceneId);
        }
        else if (sceneId is string)
        {
            if (Application.loadedLevelName != (string)sceneId)
                PhotonNetwork.LoadLevel((string)sceneId);
        }
    }

    protected internal void SetLevelInPropsIfSynced(object levelId)
    {
        if (!PhotonNetwork.automaticallySyncScene || !PhotonNetwork.isMasterClient || PhotonNetwork.room == null)
        {
            return;
        }
        if (levelId == null)
        {
            Debug.LogError("Parameter levelId can't be null!");
            return;
        }

        // check if "current level" is already set in props
        if (PhotonNetwork.room.customProperties.ContainsKey(NetworkingPeer.CurrentSceneProperty))
        {
            object levelIdInProps = PhotonNetwork.room.customProperties[NetworkingPeer.CurrentSceneProperty];
            if (levelIdInProps is int && Application.loadedLevel == (int)levelIdInProps)
            {
                return;
            }
            if (levelIdInProps is string && Application.loadedLevelName.Equals((string)levelIdInProps))
            {
                return;
            }
        }

        // current level is not yet in props, so this client has to set it
        Hashtable setScene = new Hashtable();
        if (levelId is int) setScene[NetworkingPeer.CurrentSceneProperty] = (int)levelId;
        else if (levelId is string) setScene[NetworkingPeer.CurrentSceneProperty] = (string)levelId;
        else Debug.LogError("Parameter levelId must be int or string!");

        PhotonNetwork.room.SetCustomProperties(setScene);
        this.SendOutgoingCommands();    // send immediately! because: in most cases the client will begin to load and not send for a while
    }

    public void SetApp(string appId, string gameVersion)
    {
        this.mAppId = appId.Trim();

        if (!string.IsNullOrEmpty(gameVersion))
        {
            this.mAppVersion = gameVersion.Trim();
        }
    }


    public bool WebRpc(string uriPath, object parameters)
    {
        Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
        opParameters.Add(ParameterCode.UriPath, uriPath);
        opParameters.Add(ParameterCode.WebRpcParameters, parameters);

        return this.OpCustom(OperationCode.WebRpc, opParameters, true);

    }
}

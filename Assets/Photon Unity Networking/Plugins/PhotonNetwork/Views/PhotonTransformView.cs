﻿using UnityEngine;
using System.Collections;

/// <summary>
/// This class helps you to synchronize position, rotation and scale
/// of a GameObject. It also gives you many different options to make
/// the synchronized values appear smooth, even when the data is only
/// send a couple of times per second.
/// Simply add the component to your GameObject and make sure that
/// the PhotonTransformView is added to the list of observed components
/// </summary>
[RequireComponent( typeof( PhotonView ) )]
[AddComponentMenu("Photon Networking/Photon Transform View")]
public class PhotonTransformView : MonoBehaviour, IPunObservable
{
    //Since this component is very complex, we seperated it into multiple objects.
    //The PositionModel, RotationModel and ScaleMode store the data you are able to
    //configure in the inspector while the control objects below are actually moving
    //the object and calculating all the inter- and extrapolation

    [SerializeField]
    PhotonTransformViewPositionModel m_PositionModel = new PhotonTransformViewPositionModel();

    [SerializeField]
    PhotonTransformViewRotationModel m_RotationModel = new PhotonTransformViewRotationModel();

    [SerializeField]
    PhotonTransformViewScaleModel m_ScaleModel = new PhotonTransformViewScaleModel();

    PhotonTransformViewPositionControl m_PositionControl;
    PhotonTransformViewRotationControl m_RotationControl;
    PhotonTransformViewScaleControl m_ScaleControl;

    PhotonView m_PhotonView;

    bool m_ReceivedNetworkUpdate = false;

    void Awake()
    {
        this.m_PhotonView = this.GetComponent<PhotonView>();

        this.m_PositionControl = new PhotonTransformViewPositionControl(this.m_PositionModel );
        this.m_RotationControl = new PhotonTransformViewRotationControl(this.m_RotationModel );
        this.m_ScaleControl = new PhotonTransformViewScaleControl(this.m_ScaleModel );
    }

    void Update()
    {
        if(this.m_PhotonView == null || this.m_PhotonView.isMine == true || PhotonNetwork.connected == false )
        {
            return;
        }

        this.UpdatePosition();
        this.UpdateRotation();
        this.UpdateScale();
    }

    void UpdatePosition()
    {
        if(this.m_PositionModel.SynchronizeEnabled == false || this.m_ReceivedNetworkUpdate == false )
        {
            return;
        }

        this.transform.localPosition = this.m_PositionControl.UpdatePosition(this.transform.localPosition );
    }

    void UpdateRotation()
    {
        if(this.m_RotationModel.SynchronizeEnabled == false || this.m_ReceivedNetworkUpdate == false )
        {
            return;
        }

        this.transform.localRotation = this.m_RotationControl.GetRotation(this.transform.localRotation );
    }

    void UpdateScale()
    {
        if(this.m_ScaleModel.SynchronizeEnabled == false || this.m_ReceivedNetworkUpdate == false )
        {
            return;
        }

        this.transform.localScale = this.m_ScaleControl.GetScale(this.transform.localScale );
    }

    /// <summary>
    /// These values are synchronized to the remote objects if the interpolation mode
    /// or the extrapolation mode SynchronizeValues is used. Your movement script should pass on
    /// the current speed (in units/second) and turning speed (in angles/second) so the remote
    /// object can use them to predict the objects movement.
    /// </summary>
    /// <param name="speed">The current movement vector of the object in units/second.</param>
    /// <param name="turnSpeed">The current turn speed of the object in angles/second.</param>
    public void SetSynchronizedValues( Vector3 speed, float turnSpeed )
    {
        this.m_PositionControl.SetSynchronizedValues( speed, turnSpeed );
    }

    public void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info )
    {
        this.m_PositionControl.OnPhotonSerializeView(this.transform.localPosition, stream, info );
        this.m_RotationControl.OnPhotonSerializeView(this.transform.localRotation, stream, info );
        this.m_ScaleControl.OnPhotonSerializeView(this.transform.localScale, stream, info );

        if(this.m_PhotonView.isMine == false && this.m_PositionModel.DrawErrorGizmo == true )
        {
            this.DoDrawEstimatedPositionError();
        }

        if( stream.isReading == true )
        {
            this.m_ReceivedNetworkUpdate = true;
        }
    }

    //void OnDrawGizmos()
    //{
    //    if( Application.isPlaying == false || m_PhotonView == null || m_PhotonView.isMine == true || PhotonNetwork.connected == false )
    //    {
    //        return;
    //    }

    //    DoDrawNetworkPositionGizmo();
    //    DoDrawExtrapolatedPositionGizmo();
    //}

    void DoDrawEstimatedPositionError()
    {
        Vector3 targetPosition = this.m_PositionControl.GetNetworkPosition();

        Debug.DrawLine( targetPosition, this.transform.position, Color.red, 2f );
        Debug.DrawLine(this.transform.position, this.transform.position + Vector3.up, Color.green, 2f );
        Debug.DrawLine( targetPosition, targetPosition + Vector3.up, Color.red, 2f );
    }

    //void DoDrawNetworkPositionGizmo()
    //{
    //    if( m_PositionModel.DrawNetworkGizmo == false || m_PositionControl == null )
    //    {
    //        return;
    //    }

    //    ExitGames.Client.GUI.GizmoTypeDrawer.Draw( m_PositionControl.GetNetworkPosition(),
    //                                               m_PositionModel.NetworkGizmoType,
    //                                               m_PositionModel.NetworkGizmoColor,
    //                                               m_PositionModel.NetworkGizmoSize );
    //}

    //void DoDrawExtrapolatedPositionGizmo()
    //{
    //    if( m_PositionModel.DrawExtrapolatedGizmo == false ||
    //        m_PositionModel.ExtrapolateOption == PhotonTransformViewPositionModel.ExtrapolateOptions.Disabled ||
    //        m_PositionControl == null )
    //    {
    //        return;
    //    }

    //    ExitGames.Client.GUI.GizmoTypeDrawer.Draw( m_PositionControl.GetNetworkPosition() + m_PositionControl.GetExtrapolatedPositionOffset(),
    //                                               m_PositionModel.ExtrapolatedGizmoType,
    //                                               m_PositionModel.ExtrapolatedGizmoColor,
    //                                               m_PositionModel.ExtrapolatedGizmoSize );
    //}
}

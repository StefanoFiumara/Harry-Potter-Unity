using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhotonTransformViewPositionControl 
{
    PhotonTransformViewPositionModel m_Model;
    float m_CurrentSpeed;
    double m_LastSerializeTime;
    Vector3 m_SynchronizedSpeed = Vector3.zero;
    float m_SynchronizedTurnSpeed = 0;

    Vector3 m_NetworkPosition;
    Queue<Vector3> m_OldNetworkPositions = new Queue<Vector3>();

    bool m_UpdatedPositionAfterOnSerialize = true;

    public PhotonTransformViewPositionControl( PhotonTransformViewPositionModel model )
    {
        this.m_Model = model;
    }

    Vector3 GetOldestStoredNetworkPosition()
    {
        Vector3 oldPosition = this.m_NetworkPosition;

        if(this.m_OldNetworkPositions.Count > 0 )
        {
            oldPosition = this.m_OldNetworkPositions.Peek();
        }

        return oldPosition;
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
        this.m_SynchronizedSpeed = speed;
        this.m_SynchronizedTurnSpeed = turnSpeed;
    }

    /// <summary>
    /// Calculates the new position based on the values setup in the inspector
    /// </summary>
    /// <param name="currentPosition">The current position.</param>
    /// <returns>The new position.</returns>
    public Vector3 UpdatePosition( Vector3 currentPosition )
    {
        Vector3 targetPosition = this.GetNetworkPosition() + this.GetExtrapolatedPositionOffset();

        switch(this.m_Model.InterpolateOption )
        {
        case PhotonTransformViewPositionModel.InterpolateOptions.Disabled:
            if(this.m_UpdatedPositionAfterOnSerialize == false )
            {
                currentPosition = targetPosition;
                this.m_UpdatedPositionAfterOnSerialize = true;
            }
            break;
        case PhotonTransformViewPositionModel.InterpolateOptions.FixedSpeed:
            currentPosition = Vector3.MoveTowards( currentPosition, targetPosition, Time.deltaTime *this.m_Model.InterpolateMoveTowardsSpeed );
            break;
        case PhotonTransformViewPositionModel.InterpolateOptions.EstimatedSpeed:
            int positionsCount = Mathf.Min( 1, this.m_OldNetworkPositions.Count );
            float estimatedSpeed = Vector3.Distance(this.m_NetworkPosition, this.GetOldestStoredNetworkPosition() ) / positionsCount;
            currentPosition = Vector3.MoveTowards( currentPosition, targetPosition, Time.deltaTime * estimatedSpeed );
            break;
        case PhotonTransformViewPositionModel.InterpolateOptions.SynchronizeValues:
            if(this.m_SynchronizedSpeed.magnitude == 0 )
            {
                currentPosition = targetPosition;
            }
            else
            {
                currentPosition = Vector3.MoveTowards( currentPosition, targetPosition, Time.deltaTime *this.m_SynchronizedSpeed.magnitude );
            }
            break;
        case PhotonTransformViewPositionModel.InterpolateOptions.Lerp:
            currentPosition = Vector3.Lerp( currentPosition, targetPosition, Time.deltaTime *this.m_Model.InterpolateLerpSpeed );
            break;
        /*case PhotonTransformViewPositionModel.InterpolateOptions.MoveTowardsComplex:
            float distanceToTarget = Vector3.Distance( currentPosition, targetPosition );
            float targetSpeed = m_Model.InterpolateSpeedCurve.Evaluate( distanceToTarget ) * m_Model.InterpolateMoveTowardsSpeed;

            if( targetSpeed > m_CurrentSpeed )
            {
                m_CurrentSpeed = Mathf.MoveTowards( m_CurrentSpeed, targetSpeed, Time.deltaTime * m_Model.InterpolateMoveTowardsAcceleration );
            }
            else
            {
                m_CurrentSpeed = Mathf.MoveTowards( m_CurrentSpeed, targetSpeed, Time.deltaTime * m_Model.InterpolateMoveTowardsDeceleration );
            }

            //Debug.Log( m_CurrentSpeed + " - " + targetSpeed + " - " + transform.localPosition + " - " + targetPosition );

            currentPosition = Vector3.MoveTowards( currentPosition, targetPosition, Time.deltaTime * m_CurrentSpeed );
            break;*/
        }

        if(this.m_Model.TeleportEnabled == true )
        {
            if( Vector3.Distance( currentPosition, this.GetNetworkPosition() ) > this.m_Model.TeleportIfDistanceGreaterThan )
            {
                currentPosition = this.GetNetworkPosition();
            }
        }

        return currentPosition;
    }

    /// <summary>
    /// Gets the last position that was received through the network
    /// </summary>
    /// <returns></returns>
    public Vector3 GetNetworkPosition()
    {
        return this.m_NetworkPosition;
    }

    /// <summary>
    /// Calculates an estimated position based on the last synchronized position,
    /// the time when the last position was received and the movement speed of the object
    /// </summary>
    /// <returns>Estimated position of the remote object</returns>
    public Vector3 GetExtrapolatedPositionOffset()
    {
        float timePassed = (float)( PhotonNetwork.time - this.m_LastSerializeTime );

        if(this.m_Model.ExtrapolateIncludingRoundTripTime == true )
        {
            timePassed += (float)PhotonNetwork.GetPing() / 1000f;
        }

        Vector3 extrapolatePosition = Vector3.zero;

        switch(this.m_Model.ExtrapolateOption )
        {
        case PhotonTransformViewPositionModel.ExtrapolateOptions.SynchronizeValues:
            Quaternion turnRotation = Quaternion.Euler( 0, this.m_SynchronizedTurnSpeed * timePassed, 0 );
            extrapolatePosition = turnRotation * (this.m_SynchronizedSpeed * timePassed );
            break;
        case PhotonTransformViewPositionModel.ExtrapolateOptions.FixedSpeed:
            Vector3 moveDirection = (this.m_NetworkPosition - this.GetOldestStoredNetworkPosition() ).normalized;

            extrapolatePosition = moveDirection *this.m_Model.ExtrapolateSpeed * timePassed;
            break;
        case PhotonTransformViewPositionModel.ExtrapolateOptions.EstimateSpeedAndTurn:
            Vector3 moveDelta = (this.m_NetworkPosition - this.GetOldestStoredNetworkPosition() ) * PhotonNetwork.sendRateOnSerialize;
            extrapolatePosition = moveDelta * timePassed;
            break;
        }

        return extrapolatePosition;
    }

    public void OnPhotonSerializeView( Vector3 currentPosition, PhotonStream stream, PhotonMessageInfo info )
    {
        if(this.m_Model.SynchronizeEnabled == false )
        {
            return;
        }

        if( stream.isWriting == true )
        {
            this.SerializeData( currentPosition, stream, info );
        }
        else
        {
            this.DeserializeData( stream, info );
        }

        this.m_LastSerializeTime = PhotonNetwork.time;
        this.m_UpdatedPositionAfterOnSerialize = false;
    }

    void SerializeData( Vector3 currentPosition, PhotonStream stream, PhotonMessageInfo info )
    {
        stream.SendNext( currentPosition );
        this.m_NetworkPosition = currentPosition;

        if(this.m_Model.ExtrapolateOption == PhotonTransformViewPositionModel.ExtrapolateOptions.SynchronizeValues || this.m_Model.InterpolateOption == PhotonTransformViewPositionModel.InterpolateOptions.SynchronizeValues )
        {
            stream.SendNext(this.m_SynchronizedSpeed );
            stream.SendNext(this.m_SynchronizedTurnSpeed );
        }
    }

    void DeserializeData( PhotonStream stream, PhotonMessageInfo info )
    {
        this.m_OldNetworkPositions.Enqueue(this.m_NetworkPosition );

        while(this.m_OldNetworkPositions.Count > this.m_Model.ExtrapolateNumberOfStoredPositions )
        {
            this.m_OldNetworkPositions.Dequeue();
        }

        this.m_NetworkPosition = (Vector3)stream.ReceiveNext();

        if(this.m_Model.ExtrapolateOption == PhotonTransformViewPositionModel.ExtrapolateOptions.SynchronizeValues || this.m_Model.InterpolateOption == PhotonTransformViewPositionModel.InterpolateOptions.SynchronizeValues )
        {
            this.m_SynchronizedSpeed = (Vector3)stream.ReceiveNext();
            this.m_SynchronizedTurnSpeed = (float)stream.ReceiveNext();
        }
    }
}

namespace HarryPotterUnity.Utils
{
    public class PlayerNetworkView : Photon.MonoBehaviour
    {

        void Start()
        {
       
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                // We own this player: send the others our data

            }
            else
            {
                // Network player, receive data

            }
        }

        void Update()
        {
            if (!photonView.isMine)
            {
            
            }
        }
    }
}
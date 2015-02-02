using Assets.Scripts.HarryPotterUnity.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HarryPotterUnity.Utils
{
    public class NetworkManager : MonoBehaviour
    {
        [SerializeField] private Text _networkStatusText;

        public void Start ()
        {
            PhotonNetwork.ConnectUsingSettings("v0.1");
        }

        public void FixedUpdate()
        {
            _networkStatusText.text = string.Format("Network Status: {0}", PhotonNetwork.connectionStateDetailed);
        }

        public void OnJoinedLobby()
        {
            
        }
    }
}

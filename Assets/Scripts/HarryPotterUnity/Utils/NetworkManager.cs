using Assets.Scripts.HarryPotterUnity.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HarryPotterUnity.Utils
{
    public class NetworkManager : MonoBehaviour
    {
        [SerializeField] private Text _networkStatusText;
        private bool _connected = false;

        public enum NetworkState
        {
            Disconnected, InLobby, WaitingForMatch, InGame
        }

        private NetworkState _networkState = NetworkState.Disconnected;

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
            _networkState = NetworkState.InLobby;
        }

        public void OnJoinedRoom()
        {
            _networkState = PhotonNetwork.room.playerCount >= 2 ? NetworkState.InGame : NetworkState.WaitingForMatch;
        }

        public void OnPhotonPlayerConnected()
        {
            _networkState = PhotonNetwork.room.playerCount >= 2 ? NetworkState.InGame : NetworkState.WaitingForMatch;

            //start game here?
        }

        public void OnGUI()
        {
            switch (_networkState)
            {
                case NetworkState.InLobby:
                    ShowMainMenu();
                    break;
                case NetworkState.WaitingForMatch:
                    ShowWaiting();
                    break;
                case NetworkState.InGame:
                    ShowStartingGame();
                    break;
            }
        }

        private void ShowStartingGame()
        {
            GUI.Label(new Rect(100,100,190,90), "Match found! Joining Game...");
        }

        private void ShowWaiting()
        {
            GUI.Label(new Rect(100,100,190,90), "Waiting for match...");
        }

        private void ShowMainMenu()
        {
            if (GUI.Button(new Rect(100, 100, 190, 90), "Find Match..."))
            {
                _networkState = NetworkState.WaitingForMatch;
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public void OnPhotonRandomJoinFailed()
        {
            PhotonNetwork.JoinOrCreateRoom(string.Format("Room {0}", PhotonNetwork.GetRoomList().Length),  null, null);
        }
    }
}

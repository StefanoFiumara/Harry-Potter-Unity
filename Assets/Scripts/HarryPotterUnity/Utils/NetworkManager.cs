using HarryPotterUnity.Game;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 649

namespace HarryPotterUnity.Utils
{
    public class NetworkManager : MonoBehaviour
    {
        [SerializeField] private Text _networkStatusText;
        [SerializeField] private GameManager _gameManager;

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

        public void OnPhotonPlayerConnected()
        {
            _networkState = NetworkState.InGame;

            //spawn player here? prompt for lesson type?
            _gameManager.SpawnPlayer2();
        }

        public void OnJoinedRoom()
        {
            if (PhotonNetwork.room.playerCount < 2)
            {
                _networkState = NetworkState.WaitingForMatch;
            }
            else
            {
                _networkState = NetworkState.InGame;
                //spawn player here? prompt for lesson types?
                _gameManager.SpawnPlayer1();
            }
        }

        public void OnPhotonPlayerDisconnected()
        {
            PhotonNetwork.LeaveRoom();
            //remove players?
        }

        public void OnGUI()
        {
            GUI.contentColor = Color.yellow;
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
            GUI.Label(new Rect(10,20,130,40), "Match found! Joining Game...");
        }

        private void ShowWaiting()
        {
            GUI.Label(new Rect(10,20,130,40), "Waiting for match...");
        }

        private void ShowMainMenu()
        {
            if (GUI.Button(new Rect(10, 20, 130, 40), "Find Match..."))
            {
                FindMatch_Click();
            }
        }

        private void FindMatch_Click()
        {
            _networkState = NetworkState.WaitingForMatch;
            PhotonNetwork.JoinRandomRoom();
        }

        public void OnPhotonRandomJoinFailed()
        {
            var roomName = string.Format("Room {0}", PhotonNetwork.GetRoomList().Length);
            PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions {maxPlayers =  2}, null);
        }
    }
}

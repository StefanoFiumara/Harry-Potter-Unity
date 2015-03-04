using HarryPotterUnity.Game;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 649

namespace HarryPotterUnity.Utils
{
    public class NetworkManager : MonoBehaviour
    {
        [SerializeField] private Text _networkStatusText;

        private GameObject _manager;

        public enum NetworkState
        {
            Disconnected, InLobby, WaitingForMatch, InGame
        }

        private NetworkState _networkState = NetworkState.Disconnected;

        public void Start ()
        {
            PhotonNetwork.ConnectUsingSettings("v0.1");
        }

        public void Update()
        {
            _networkStatusText.text = string.Format("Network Status: {0}", PhotonNetwork.connectionStateDetailed);
        }

        public void OnJoinedLobby()
        {
            _networkState = NetworkState.InLobby;
        }

        public void OnJoinedRoom()
        {
            _networkState = PhotonNetwork.room.playerCount < 2 ? NetworkState.WaitingForMatch : NetworkState.InGame;

            if (PhotonNetwork.room.playerCount == 1)
            {
                _manager = PhotonNetwork.Instantiate("MultiplayerGameManager", Vector3.zero, Quaternion.identity, 0);
            }
        }

        public void OnPhotonPlayerConnected()
        {
            _networkState = NetworkState.InGame;

            var seed = Random.Range(int.MinValue, int.MaxValue);
            _manager.GetPhotonView().RPC("StartGameRpc", PhotonTargets.All, seed);
        }

        public void OnPhotonPlayerDisconnected()
        {
            PhotonNetwork.LeaveRoom();
            //remove players?
        }

        // ReSharper disable once InconsistentNaming
        public void OnGUI()
        {
            //GUI.contentColor = Color.yellow;
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

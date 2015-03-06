using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649

namespace HarryPotterUnity.UI
{
    [UsedImplicitly]
    public class MultiplayerLobbyHudManager : MonoBehaviour
    {
        private MultiplayerGameManager _multiplayerGameManager;

        [SerializeField]
        private Camera _mainCamera;
        [SerializeField]
        private Camera _previewCamera;

        #region HUD Elements
        [SerializeField]
        private Button _findMatchButton;

        [SerializeField]
        private Text _networkStatusText;

        [SerializeField] 
        private Text _gameStatusText;

        [SerializeField] 
        private Text _titleText;
        #endregion

        [UsedImplicitly]
        public void Start ()
        {
            PhotonNetwork.ConnectUsingSettings("v0.1");
        }

        [UsedImplicitly]
        public void Update()
        {
            _networkStatusText.text = string.Format("Network Status: {0}", PhotonNetwork.connectionStateDetailed);
        }

        [UsedImplicitly]
        public void OnJoinedLobby()
        {
            _gameStatusText.text = "Connected to Photon Server!";
            _findMatchButton.gameObject.SetActive(true);

            _titleText.gameObject.SetActive(true);
            _gameStatusText.gameObject.SetActive(true);
        }

        [UsedImplicitly]
        public void OnJoinedRoom()
        {
            if (PhotonNetwork.room.playerCount == 1)
            {
                _multiplayerGameManager =
                    PhotonNetwork.Instantiate("MultiplayerGameManager", Vector3.zero, Quaternion.identity, 0)
                        .GetComponent<MultiplayerGameManager>();
            }
            else
            {
                var rotation = Quaternion.Euler(0f, 0f, 180f);

                _mainCamera.transform.rotation = rotation;
                _previewCamera.transform.rotation = rotation;
            }
        }

        [UsedImplicitly]
        public void OnPhotonPlayerConnected()
        {
            var seed = Random.Range(int.MinValue, int.MaxValue);
            _multiplayerGameManager.photonView.RPC("StartGameRpc", PhotonTargets.All, seed);
        }

        [UsedImplicitly]
        public void OnPhotonPlayerDisconnected()
        {
            PhotonNetwork.LeaveRoom();

            _gameStatusText.text = "Disconnected from Match...\nReturning to Lobby.";
            _titleText.gameObject.SetActive(true);
            _gameStatusText.gameObject.SetActive(true);
        }

        [UsedImplicitly]
        public void FindMatch_Click()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        [UsedImplicitly]
        public void OnPhotonRandomJoinFailed()
        {
            var roomName = string.Format("Room {0}", PhotonNetwork.GetRoomList().Length);
            PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions {maxPlayers =  2}, null);
        }

        public void DisableHud()
        {
            _titleText.gameObject.SetActive(false);
            _gameStatusText.gameObject.SetActive(false);
        }
    }
}

﻿using HarryPotterUnity.Game;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 649

namespace HarryPotterUnity.Utils
{
    public class HudManager : MonoBehaviour
    {
        private MultiplayerGameManager _multiplayerGameManager;

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
            _gameStatusText.text = "Connected to Photon Server!";
            _findMatchButton.gameObject.SetActive(true);

            _titleText.gameObject.SetActive(true);
            _gameStatusText.gameObject.SetActive(true);
        }

        public void OnJoinedRoom()
        {
            if (PhotonNetwork.room.playerCount == 1)
            {
                _multiplayerGameManager = PhotonNetwork.Instantiate("MultiplayerGameManager", Vector3.zero, Quaternion.identity, 0).GetComponent<MultiplayerGameManager>();
            }
        }

        public void OnPhotonPlayerConnected()
        {
            var seed = Random.Range(int.MinValue, int.MaxValue);
            _multiplayerGameManager.photonView.RPC("StartGameRpc", PhotonTargets.All, seed);
        }

        public void OnPhotonPlayerDisconnected()
        {
            PhotonNetwork.LeaveRoom();

            _titleText.gameObject.SetActive(true);
            _gameStatusText.gameObject.SetActive(true);
        }

        public void FindMatch_Click()
        {
            PhotonNetwork.JoinRandomRoom();
        }

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

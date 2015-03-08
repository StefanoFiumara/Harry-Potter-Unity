using System;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using HarryPotterUnity.Cards;
using UnityEngine;
using UnityEngine.UI;
using LessonTypes = HarryPotterUnity.Cards.Lesson.LessonTypes;
using Random = UnityEngine.Random;

#pragma warning disable 649

namespace HarryPotterUnity.UI
{
    [UsedImplicitly]
    public class MultiplayerLobbyHudManager : MonoBehaviour
    {
        private MultiplayerGameManager _multiplayerGameManager;

        #region HUD Elements
        [SerializeField]
        private Camera _mainCamera;
        [SerializeField]
        private Camera _previewCamera;

        [SerializeField]
        private Button _findMatchButton;

        [SerializeField] 
        private Text _gameStatusText;

        [SerializeField] 
        private Text _titleText;

        [SerializeField] 
        private RectTransform _errorPanel;

        [SerializeField] 
        private RectTransform _lessonSelectPanel;

        [SerializeField] 
        private Toggle _selectCreatures;
        [SerializeField]
        private Toggle _selectCharms;
        [SerializeField]
        private Toggle _selectTransfiguration;
        [SerializeField]
        private Toggle _selectPotions;
        [SerializeField]
        private Toggle _selectQuidditch;
        #endregion

        public List<LessonTypes> SelectedLessons;
        
        [UsedImplicitly]
        public void Start()
        {
            PhotonNetwork.ConnectUsingSettings("v0.1");
            SelectedLessons = new List<LessonTypes>();
        }

        [UsedImplicitly]
        public void OnJoinedLobby()
        {
            _gameStatusText.text = "Connected to Photon Server!";
            _findMatchButton.gameObject.SetActive(true);

            _titleText.gameObject.SetActive(true);
            _gameStatusText.gameObject.SetActive(true);

            EnableLessonSelect();
            _lessonSelectPanel.gameObject.SetActive(true);
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
            UpdateLessonSelection();
            if (SelectedLessons.Count == 2 || SelectedLessons.Count == 3)
            {
                _findMatchButton.gameObject.SetActive(false);
                _gameStatusText.text = "Finding Match...";
                PhotonNetwork.JoinRandomRoom();
                
                DisableLessonSelect();
            }
            else
            {
                _errorPanel.gameObject.SetActive(true);
            }

        }

        private void UpdateLessonSelection()
        {
            SelectedLessons.Clear();
            if (_selectCreatures.isOn) SelectedLessons.Add(LessonTypes.Creatures);
            if (_selectCharms.isOn) SelectedLessons.Add(LessonTypes.Charms);
            if (_selectTransfiguration.isOn) SelectedLessons.Add(LessonTypes.Transfiguration);
            if (_selectPotions.isOn) SelectedLessons.Add(LessonTypes.Potions);
            if (_selectQuidditch.isOn) SelectedLessons.Add(LessonTypes.Quidditch);

            //Convert to byte array for serialization
            var selectedLessons = Array.ConvertAll(SelectedLessons.ToArray(), input => (byte)input);
            var selected = new Hashtable {{"lessons", selectedLessons}};
            PhotonNetwork.player.SetCustomProperties(selected);
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
            _lessonSelectPanel.gameObject.SetActive(false);
        }

        public void DisableLessonSelect()
        {
            _selectCreatures.interactable = false;
            _selectCharms.interactable = false;
            _selectTransfiguration.interactable = false;
            _selectPotions.interactable = false;
            _selectQuidditch.interactable = false;
        }

        public void EnableLessonSelect()
        {
            _selectCreatures.interactable = true;
            _selectCharms.interactable = true;
            _selectTransfiguration.interactable = true;
            _selectPotions.interactable = true;
            _selectQuidditch.interactable = true;
        }
    }
}

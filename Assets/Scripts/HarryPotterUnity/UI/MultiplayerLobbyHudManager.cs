using System;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using System.Collections.Generic;
using ExitGames.Client.Photon;
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

        [Header("Main Menu")]
        [SerializeField]
        private RectTransform _mainMenuHudContainer;

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

        [Header("Gameplay")]
        [SerializeField]
        private RectTransform _gameplayHudContainer;

        [SerializeField] 
        private Image _turnIndicatorLocal;
        [SerializeField]
        private Image _turnIndicatorRemote;

        [SerializeField]
        private Text _cardsLeftLocal;
        [SerializeField]
        private Text _cardsLeftRemote;

        [SerializeField]
        private Text _actionsLeftLocal;
        [SerializeField]
        private Text _actionsLeftRemote;
        [SerializeField]
        private RectTransform _endGamePanel;
        #endregion

        public Image TurnIndicatorLocal { get { return _turnIndicatorLocal;} }
        public Image TurnIndicatorRemote { get { return _turnIndicatorRemote; } }

        public Text CardsLeftLocal { get { return _cardsLeftLocal;} }
        public Text CardsLeftRemote { get { return _cardsLeftRemote;} }

        public Text ActionsLeftLocal { get { return _actionsLeftLocal;} }
        public Text ActionsLeftRemote { get { return _actionsLeftRemote;} }

        public RectTransform EndGamePanel { get { return _endGamePanel; } }



        private List<LessonTypes> _selectedLessons;
        
        [UsedImplicitly]
        public void Start()
        {
            PhotonNetwork.ConnectUsingSettings("v0.1");
            _selectedLessons = new List<LessonTypes>();
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
            BackToMainMenu();
        }

        [UsedImplicitly]
        public void BackToMainMenu()
        {
            PhotonNetwork.LeaveRoom();

            _gameStatusText.text = "Disconnected from Match...\nReturning to Lobby.";

            _mainCamera.transform.rotation = Quaternion.identity;
            _previewCamera.transform.rotation = Quaternion.identity;

            _turnIndicatorLocal.gameObject.SetActive(false);
            _turnIndicatorRemote.gameObject.SetActive(false);

            UtilManager.TweenQueue.Reset();

            DisableGameplayHud();
            EnableMainMenuHud();
        }

        [UsedImplicitly]
        public void FindMatch_Click()
        {
            UpdateLessonSelection();
            if (_selectedLessons.Count == 2 || _selectedLessons.Count == 3)
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
            _selectedLessons.Clear();
            if (_selectCreatures.isOn) _selectedLessons.Add(LessonTypes.Creatures);
            if (_selectCharms.isOn) _selectedLessons.Add(LessonTypes.Charms);
            if (_selectTransfiguration.isOn) _selectedLessons.Add(LessonTypes.Transfiguration);
            if (_selectPotions.isOn) _selectedLessons.Add(LessonTypes.Potions);
            if (_selectQuidditch.isOn) _selectedLessons.Add(LessonTypes.Quidditch);

            //Convert to byte array for serialization
            var selectedLessons = Array.ConvertAll(_selectedLessons.ToArray(), input => (byte)input);
            var selected = new Hashtable {{"lessons", selectedLessons}};
            PhotonNetwork.player.SetCustomProperties(selected);
        }

        [UsedImplicitly]
        public void OnPhotonRandomJoinFailed()
        {
            var roomName = string.Format("Room {0}", PhotonNetwork.GetRoomList().Length);
            PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions {maxPlayers =  2}, null);
        }

        public void DisableMainMenuHud()
        {
            _mainMenuHudContainer.gameObject.SetActive(false);
        }

        private void EnableMainMenuHud()
        {
            _mainMenuHudContainer.gameObject.SetActive(true);
        }

        public void EnableGameplayHud()
        {
            _gameplayHudContainer.gameObject.SetActive(true);
        }

        private void DisableGameplayHud()
        {
            _gameplayHudContainer.gameObject.SetActive(false);
        }

        private void DisableLessonSelect()
        {
            _selectCreatures.interactable = false;
            _selectCharms.interactable = false;
            _selectTransfiguration.interactable = false;
            _selectPotions.interactable = false;
            _selectQuidditch.interactable = false;
        }

        private void EnableLessonSelect()
        {
            _selectCreatures.interactable = true;
            _selectCharms.interactable = true;
            _selectTransfiguration.interactable = true;
            _selectPotions.interactable = true;
            _selectQuidditch.interactable = true;
        }
    }
}

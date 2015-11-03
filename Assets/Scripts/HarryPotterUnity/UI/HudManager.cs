using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using MonoBehaviour = UnityEngine.MonoBehaviour;

namespace HarryPotterUnity.UI
{
    [UsedImplicitly]
    public class HudManager : MonoBehaviour
    {
        private NetworkManager _networkManager;

        #region HUD Elements
        [SerializeField, UsedImplicitly]
        private Camera _mainCamera;
        [SerializeField, UsedImplicitly]
        private Camera _previewCamera;

        [Header("Main Menu")]
        [SerializeField, UsedImplicitly]
        private RectTransform _mainMenuHudContainer;

        [SerializeField, UsedImplicitly]
        private Button _findMatchButton;

        [SerializeField, UsedImplicitly] 
        private Text _gameStatusText;

        [SerializeField, UsedImplicitly]
        private Text _playersOnlineText;

        [SerializeField, UsedImplicitly] 
        private Text _titleText;

        [SerializeField, UsedImplicitly] 
        private RectTransform _errorPanel;

        [SerializeField, UsedImplicitly] 
        private RectTransform _lessonSelectPanel;

        [SerializeField, UsedImplicitly] 
        private Toggle _selectCreatures;
        [SerializeField, UsedImplicitly]
        private Toggle _selectCharms;
        [SerializeField, UsedImplicitly]
        private Toggle _selectTransfiguration;
        [SerializeField, UsedImplicitly]
        private Toggle _selectPotions;
        [SerializeField, UsedImplicitly]
        private Toggle _selectQuidditch;

        [Header("Gameplay")]
        [SerializeField, UsedImplicitly]
        private RectTransform _gameplayHudContainer;

        [SerializeField, UsedImplicitly] 
        private Image _turnIndicatorLocal;
        [SerializeField, UsedImplicitly]
        private Image _turnIndicatorRemote;

        [SerializeField, UsedImplicitly]
        private Text _cardsLeftLocal;
        [SerializeField, UsedImplicitly]
        private Text _cardsLeftRemote;

        [SerializeField, UsedImplicitly]
        private Text _actionsLeftLocal;
        [SerializeField, UsedImplicitly]
        private Text _actionsLeftRemote;
        [SerializeField, UsedImplicitly]
        private RectTransform _endGamePanel;

        [SerializeField, UsedImplicitly]
        private GameObject _skipActionButton;
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
            _networkManager = FindObjectsOfType<NetworkManager>().First();
            _selectedLessons = new List<LessonTypes>();
        }

        [UsedImplicitly]
        public void Update()
        {
            _playersOnlineText.text = string.Format("Players Online: {0}", PhotonNetwork.countOfPlayers);
        }

        public void InitMainMenu()
        {
            _gameStatusText.text = "Connected to Photon Server!";
            _findMatchButton.gameObject.SetActive(true);

            _titleText.gameObject.SetActive(true);
            _gameStatusText.gameObject.SetActive(true);

            EnableLessonSelect();
            _lessonSelectPanel.gameObject.SetActive(true);
        }

        public void SetPlayer2CameraRotation()
        {
            var rotation = Quaternion.Euler(0f, 0f, 180f);

            _mainCamera.transform.rotation = rotation;
            _previewCamera.transform.rotation = rotation;
        }

        public void BackToMainMenu()
        {
            if (PhotonNetwork.inRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            
            _networkManager.DestroyPlayerObjects();

            _gameStatusText.text = "Disconnected from Match...\nReturning to Lobby.";

            if    (_mainCamera != null)  _mainCamera.transform.rotation    = Quaternion.identity;
            if (_previewCamera != null)  _previewCamera.transform.rotation = Quaternion.identity;

            _turnIndicatorLocal.gameObject.SetActive(false);
            _turnIndicatorRemote.gameObject.SetActive(false);

            _skipActionButton.SetActive(true);

            _endGamePanel.gameObject.SetActive(false);
            GameManager.TweenQueue.Reset();

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

        [UsedImplicitly]
        public void SkipAction_Click()
        {
            _networkManager.photonView.RPC("ExecuteSkipAction", PhotonTargets.All);
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

            _skipActionButton.SetActive(PhotonNetwork.isMasterClient);
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

        public void ToggleSkipActionButton()
        {
            _skipActionButton.SetActive(!_skipActionButton.activeSelf);
        }
    }
}

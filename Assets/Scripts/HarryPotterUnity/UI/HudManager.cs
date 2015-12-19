using System.Linq;
using HarryPotterUnity.Game;
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
        private Button _cancelFindMatchButton;

        [SerializeField, UsedImplicitly] 
        private Text _gameStatusText;

        [SerializeField, UsedImplicitly]
        private Text _playersOnlineText;

        [SerializeField, UsedImplicitly] 
        private Text _titleText;

        [SerializeField, UsedImplicitly] 
        private RectTransform _errorPanel;
        
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


        [SerializeField, UsedImplicitly] private Text _networkStatusText;

        
        public void Start()
        {
            _networkManager = FindObjectsOfType<NetworkManager>().First();
        }
        
        public void InitMainMenu()
        {
            _findMatchButton.gameObject.SetActive(true);
            _findMatchButton.interactable = true;
            _cancelFindMatchButton.gameObject.SetActive(false);
        }

        public void SetPlayer2CameraRotation()
        {
            var rotation = Quaternion.Euler(0f, 0f, 180f);

            _mainCamera.transform.rotation = rotation;
            _previewCamera.transform.rotation = rotation;
        }
  
        [UsedImplicitly]
        public void SkipAction_Click()
        {
            _networkManager.photonView.RPC("ExecuteSkipAction", PhotonTargets.All);
        }
        
        public void ToggleSkipActionButton()
        {
            _skipActionButton.SetActive(!_skipActionButton.activeSelf);
        }
    }
}

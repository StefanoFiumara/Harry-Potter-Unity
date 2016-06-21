using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityLogWrapper;

namespace HarryPotterUnity.UI.Menu
{
    public class GameplayMenu : BaseMenu
    {
        private Player _localPlayer;
        private Player _remotePlayer;

        public Player LocalPlayer
        {
            private get { return _localPlayer; }
            set
            {
                _localPlayer = value;

                _localPlayer.OnTurnStart += () =>
                {
                    _actionsLeftLabelLocal.GetComponent<Animator>().SetBool("IsOpen", true);
                    _skipActionButton.interactable = true;
                };

                _localPlayer.OnTurnEnd += () =>
                {
                    _actionsLeftLabelLocal.GetComponent<Animator>().SetBool("IsOpen", false);
                    _skipActionButton.interactable = false;
                };

                _localPlayer.InPlay.OnCardEnteredPlay += card =>
                {
                    if (card is ILessonProvider)
                    {
                        UpdateLessonPanel();
                    }
                };

                _localPlayer.InPlay.OnCardExitedPlay += card =>
                {
                    if (card is ILessonProvider)
                    {
                        UpdateLessonPanel();
                    }
                };

                _localPlayer.Deck.OnDeckIsOutOfCards += ShowGameOverMessage;

                UpdateLessonPanel();
            }
        }

        public Player RemotePlayer
        {
            private get { return _remotePlayer; }
            set
            {
                _remotePlayer = value;
                _remotePlayer.OnTurnStart +=
                    () => _actionsLeftLabelRemote.GetComponent<Animator>().SetBool("IsOpen", true);
                _remotePlayer.OnTurnEnd +=
                    () => _actionsLeftLabelRemote.GetComponent<Animator>().SetBool("IsOpen", false);

                _remotePlayer.Deck.OnDeckIsOutOfCards += ShowGameOverMessage;
            }
        }

        private Text _actionsLeftLabelLocal;
        private Text _actionsLeftLabelRemote;

        private Text _cardsLeftLabelLocal;
        private Text _cardsLeftLabelRemote;

        private Button _skipActionButton;

        private Text _mainMenuTitle;
        private Image _mainMenuBackground;

        private Text _lessonCountLabel;

        private Image _lessonIndicatorCreatures;
        private Image _lessonIndicatorCharms;
        private Image _lessonIndicatorTransfiguration;
        private Image _lessonIndicatorPotions;
        private Image _lessonIndicatorQuidditch;

        protected override void Awake()
        {
            base.Awake();

            var allText = FindObjectsOfType<Text>();
            var allImages = FindObjectsOfType<Image>();

            _mainMenuBackground = allImages.FirstOrDefault(i => i.name.Contains("MainMenuBackground"));
            _mainMenuTitle = allText.FirstOrDefault(i => i.name.Contains("MainMenuTitle"));

            _actionsLeftLabelLocal = allText.FirstOrDefault(t => t.name.Contains("ActionsLeftLabel_Local"));
            _actionsLeftLabelRemote = allText.FirstOrDefault(t => t.name.Contains("ActionsLeftLabel_Remote"));

            _cardsLeftLabelLocal = allText.FirstOrDefault(t => t.name.Contains("CardsLeftLabel_Local"));
            _cardsLeftLabelRemote = allText.FirstOrDefault(t => t.name.Contains("CardsLeftLabel_Remote"));

            _skipActionButton = FindObjectsOfType<Button>().FirstOrDefault(b => b.name.Contains("SkipActionButton"));

            _lessonCountLabel = allText.FirstOrDefault(t => t.name.Contains("LessonCountLabel"));
            _lessonIndicatorCreatures = allImages.FirstOrDefault(i => i.name.Contains("LessonIndicator_Creatures"));
            _lessonIndicatorCharms = allImages.FirstOrDefault(i => i.name.Contains("LessonIndicator_Charms"));
            _lessonIndicatorTransfiguration =
                allImages.FirstOrDefault(i => i.name.Contains("LessonIndicator_Transfiguration"));
            _lessonIndicatorPotions = allImages.FirstOrDefault(i => i.name.Contains("LessonIndicator_Potions"));
            _lessonIndicatorQuidditch = allImages.FirstOrDefault(i => i.name.Contains("LessonIndicator_Quidditch"));

            if (_actionsLeftLabelLocal == null ||
                _actionsLeftLabelRemote == null ||
                _cardsLeftLabelLocal == null ||
                _cardsLeftLabelRemote == null ||
                _mainMenuTitle == null ||
                _mainMenuBackground == null ||
                _skipActionButton == null ||
                _lessonCountLabel == null ||
                _lessonIndicatorCreatures == null ||
                _lessonIndicatorCharms == null ||
                _lessonIndicatorTransfiguration == null ||
                _lessonIndicatorPotions == null ||
                _lessonIndicatorQuidditch == null)
            {
                Log.Error("Could not find all needed HUD elements in gameplay menu, report this error!");
            }
        }

        private void Start()
        {
            _skipActionButton.interactable = false;
        }

        protected override void Update()
        {
            base.Update();

            if (LocalPlayer != null)
            {
                //Update Local Player Properties
                _actionsLeftLabelLocal.text = string.Format("Actions Left: {0}", LocalPlayer.ActionsAvailable);
                _cardsLeftLabelLocal.text = string.Format("Cards Left: {0}", LocalPlayer.Deck.Cards.Count);
            }

            if (RemotePlayer != null)
            {
                //Update Remote Plater Properties
                _actionsLeftLabelRemote.text = string.Format("Actions Left: {0}", RemotePlayer.ActionsAvailable);
                _cardsLeftLabelRemote.text = string.Format("Cards Left: {0}", RemotePlayer.Deck.Cards.Count);
            }
        }

        private void ShowGameOverMessage(Player loser)
        {
            //TODO: Test how this function behaves
            //Unsub from the other game over message here, only show the win/lose based on who died first
            Log.Write("Player {0} has lost the game.", loser.NetworkId+1);
        }

        private void UpdateLessonPanel()
        {
            //TODO: Test this
            _lessonCountLabel.text = string.Format("Lesson Power: {0:D2}", LocalPlayer.AmountLessonsInPlay);

            _lessonIndicatorCreatures.gameObject.SetActive(LocalPlayer.LessonTypesInPlay.Contains(LessonTypes.Creatures));
            _lessonIndicatorCharms.gameObject.SetActive(LocalPlayer.LessonTypesInPlay.Contains(LessonTypes.Charms));
            _lessonIndicatorTransfiguration.gameObject.SetActive(LocalPlayer.LessonTypesInPlay.Contains(LessonTypes.Transfiguration));
            _lessonIndicatorPotions.gameObject.SetActive(LocalPlayer.LessonTypesInPlay.Contains(LessonTypes.Potions));
            _lessonIndicatorQuidditch.gameObject.SetActive(LocalPlayer.LessonTypesInPlay.Contains(LessonTypes.Quidditch));
        }

        [UsedImplicitly]
        public void SkipAction()
        {
            var player = LocalPlayer.CanUseActions() ? LocalPlayer : RemotePlayer;

            if (player.ActionsAvailable == 1)
            {
                _skipActionButton.interactable = false;
            }

            GameManager.Network.RPC("ExecuteSkipAction", PhotonTargets.All);
        }

        public override void OnShowMenu()
        {
            _mainMenuBackground.GetComponent<Animator>().SetBool("IsVisible", false);
            _mainMenuTitle.GetComponent<Animator>().SetBool("IsVisible", false);
        }

        public override void OnHideMenu()
        {
            _mainMenuBackground.GetComponent<Animator>().SetBool("IsVisible", true);
            _mainMenuTitle.GetComponent<Animator>().SetBool("IsVisible", true);
        }
    }
}

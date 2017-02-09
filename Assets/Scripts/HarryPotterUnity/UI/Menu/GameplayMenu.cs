using System.Linq;
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
            private get { return this._localPlayer; }
            set
            {
                this._localPlayer = value;

                this._localPlayer.OnStartTurnEvent += () =>
                {
                    this._actionsLeftLabelLocal.GetComponent<Animator>().SetBool("IsOpen", true);
                    this._skipActionButton.interactable = true;
                };

                this._localPlayer.OnEndTurnEvent += () =>
                {
                    this._actionsLeftLabelLocal.GetComponent<Animator>().SetBool("IsOpen", false);
                    this._skipActionButton.interactable = false;
                };

                this._localPlayer.InPlay.OnCardEnteredPlay += card =>
                {
                        this.UpdateLessonPanel();
                };

                this._localPlayer.InPlay.OnCardExitedPlay += card =>
                {
                        this.UpdateLessonPanel();
                };

                this._localPlayer.Deck.OnDeckIsOutOfCardsEvent += this.ShowGameOverMessage;

                this.UpdateLessonPanel(forceUpdate: true);
            }
        }

        public Player RemotePlayer
        {
            private get { return this._remotePlayer; }
            set
            {
                this._remotePlayer = value;
                this._remotePlayer.OnStartTurnEvent +=
                    () => this._actionsLeftLabelRemote.GetComponent<Animator>().SetBool("IsOpen", true);
                this._remotePlayer.OnEndTurnEvent +=
                    () => this._actionsLeftLabelRemote.GetComponent<Animator>().SetBool("IsOpen", false);

                this._remotePlayer.Deck.OnDeckIsOutOfCardsEvent += this.ShowGameOverMessage;
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

            var allText   = FindObjectsOfType<Text>();
            var allImages = FindObjectsOfType<Image>();

            this._mainMenuBackground = allImages.FirstOrDefault(i => i.name.Contains("MainMenuBackground"));
            this._mainMenuTitle      = allText.FirstOrDefault(i => i.name.Contains("MainMenuTitle"));

            this._actionsLeftLabelLocal  = allText.FirstOrDefault(t => t.name.Contains("ActionsLeftLabel_Local"));
            this._actionsLeftLabelRemote = allText.FirstOrDefault(t => t.name.Contains("ActionsLeftLabel_Remote"));

            this._cardsLeftLabelLocal  = allText.FirstOrDefault(t => t.name.Contains("CardsLeftLabel_Local"));
            this._cardsLeftLabelRemote = allText.FirstOrDefault(t => t.name.Contains("CardsLeftLabel_Remote"));

            this._skipActionButton = FindObjectsOfType<Button>().FirstOrDefault(b => b.name.Contains("SkipActionButton"));

            this._lessonCountLabel               = allText.FirstOrDefault(t => t.name.Contains("LessonCountLabel"));
            this._lessonIndicatorCreatures       = allImages.FirstOrDefault(i => i.name.Contains("LessonIndicator_Creatures"));
            this._lessonIndicatorCharms          = allImages.FirstOrDefault(i => i.name.Contains("LessonIndicator_Charms"));
            this._lessonIndicatorTransfiguration = allImages.FirstOrDefault(i => i.name.Contains("LessonIndicator_Transfiguration"));
            this._lessonIndicatorPotions         = allImages.FirstOrDefault(i => i.name.Contains("LessonIndicator_Potions"));
            this._lessonIndicatorQuidditch       = allImages.FirstOrDefault(i => i.name.Contains("LessonIndicator_Quidditch"));

            if (this._actionsLeftLabelLocal == null || 
                this._actionsLeftLabelRemote == null || 
                this._cardsLeftLabelLocal == null || 
                this._cardsLeftLabelRemote == null || 
                this._mainMenuTitle == null || 
                this._mainMenuBackground == null || 
                this._skipActionButton == null || 
                this._lessonCountLabel == null || 
                this._lessonIndicatorCreatures == null || 
                this._lessonIndicatorCharms == null || 
                this._lessonIndicatorTransfiguration == null || 
                this._lessonIndicatorPotions == null || 
                this._lessonIndicatorQuidditch == null)
            {
                Log.Error("Could not find all needed HUD elements in gameplay menu, report this error!");
            }
        }

        private void Start()
        {
            this._skipActionButton.interactable = false;
        }

        protected override void Update()
        {
            base.Update();

            if (this.LocalPlayer != null)
            {
                //Update Local Player Properties
                this._actionsLeftLabelLocal.text = string.Format("Actions Left: {0}", this.LocalPlayer.ActionsAvailable);
                this._cardsLeftLabelLocal.text = string.Format("Cards Left: {0}", this.LocalPlayer.Deck.Cards.Count);
            }

            if (this.RemotePlayer != null)
            {
                //Update Remote Plater Properties
                this._actionsLeftLabelRemote.text = string.Format("Actions Left: {0}", this.RemotePlayer.ActionsAvailable);
                this._cardsLeftLabelRemote.text = string.Format("Cards Left: {0}", this.RemotePlayer.Deck.Cards.Count);
            }
        }

        private void ShowGameOverMessage(Player loser)
        {
            //TODO: Test how this function behaves, show pop up here or use NetworkManager to RPC the event to both players
            //Unsub from the other game over message here, only show the win/lose based on who died first
            Log.Write("Player {0} has lost the game.", loser.NetworkId+1);
        }

        private void UpdateLessonPanel(bool forceUpdate = false)
        {
            int currentAmount;
            bool parsed = int.TryParse(this._lessonCountLabel.text, out currentAmount);

            if (!parsed) forceUpdate = true;

            if (currentAmount == this.LocalPlayer.AmountLessonsInPlay && !forceUpdate) return;

            this._lessonCountLabel.text = this.LocalPlayer.AmountLessonsInPlay <= 0 ? string.Empty : this.LocalPlayer.AmountLessonsInPlay.ToString();
            this._lessonIndicatorCreatures.gameObject.SetActive(this.LocalPlayer.LessonTypesInPlay.Contains(LessonTypes.Creatures));
            this._lessonIndicatorCharms.gameObject.SetActive(this.LocalPlayer.LessonTypesInPlay.Contains(LessonTypes.Charms));
            this._lessonIndicatorTransfiguration.gameObject.SetActive(this.LocalPlayer.LessonTypesInPlay.Contains(LessonTypes.Transfiguration));
            this._lessonIndicatorPotions.gameObject.SetActive(this.LocalPlayer.LessonTypesInPlay.Contains(LessonTypes.Potions));
            this._lessonIndicatorQuidditch.gameObject.SetActive(this.LocalPlayer.LessonTypesInPlay.Contains(LessonTypes.Quidditch));
        }
        
        [UsedImplicitly]
        public void SkipAction()
        {
            var player = this.LocalPlayer.CanUseActions() ? this.LocalPlayer : this.RemotePlayer;
            
            if (player.ActionsAvailable == 1)
            {
                this._skipActionButton.interactable = false;
            }

            GameManager.Network.RPC("ExecuteSkipAction", PhotonTargets.All);
        }

        public override void OnShowMenu()
        {
            this._mainMenuBackground.GetComponent<Animator>().SetBool("IsVisible", false);
            this._mainMenuTitle.GetComponent<Animator>().SetBool("IsVisible", false);
        }

        public override void OnHideMenu()
        {
            this._mainMenuBackground.GetComponent<Animator>().SetBool("IsVisible", true);
            this._mainMenuTitle.GetComponent<Animator>().SetBool("IsVisible", true);
        }
    }
}

using System.Linq;
using HarryPotterUnity.Game;
using UnityEngine.UI;
using UnityLogWrapper;

namespace HarryPotterUnity.UI.Menu
{
    public class GameplayMenu : BaseMenu
    {
        private Player _localPlayer;
        private Player _remotePlayer;

        //TODO: Use animator to toggle the labels
        public Player LocalPlayer
        {
            private get { return _localPlayer; }
            set
            {
                _localPlayer = value;
                _localPlayer.OnTurnStart += () => _actionsLeftLabelLocal.gameObject.SetActive(true);
                _localPlayer.OnTurnEnd += () => _actionsLeftLabelLocal.gameObject.SetActive(false);
            }
        }

        public Player RemotePlayer
        {
            private get { return _remotePlayer; }
            set
            {
                _remotePlayer = value;
                _remotePlayer.OnTurnStart += () => _actionsLeftLabelRemote.gameObject.SetActive(true);
                _remotePlayer.OnTurnEnd += () => _actionsLeftLabelRemote.gameObject.SetActive(false);

            }
        }

        private Text _actionsLeftLabelLocal;
        private Text _actionsLeftLabelRemote;

        private Text _cardsLeftLabelLocal;
        private Text _cardsLeftLabelRemote;
        

        protected override void Awake()
        {
            base.Awake();

            var allText = FindObjectsOfType<Text>();

            _actionsLeftLabelLocal = allText.FirstOrDefault(t => t.name.Contains("ActionsLeftLabel_Local"));
            _actionsLeftLabelRemote = allText.FirstOrDefault(t => t.name.Contains("ActionsLeftLabel_Remote"));

            _cardsLeftLabelLocal = allText.FirstOrDefault(t => t.name.Contains("CardsLeftLabel_Local"));
            _cardsLeftLabelRemote = allText.FirstOrDefault(t => t.name.Contains("CardsLeftLabel_Remote"));

            if (_actionsLeftLabelLocal == null || _actionsLeftLabelRemote == null || _cardsLeftLabelLocal == null || _cardsLeftLabelRemote == null)
            {
                Log.Error("Could not find all needed HUD elements in gameplay menu, report this error!");
            }
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

        public override void OnShowMenu()
        {
            //Hide Main Menu Background and Logo
        }

        public override void OnHideMenu()
        {
            //Show Main Menu Background and Logo
        }
    }
}

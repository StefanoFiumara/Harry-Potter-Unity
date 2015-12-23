using System.Linq;
using HarryPotterUnity.Game;
using UnityEngine;
using UnityEngine.UI;
using UnityLogWrapper;

namespace HarryPotterUnity.UI.Menu
{
    public class GameplayMenu : BaseMenu
    {
        public Player LocalPlayer { private get; set; }
        public Player RemotePlayer { private get; set; }

        private Text _actionsLeftLabelLocal;
        private Text _actionsLeftLabelRemote;

        protected override void Awake()
        {
            base.Awake();

            _actionsLeftLabelLocal = FindObjectsOfType<Text>().First(t => t.name.Contains("ActionsLeftLabel_Local"));
            _actionsLeftLabelRemote = FindObjectsOfType<Text>().First(t => t.name.Contains("ActionsLeftLabel_Remote"));

            if (_actionsLeftLabelLocal == null || _actionsLeftLabelRemote == null)
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
            }

            if (RemotePlayer != null)
            {
                //Update Remote Plater Properties
                _actionsLeftLabelRemote.text = string.Format("Actions Left: {0}", RemotePlayer.ActionsAvailable);
            }
        }
    }
}

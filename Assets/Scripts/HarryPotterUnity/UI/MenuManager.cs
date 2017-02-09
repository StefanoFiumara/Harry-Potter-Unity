using System.Linq;
using HarryPotterUnity.UI.Menu;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace HarryPotterUnity.UI
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField]
        private BaseMenu _currentMenu;

        [SerializeField]
        private SubMenuManager _subMenuManager;

        private Text _networkStatus;
        private Text _playersOnline;

        private void Awake()
        {
            this._subMenuManager = this.GetComponent<SubMenuManager>();

            var allTextObjects = FindObjectsOfType<Text>();

            this._networkStatus = allTextObjects.SingleOrDefault(t => t.name.Contains("NetworkStatusDetailed"));
            this._playersOnline = allTextObjects.SingleOrDefault(t => t.name.Contains("PlayersOnlineLabel"));
        }

        [UsedImplicitly]
        public void OnJoinedLobby()
        {
            this.ShowMenu(this._currentMenu);
        }

        public void ShowMenu(BaseMenu menu)
        {
            if (this._currentMenu != null)
            {
                this._currentMenu.IsOpen = false;
            }

            this._currentMenu = menu;
            this._currentMenu.IsOpen = true;

            this._subMenuManager.HideMenu();
        }

        [UsedImplicitly]
        public void HideMenu()
        {
            if (this._currentMenu != null)
            {
                this._currentMenu.IsOpen = false;
            }

            this._currentMenu = null;
        }

        private void Update()
        {
            this._playersOnline.text = string.Format("Players Online: {0}", PhotonNetwork.countOfPlayers);
            this._networkStatus.text = string.Format("Network Status: {0}", PhotonNetwork.connectionStateDetailed);
        }
    }
}

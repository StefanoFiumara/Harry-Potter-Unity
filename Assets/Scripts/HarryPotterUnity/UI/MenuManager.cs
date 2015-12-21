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
            _subMenuManager = GetComponent<SubMenuManager>();

            var allTextObjects = FindObjectsOfType<Text>();

            _networkStatus = allTextObjects.SingleOrDefault(t => t.name.Contains("NetworkStatusDetailed"));
            _playersOnline = allTextObjects.SingleOrDefault(t => t.name.Contains("PlayersOnlineLabel"));
        }

        [UsedImplicitly]
        public void OnJoinedLobby()
        {
            ShowMenu(_currentMenu);
        }

        [UsedImplicitly]
        public void ShowMenu(BaseMenu menu)
        {
            if (_currentMenu != null)
            {
                _currentMenu.IsOpen = false;
            }
            
            _currentMenu = menu;
            _currentMenu.IsOpen = true;
            _currentMenu.OnShowMenu();

            _subMenuManager.HideMenu();
        }

        [UsedImplicitly]
        public void HideMenu(BaseMenu menu)
        {
            if (menu.IsOpen)
            {
                menu.IsOpen = false;
                _currentMenu = null;
            }
        }

        private void Update()
        {
            _playersOnline.text = string.Format("Players Online: {0}", PhotonNetwork.countOfPlayers);
            _networkStatus.text = string.Format("Network Status: {0}", PhotonNetwork.connectionStateDetailed);
        }
    }
}

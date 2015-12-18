using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.UI
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField]
        private Menu _currentMenu;

        [SerializeField]
        private SubMenuManager _subMenuManager;

        private void Start()
        {
            _subMenuManager = GetComponent<SubMenuManager>();
        }

        [UsedImplicitly]
        public void OnJoinedLobby()
        {
            ShowMenu(_currentMenu);
        }

        [UsedImplicitly]
        public void ShowMenu(Menu menu)
        {
            if (_currentMenu != null)
            {
                _currentMenu.IsOpen = false;
            }

            _currentMenu = menu;
            _currentMenu.IsOpen = true;

            _subMenuManager.ShowMenu(null);
        }
    }
}

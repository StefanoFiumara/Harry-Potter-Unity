using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.UI
{
    public class SubMenuManager : MonoBehaviour
    {
        private Menu _currentMenu;
        
        [UsedImplicitly]
        public void ShowMenu(Menu menu)
        {
            if (menu.IsOpen)
            {
                return;
            }
            
            _currentMenu = menu;
            _currentMenu.IsOpen = true;
        }

        [UsedImplicitly]
        public void HideMenu()
        {
            if (_currentMenu == null) return;

            _currentMenu.IsOpen = false;
            _currentMenu = null;
        }
    }
}

using HarryPotterUnity.UI.Menu;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.UI
{
    public class SubMenuManager : MonoBehaviour
    {
        private BaseMenu _currentMenu;
        
        public void ShowMenu(BaseMenu menu)
        {
            if (menu.IsOpen)
            {
                return;
            }
            
            _currentMenu = menu;
            _currentMenu.IsOpen = true;
        }

        public void HideMenu()
        {
            if (_currentMenu == null) return;

            _currentMenu.IsOpen = false;
            _currentMenu.OnHideMenu();
            _currentMenu = null;
        }
    }
}

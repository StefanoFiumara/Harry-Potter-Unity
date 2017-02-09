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

            this._currentMenu = menu;
            this._currentMenu.IsOpen = true;
        }

        public void HideMenu()
        {
            if (this._currentMenu == null) return;

            this._currentMenu.IsOpen = false;
            this._currentMenu.OnHideMenu();
            this._currentMenu = null;
        }
    }
}

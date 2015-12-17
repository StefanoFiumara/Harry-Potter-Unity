using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.UI
{
    public class SubMenuManager : MonoBehaviour
    {
        [SerializeField]
        private Menu _currentMenu;
        
        [UsedImplicitly]
        public void ShowMenu(Menu menu)
        {
            if (_currentMenu != null && _currentMenu == menu)
            {
                _currentMenu.IsOpen = false;
                _currentMenu = null;
            }
            else if (menu == null)
            {
                if (_currentMenu != null) _currentMenu.IsOpen = false;
                _currentMenu = null;
            }
            else
            {
                _currentMenu = menu;
                _currentMenu.IsOpen = true;
            }
        }
    }
}

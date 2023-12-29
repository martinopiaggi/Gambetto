using UnityEngine;

namespace Gambetto.Scripts.UI
{
    public class SettingsMenu : MonoBehaviour
    {
        private GameObject _callerMenu;

        // Method that makes the settings menu appear and hides the menu from which it was called
        public void OpenSettingsMenu(GameObject menu)
        {
            _callerMenu = menu;
            menu.SetActive(false);
            gameObject.SetActive(true);
        }

        public void CloseSettingsMenu()
        {
            gameObject.SetActive(false);
            // If the menu from which the settings menu was called is not null, it is made active again
            if (_callerMenu != null)
                _callerMenu.SetActive(true);
        }
    }
}

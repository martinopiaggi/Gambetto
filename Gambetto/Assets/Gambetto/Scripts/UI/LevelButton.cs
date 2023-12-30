using UnityEngine;

namespace Gambetto.Scripts.UI
{
    public class LevelButton : MonoBehaviour
    {
        public void BackMenu()
        {
            LevelSelector.instance.BackToMainMenu();
        }

        public void LoadLevel(int level)
        {
            LevelSelector.instance.LoadLevel(level);
        }
    }
}

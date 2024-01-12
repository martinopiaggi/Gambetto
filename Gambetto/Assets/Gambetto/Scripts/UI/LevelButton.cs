using UnityEngine;

namespace Gambetto.Scripts.UI
{
    public class LevelButton : MonoBehaviour
    {
        public string levelName;
        public string nextLevelName;

        public void BackMenu()
        {
            LevelSelector.instance.BackToMainMenu();
        }

        public void LoadLevel(int level)
        {
            LevelSelector.instance.LoadLevel(level);
        }

        public void LoadLevel()
        {
            LevelSelector.instance.LoadLevel(levelName);
            GameManager.Instance.nextLevel = nextLevelName;
        }
    }
}

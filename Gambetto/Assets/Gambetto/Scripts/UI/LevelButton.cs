using System.Collections.Generic;
using UnityEngine;

namespace Gambetto.Scripts.UI
{
    public class LevelButton : MonoBehaviour
    {
        public string levelName;

        public List<string> nextLevels;

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
            GameManager.Instance.nextLevels = nextLevels;
        }
    }
}

using System.Collections.Generic;
using Gambetto.Scripts.Utils;
using UnityEngine;

namespace Gambetto.Scripts.UI
{
    public class LevelButton : MonoBehaviour
    {
        public string levelName;

        public List<string> nextLevels;

        public void BackMenu()
        {
            TimeManager.StopTime();
            GameManager.Instance.sceneTransition.CrossFade("MainMenu");
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

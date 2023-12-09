using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts;
using UnityEngine;

namespace Gambetto.Scripts.UI
{
    public class LevelButton : MonoBehaviour
    {
        public void BackMenu()
        {
            LevelSelector.Instance.BackToMainMenu();
        }

        public void LoadLevel(string level)
        {
            LevelSelector.Instance.LoadLevel(level);
        }
    }
}

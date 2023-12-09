using Gambetto.Scripts.Utils;
using UnityEngine;

namespace Gambetto.Scripts.UI
{
    public class PauseButton : MonoBehaviour
    {
        public void OpenSettings()
        {
            TimeManager.StopTime();
        }

        public void ResumeGame()
        {
            TimeManager.ResumeTime();
        }

        public void BackToMainMenu()
        {
            TimeManager.ResumeTime();
            GameManager.Instance.sceneTransition.CrossFade("Level selection");
        }
    }
}

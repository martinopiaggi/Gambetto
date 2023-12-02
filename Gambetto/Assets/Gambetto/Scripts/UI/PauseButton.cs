using UnityEngine;

namespace Gambetto.Scripts.UI
{
    public class PauseButton : MonoBehaviour
    {
        public void OpenSettings()
        {
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1f;
        }
    }
}

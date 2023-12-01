using UnityEngine;

namespace Gambetto.Scripts.UI
{
    public class PauseButton : MonoBehaviour
    {
        public void OpenSettings()
        {
            //pause clock
            GameClock.Instance.PauseClock();
            //show settings menu
        }

        public void ResumeGame()
        {
            //resume clock
            GameClock.Instance.ResumeClock();
        }
    }
}

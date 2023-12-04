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
    }
}

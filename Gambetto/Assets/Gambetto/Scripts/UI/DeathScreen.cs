using Gambetto.Scripts.GameCore.Grid;
using Gambetto.Scripts.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gambetto.Scripts.UI
{
    public class DeathScreen : MonoBehaviour
    {
        GridManager gridManager;

        private void Awake()
        {
            gridManager = FindObjectOfType<GridManager>();
        }

        public void Retry()
        {
            TimeManager.ResumeTime();
            gridManager.RestartLevel();
        }

        public void BackToMainMenu()
        {
            TimeManager.ResumeTime();
            SceneManager.LoadScene("MainMenu");
        }
    }
}

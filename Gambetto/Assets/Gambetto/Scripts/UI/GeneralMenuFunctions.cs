using Gambetto.Scripts.GameCore.Grid;
using Gambetto.Scripts.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gambetto.Scripts.UI
{
    public class GeneralMenuFunctions : MonoBehaviour
    {
        private GridManager gridManager;

        private void Awake()
        {
            gridManager = FindObjectOfType<GridManager>();
        }

        /// <summary>
        /// Restart the level and sets a given menu to inactive,
        /// </summary>
        /// <param name="menuToClose">Menu to set inactive.</param>
        public void RestartAndCloseMenu(GameObject menuToClose = null)
        {
            if (menuToClose != null)
                menuToClose.SetActive(false);
            TimeManager.ResumeTime();
            PauseButton.mouseOverItemDropLocation = false;
            gridManager.RestartLevel();
        }

        public void BackToLevels(GameObject menuToClose = null)
        {
            if (menuToClose != null)
                menuToClose.SetActive(false);
            GameManager.Instance.sceneTransition.CrossFade("Level selection");
            TimeManager.ResumeTime();
        }

        public void ResumeGame(GameObject menuToClose = null)
        {
            if (menuToClose != null)
                menuToClose.SetActive(false);
            TimeManager.ResumeTime();
        }

        public void LoadNextLevel()
        {
            
            var nextLevel = GameManager.Instance.GetNextLevel(SceneManager.GetActiveScene().name);
            if (nextLevel == SceneManager.GetActiveScene().name)
            {
                BackToLevels();
                return;
            }
            GameManager.Instance.sceneTransition.CrossFade(nextLevel);

            //todo: set the GameManager.Instance.nextLevel to the next level

            TimeManager.ResumeTime();
        }
    }
}

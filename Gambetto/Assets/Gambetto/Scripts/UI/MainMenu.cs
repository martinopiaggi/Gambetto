using UnityEngine;

namespace Gambetto.Scripts.UI
{
    public class MainMenu : MonoBehaviour
    {
        public void PlayGame()
        {
            GameManager.Instance.sceneTransition.CrossFade("Level selection");
        }

        public void QuitGame()
        {
            Debug.Log("Quit");
            Application.Quit();
        }
    }
}

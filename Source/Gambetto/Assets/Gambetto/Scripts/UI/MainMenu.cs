using UnityEngine;

namespace Gambetto.Scripts.UI
{
    public class MainMenu : MonoBehaviour
    {
        public void PlayGame()
        {
            GameManager.Instance.sceneTransition.CrossFade(1);
        }
        
        public void OpenCredits()
        {
            GameManager.Instance.sceneTransition.CrossFade("CreditsScene");
        }

        public void QuitGame()
        {
            Debug.Log("Quit");
            Application.Quit();
        }
    }
}

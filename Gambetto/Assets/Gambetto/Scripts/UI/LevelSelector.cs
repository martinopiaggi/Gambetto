using UnityEngine;

namespace Gambetto.Scripts.UI
{
    public class LevelSelector : MonoBehaviour
    {
        // Start is called before the first frame update

        public static LevelSelector Instance;

        //awake method makes sure that LevelSelector is not destroyed
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadLevel(string level)
        {
            GameManager.Instance.sceneTransition.CrossFade(level);
        }

        //method used by the back button
        public void BackToMainMenu()
        {
            GameManager.Instance.sceneTransition.CrossFade("MainMenu");
            //SceneManager.LoadScene("MainMenu");
        }
    }
}

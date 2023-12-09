using UnityEngine;

namespace Gambetto.Scripts.UI
{
    public class LevelSelector : MonoBehaviour
    {
        // Start is called before the first frame update

        public static LevelSelector Instance;

        public int currentLevel;

        //boolean list to keep track of completed levels
        private bool[] completedLevels = new bool[10];

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
            currentLevel = 1;
            GameManager.Instance.sceneTransition.CrossFade(level);
            //SceneManager.LoadScene("Sample Scene");
        }

        //method used by the back button
        public void BackToMainMenu()
        {
            GameManager.Instance.sceneTransition.CrossFade("MainMenu");
            //SceneManager.LoadScene("MainMenu");
        }
    }
}

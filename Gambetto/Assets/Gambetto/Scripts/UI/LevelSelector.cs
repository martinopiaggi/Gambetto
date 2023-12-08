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

        public void LoadLevel1()
        {
            currentLevel = 1;
            SceneTransition.Instance.CrossFade("Sample Scene");
            //SceneManager.LoadScene("Sample Scene");
        }

        //method used by the back button
        public void BackToMainMenu()
        {
            SceneTransition.Instance.CrossFade("MainMenu");
            //SceneManager.LoadScene("MainMenu");
        }

    }
}

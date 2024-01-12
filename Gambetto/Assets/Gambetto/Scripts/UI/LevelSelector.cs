using UnityEngine;

namespace Gambetto.Scripts.UI
{
    public class LevelSelector : MonoBehaviour
    {
        // Start is called before the first frame update

        public static LevelSelector instance;

        //awake method makes sure that LevelSelector is not destroyed
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadLevel(int level)
        {
            GameManager.Instance.sceneTransition.CrossFade(level);
        }

        public void LoadLevel(string level)
        {
            GameManager.Instance.sceneTransition.CrossFade(level);
        }

        //method used by the back button
        public void BackToMainMenu()
        {
            GameManager.Instance.sceneTransition.CrossFade(0);
        }
    }
}

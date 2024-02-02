using System.Collections;
using UnityEngine;

namespace Gambetto.Scripts.UI
{
    public class LevelSelector : MonoBehaviour
    {
        // Start is called before the first frame update

        public static LevelSelector instance;
        public string currentLevel;

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
            //start load level coroutine
            StartCoroutine(LoadLevelRoutine(level));
        }

        public void LoadLevel(string level)
        {
            //start load level coroutine
            StartCoroutine(LoadLevelRoutine(level));
            currentLevel = level;
        }

        private static IEnumerator LoadLevelRoutine(string level)
        {
            if (!GameManager.Instance.DisableQuotes)
            {
                GameManager.Instance.sceneTransition.CrossFade("QuotesScene");

                //wait for any key to be pressed
                while (!Input.anyKey)
                {
                    yield return null;
                }
            }

            GameManager.Instance.sceneTransition.CrossFade(level);
        }

        private static IEnumerator LoadLevelRoutine(int level)
        {
            if (!GameManager.Instance.DisableQuotes)
            {
                GameManager.Instance.sceneTransition.CrossFade("QuotesScene");

                //wait for any key to be pressed
                while (!Input.anyKey)
                {
                    yield return null;
                }
            }

            GameManager.Instance.sceneTransition.CrossFade(level);
        }
    }
}

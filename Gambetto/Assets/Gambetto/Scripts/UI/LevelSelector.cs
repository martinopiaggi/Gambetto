using System.Collections;
using Gambetto.Scripts.Utils;
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
            TimeManager.StopTime();
            //start load level coroutine
            StartCoroutine(LoadLevelRoutine(level));
        }

        public void LoadLevel(string level)
        {
            TimeManager.StopTime();
            //start load level coroutine
            StartCoroutine(LoadLevelRoutine(level));
            currentLevel = level;
        }

        //method used by the back button
        public void BackToMainMenu()
        {
            TimeManager.StopTime();
            GameManager.Instance.sceneTransition.CrossFade(0);
        }

        private IEnumerator LoadLevelRoutine(string level)
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

        private IEnumerator LoadLevelRoutine(int level)
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

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gambetto.Scripts.UI
{
    public class SceneTransition : MonoBehaviour
    {
        public static SceneTransition Instance;

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

        //add here other animations
        public Animator crossfadeAnimator;

        public void CrossFade(String scene)
        {
            StartCoroutine(LoadScene(scene));
        }

        IEnumerator LoadScene(String scene)
        {
            crossfadeAnimator.SetTrigger("end");
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(scene);
            crossfadeAnimator.SetTrigger("start");
        }
    }
}

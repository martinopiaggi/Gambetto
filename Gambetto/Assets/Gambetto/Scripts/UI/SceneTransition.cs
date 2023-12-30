using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Gambetto.Scripts.UI
{
    public class SceneTransition : MonoBehaviour
    {
        //add here other animations
        [FormerlySerializedAs("crossfadeAnimator")]
        public Animator crossFadeAnimator;

        private static readonly int End = Animator.StringToHash("end");
        private static readonly int Start = Animator.StringToHash("start");

        public void CrossFade(int scene)
        {
            StartCoroutine(LoadScene(scene));
        }

        private IEnumerator LoadScene(int scene)
        {
            crossFadeAnimator.SetTrigger(End);
            yield return new WaitForSeconds(1f);
            yield return SceneManager.LoadSceneAsync(scene);
            crossFadeAnimator.SetTrigger(Start);
        }
    }
}

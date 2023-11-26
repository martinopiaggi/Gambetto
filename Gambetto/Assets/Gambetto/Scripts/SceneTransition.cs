using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gambetto.Scripts
{
    public class SceneTransition : MonoBehaviour
    {
        public Animator crossfadeAnimator;
    
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        
        }

        public void CrossFade(String scene)
        {
            StartCoroutine(LoadScene(scene));
        }

        IEnumerator LoadScene(String scene)
        {
            crossfadeAnimator.SetTrigger("end");
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(scene);
        }
    }
}


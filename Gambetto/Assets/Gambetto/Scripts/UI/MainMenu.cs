using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gambetto.Scripts.UI
{
    public class MainMenu : MonoBehaviour
    {
        public SceneTransition transition;
        

        public void PlayGame()
        {
            transition.CrossFade("Level Selection");
            //SceneManager.LoadScene("Level selection");
        }

        public void QuitGame()
        {
            Debug.Log("Quit");
            Application.Quit();
        }
        
    
    }  
}


using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gambetto.Scripts.UI
{
    public class MainMenu : MonoBehaviour
    {
        public void PlayGame()
        {
            GameManager.Instance.sceneTransition.CrossFade("Level selection");
        }

        public void QuitGame()
        {
            Debug.Log("Quit");
            Application.Quit();
        }
    }
}

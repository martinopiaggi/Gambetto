using System;
using Gambetto.Scripts.UI;
using UnityEngine;

namespace Gambetto.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public SceneTransition sceneTransition;

        [SerializeField]
        private AudioManager audioManager;

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

        private void Start()
        {
            audioManager.PlayBackground(AudioManager.Instance.menuBackground);
        }
    }
}

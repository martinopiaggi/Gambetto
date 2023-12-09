using Gambetto.Scripts.UI;
using UnityEngine;

namespace Gambetto.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public SceneTransition sceneTransition;
        public AudioManager audioManager;

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

        // Start is called before the first frame update
        void Start() { }

        // Update is called once per frame
        void Update() { }
    }
}

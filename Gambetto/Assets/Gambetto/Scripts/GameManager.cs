using System;
using System.Collections.Generic;
using Gambetto.Scripts.UI;
using UnityEngine;

namespace Gambetto.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public SceneTransition sceneTransition;

        [SerializeField]
        private bool allLevelsUnlocked;

        //levels have a false status if they are locked
        private readonly Dictionary<string, bool> _levelStatus = new();

        public void SetLevelStatus(string levelName, bool status)
        {
            _levelStatus[levelName] = status;
        }

        public bool GetLevelStatus(string levelName)
        {
            if (allLevelsUnlocked)
                return true;

            _levelStatus.TryGetValue(levelName, out var status);
            return status;
        }

        public string nextLevel;

        private void Awake()
        {
            // get the names of all levels in the build settings
            for (
                var i = 2;
                i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
                i++
            )
            {
                var scenePath = UnityEngine
                    .SceneManagement
                    .SceneUtility
                    .GetScenePathByBuildIndex(i);
                var lastSlash = scenePath.LastIndexOf("/", StringComparison.Ordinal);
                var sceneName = scenePath.Substring(
                    lastSlash + 1,
                    scenePath.LastIndexOf(".", StringComparison.Ordinal) - lastSlash - 1
                );
                _levelStatus.Add(sceneName, false);
            }
            // set the first levels as unlocked
            _levelStatus["level0"] = true;

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

        private void Start()
        {
            AudioManager.Instance.PlayBackground(AudioManager.Instance.menuBackground);
        }
    }
}

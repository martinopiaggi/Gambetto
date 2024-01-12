using System;
using System.Collections.Generic;
using System.IO;
using Gambetto.Scripts.UI;
using Newtonsoft.Json;
using UnityEngine;

namespace Gambetto.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public SceneTransition sceneTransition;

        [SerializeField]
        private bool allLevelsUnlocked;

        private string _saveDataPath;

        //levels have a false status if they are locked
        private Dictionary<string, bool> _levelStatus = new();

        public void SetLevelStatus(string levelName, bool status)
        {
            _levelStatus[levelName] = status;
            SaveData();
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
            _saveDataPath = Application.persistentDataPath + "/level_data.json";
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

            // load saved data
            LoadData();

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

        private void SaveData()
        {
            var json = JsonConvert.SerializeObject(_levelStatus);
            File.WriteAllText(_saveDataPath, json);
        }

        private void LoadData()
        {
            if (!File.Exists(_saveDataPath))
                return;
            var json = File.ReadAllText(_saveDataPath);
            if (json == string.Empty)
                return;

            var data = JsonConvert.DeserializeObject<Dictionary<string, bool>>(json);

            foreach (var (key, value) in data)
            {
                _levelStatus[key] = value;
            }
        }
    }
}

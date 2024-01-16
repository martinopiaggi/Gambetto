using System;
using System.Collections.Generic;
using System.IO;
using Gambetto.Scripts.UI;
using Newtonsoft.Json;
using UnityEditor.PackageManager;
using UnityEngine;
using Object = System.Object;

namespace Gambetto.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public SceneTransition sceneTransition;

        [SerializeField]
        private bool allLevelsUnlocked;
        public bool AllLevelsUnlocked
        {
            get => allLevelsUnlocked;
            set => allLevelsUnlocked = value;
        }

        private string _nextLevelsSaveDataPath;
        private string _levelsCompletedSaveDataPath;

        //levels have a false status if they are locked
        private Dictionary<string, bool> _levelStatus = new();
        
        private Dictionary<string, bool> _levelsCompleted = new();

        public void SetLevelCompleted(string levelName)
        {
            // set level as completed
            _levelsCompleted[levelName] = true;
            SaveData(_levelsCompleted, _levelsCompletedSaveDataPath);
            
            // unlock the next level
            var nextLevel = GetNextLevel(levelName);
            _levelStatus[nextLevel] = true;
            SaveData(_levelStatus, _nextLevelsSaveDataPath);
        }

        public bool GetLevelStatus(string levelName)
        {
            if (allLevelsUnlocked)
                return true;

            _levelStatus.TryGetValue(levelName, out var status);
            return status;
        }
        
        public int GetLevelCount(bool  onlyCompleted = false)
        {
            return onlyCompleted ? _levelsCompleted.Count : _levelStatus.Count;
        }
        
        public List<string> nextLevels;

        /// <param name="currentLevel">level to get the next level of</param>
        /// <returns>returns the next level if exists, otherwise returns the current level</returns>
        public string GetNextLevel(string currentLevel)
        {
            var index = nextLevels.IndexOf(currentLevel);
            if (index == -1)
                return currentLevel;

            //return only if exists
            if (index + 1 < nextLevels.Count)
                return nextLevels[index + 1];
            return currentLevel;
        }

        private void Awake()
        {
            allLevelsUnlocked = PlayerPrefs.GetInt("AllLevelsUnlocked", 0) == 1;
                
            _nextLevelsSaveDataPath = Application.persistentDataPath + "/level_data.json";
            _levelsCompletedSaveDataPath = Application.persistentDataPath + "/completed_data.json";
            
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
            _levelStatus["tutortial"] = true;
            _levelStatus["pawnEnemyIntro"] = true;
            _levelStatus["BishopEscape"] = true;

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

        private void SaveData(object dataToSave, string path)
        {
            var json = JsonConvert.SerializeObject(dataToSave);
            File.WriteAllText(path, json);
        }

        private void LoadData()
        {
            var json = ReadFile(_nextLevelsSaveDataPath);
            if (json == string.Empty)
                return;

            var data = JsonConvert.DeserializeObject<Dictionary<string, bool>>(json);

            foreach (var (key, value) in data)
            {
                _levelStatus[key] = value;
            }
            
            json = ReadFile(_levelsCompletedSaveDataPath);
            if (json == string.Empty)
                return;
            
            data = JsonConvert.DeserializeObject<Dictionary<string, bool>>(json);
            
            foreach (var (key, value) in data)
            {
                _levelsCompleted[key] = value;
            }
        }
        
        private static string ReadFile(string path) {
            if (File.Exists(path)) 
                return File.ReadAllText(path);
            Debug.LogError("File "+path+ " not found");
            return string.Empty;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    // Start is called before the first frame update

    public static LevelSelector Instance;

    public int currentLevel;

    //boolean list to keep track of completed levels
    private bool[] completedLevels = new bool[10];
    
    
    
    //awake method makes sure that LevelSelector is not destroyed
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

    public void LoadLevel1()
    {
        currentLevel = 1;
        SceneManager.LoadScene("Prova 1");
    }
    
    
    //method used by the back button
    public void backToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    
    
    public List<RoomLayout> rooms;
    [SerializeField] private GameObject gridManager;
    
    
    //awake method makes sure that LevelManager is not destroyed
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
    void Start()
    {
        gridManager.GetComponent<GridManager>().CreateGrid(rooms);
    }

    
}

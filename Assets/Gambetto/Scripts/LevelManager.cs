using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<RoomLayout> rooms;
    [SerializeField] private GameObject gridManager;
    
    // Start is called before the first frame update
    void Start()
    {
        gridManager.GetComponent<GridManager>().CreateGrid(rooms);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

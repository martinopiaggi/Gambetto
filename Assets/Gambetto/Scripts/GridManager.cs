using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private GameObject roomPrefab;


    void Start()
    {
        
    }
    
    public void CreateGrid(List<RoomLayout> rooms)
    {
        Vector3 translation = new Vector3(0,0,0);
        foreach (RoomLayout room in rooms)
        {
            GameObject roomObj = Instantiate(roomPrefab,  transform.position, Quaternion.identity);
            roomObj.GetComponent<Room>().InitializeRoom(room);
            roomObj.transform.position = translation;
            translation = translation + new Vector3(7,0,0);
        }
    }
    
    private void Awake()
    {
        roomPrefab = Resources.Load<GameObject>("Prefabs/Room");
    }
}

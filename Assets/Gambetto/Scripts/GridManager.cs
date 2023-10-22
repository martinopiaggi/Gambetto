using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private GameObject roomPrefab;
    private int lastColor = 0;


    void Start()
    {
        
    }
    
    public void CreateGrid(List<RoomLayout> rooms)
    {
        Vector3 translation = new Vector3(0,0,0);
        foreach (RoomLayout room in rooms)
        {
            GameObject roomObj = Instantiate(roomPrefab,  transform.position, Quaternion.identity);
            roomObj.GetComponent<Room>().setColorStart(lastColor);
            roomObj.GetComponent<Room>().InitializeRoom(room);
            roomObj.transform.position = translation;
            translation = translation + new Vector3(room.GetSizeRow(),0,0);
            if (room.GetSizeRow() % 2 == 0 && lastColor == 1)
            {
                if (lastColor == 1)
                {
                    lastColor = 1;
                }
                else
                {
                    lastColor = 0;
                }
            }
            else
            {
                if (lastColor == 1)
                {
                    lastColor = 0;
                }
                else
                {
                    lastColor = 1;
                }
            }
        }
    }
    
    private void Awake()
    {
        roomPrefab = Resources.Load<GameObject>("Prefabs/Room");
    }
}

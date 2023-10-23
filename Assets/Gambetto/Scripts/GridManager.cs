using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private GameObject roomPrefab;
    private int lastColor = 1;
    private bool changed = false; 


    void Start()
    {
        
    }
    
    public void CreateGrid(List<RoomLayout> rooms)
    {
        Vector3 translation = new Vector3(0,0,0);
        foreach (RoomLayout room in rooms)
        {
            GameObject roomObj = Instantiate(roomPrefab,  transform.position, Quaternion.identity);
            //if the last color is 0 (white) the starting color will be changed in 1 (blue)
            if (lastColor == 1)
            {
                roomObj.GetComponent<Room>().setColorStart(0);
                changed = false;
            }
            else
            {
                roomObj.GetComponent<Room>().setColorStart(1);
                changed = true;
            }
            roomObj.GetComponent<Room>().InitializeRoom(room);
            roomObj.transform.position = translation;
            translation = translation + new Vector3(room.GetSizeRow(),0,0);
            //this if determine what is the last color of the room, 1 (blue), 0 (white)
            //if the room has an even lenght and was not changed its last color is 1 (blue)
            //if the room has an odd lenght and was not chaged its last color is 0 (white)
            if (room.GetSizeRow() % 2 == 0)
            {
                if (changed)
                {
                    lastColor = 0;
                }
                else
                {
                    lastColor = 1;
                }
                
            }
            else
            {
                if (changed)
                {
                    lastColor = 1;
                }
                else
                {
                    lastColor = 0;
                }
            }
        }
    }
    
    private void Awake()
    {
        roomPrefab = Resources.Load<GameObject>("Prefabs/Room");
    }
}

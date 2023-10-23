using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private GameObject roomPrefab;
    private int lastColor = 1;
    private bool changed = false;

    private List<List<Cell>> grid;

    
    public void CreateGrid(List<RoomLayout> rooms)
    {
        grid = new List<List<Cell>>();
        Vector3 translation = new Vector3(0,0,0);
        int roomIdx = 0;
        
        foreach (RoomLayout roomLayout in rooms)
        {
            grid.Add(populateRoomGraph(roomLayout,translation,roomIdx));
            
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
            roomObj.GetComponent<Room>().InitializeRoom(roomLayout);
            roomObj.transform.position = translation;
            translation = translation + new Vector3(roomLayout.GetExit().x*roomLayout.GetSizeRow(), 
                                        0,
                                        roomLayout.GetExit().z*roomLayout.GetSizeColumn());
            
            //this if determine what is the last color of the room, 1 (blue), 0 (white)
            //if the room has an even lenght and was not changed its last color is 1 (blue)
            //if the room has an odd lenght and was not chaged its last color is 0 (white)
            if (roomLayout.GetSizeRow() % 2 == 0)
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

            if (roomIdx > 0) //first room hasn't any previous room to check consinstency of common cells with
            {
                //make consistency of the cells of this room with the previous one
                //we use the roomLayout exitSide information to compute it 
                //we can use Directions to also update the "cells on the common edge" of the previous room
            }
            
            roomIdx++;
        }
    }
    
    private List<Cell> populateRoomGraph(RoomLayout roomLayout, Vector3 coordinateOrigin, int roomId)
    {
        //building first a temporary matrix to build easily the graph of cells
        int[,] squaresInfo = new int[roomLayout.GetSizeRow(), roomLayout.GetSizeColumn()];
        int rowNumber = 0;
        foreach (var column in roomLayout.GetRows())
        {
            int columnNumber = 0;
            foreach (var square in column.GetColumns())
            {
                squaresInfo[rowNumber, columnNumber] = square;
                columnNumber++;
            }
            rowNumber++;
        }

        
        //creating list of cell of a single room 
        // empty cells are null 
        List<Cell> roomCells = new List<Cell>();
        
        for (int i = 0; i < roomLayout.GetSizeRow(); i++)
        {
            for (int j = 0; j < roomLayout.GetSizeColumn(); j++)
            {
                if (squaresInfo[i,j] != -1) //if empty space no need to make a Cell object
                {
                    Cell cell = new Cell(coordinateOrigin + new Vector3(i, 0, j),roomId);
                    //@todo here we have to set all the neighbors links
                    roomCells.Add(cell);
                }
            }
        }

        Debug.Log(roomCells.Count);
        
        return roomCells;
    }
    
    private void Awake()
    {
        roomPrefab = Resources.Load<GameObject>("Prefabs/Room");
    }
}

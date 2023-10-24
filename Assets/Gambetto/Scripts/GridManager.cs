using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gambetto.Scripts.Utils;

public class GridManager : MonoBehaviour
{
    private GameObject _roomPrefab;
    private int _lastColor = 1;
    private bool _changed = false;

    private List<List<Cell>> _grid;

    
    public void CreateGrid(List<RoomLayout> roomLayouts)
    {
        _grid = new List<List<Cell>>();
        Vector3 translation = new Vector3(0,0,0);
        
        for (var roomIdx = 0; roomIdx < roomLayouts.Count; roomIdx++)
        {
            var roomLayout = roomLayouts[roomIdx];
            _grid.Add(PopulateRoomGraph(roomLayout,translation,roomIdx));
            
            GameObject roomObj = Instantiate(_roomPrefab,  transform.position, Quaternion.identity);
            //if the last color is 0 (white) the starting color will be changed in 1 (blue)
            if (_lastColor == 1)
            {
                roomObj.GetComponent<Room>().setColorStart(0);
                _changed = false;
            }
            else
            {
                roomObj.GetComponent<Room>().setColorStart(1);
                _changed = true;
            }
            roomObj.GetComponent<Room>().InitializeRoom(roomLayout);
            roomObj.transform.position = translation;

            if (roomLayout.GetExit() != Directions.South && roomLayout.GetExit() != Directions.East)
            {
                translation = translation + new Vector3(roomLayout.GetExit().x*roomLayout.GetSizeRow(), 
                    0,
                    roomLayout.GetExit().z*roomLayout.GetSizeColumn());
            }
            else
            {
                //we have to compute the correct translation considering **next** roomLayout size in case of South/East
                
                if(roomIdx!=(roomLayouts.Count-1)){
                    var nextRoomLayout = roomLayouts[roomIdx+1];
                        translation = translation + new Vector3(roomLayout.GetExit().x*nextRoomLayout.GetSizeRow(), 
                            0,
                            roomLayout.GetExit().z*nextRoomLayout.GetSizeColumn());
                }
            }
            
            
            //this if determine what is the last color of the room, 1 (blue), 0 (white)
            //if the room has an even lenght and was not changed its last color is 1 (blue)
            //if the room has an odd lenght and was not chaged its last color is 0 (white)
            if (roomLayout.GetSizeRow() % 2 == 0)
            {
                if (_changed)
                {
                    _lastColor = 0;
                }
                else
                {
                    _lastColor = 1;
                }
                
            }
            else
            {
                if (_changed)
                {
                    _lastColor = 1;
                }
                else
                {
                    _lastColor = 0;
                }
            }

            if (roomIdx > 0) //first room hasn't any previous room to check consistency of common cells with
            {
                //make consistency of the cells of this room with the previous one
                //we use the roomLayout exitSide information to compute it 
                //we can use Directions to also update the "cells on the common edge" of the previous room
            }

        }
    }
    
    
    

    private List<Cell> PopulateRoomGraph(RoomLayout roomLayout, Vector3 coordinateOrigin, int roomId)
    {
        //building first a temporary matrix to build easily the graph of cells
        var matrixCells = new Cell[roomLayout.GetSizeRow(), roomLayout.GetSizeColumn()];
        var roomCells = new List<Cell>();
        for(var rowNumber = 0; rowNumber < roomLayout.GetSizeRow();rowNumber++)
        {
            for (var columnNumber = 0; columnNumber < roomLayout.GetSizeColumn(); columnNumber++)
            {
                var square = roomLayout.GetRows()[rowNumber].GetColumns()[columnNumber];
                if (square == -1) continue; //if empty space no need to make a Cell object
                
                var cell = new Cell(coordinateOrigin + new Vector3(rowNumber, 0, columnNumber),roomId);
                roomCells.Add(cell);
                matrixCells[rowNumber, columnNumber] = cell;
                //set all the neighbors links updating also neighbors links
                if (columnNumber >= 1)
                {
                    var nextEast = matrixCells[rowNumber, columnNumber - 1];
                    if (nextEast != null)
                    {
                        cell.setNext(Directions.East,nextEast);
                        nextEast.setNext(Directions.West,cell); 
                    }
                        
                    if (rowNumber >= 1)
                    {
                        var nextSouthEast = matrixCells[rowNumber -1, columnNumber - 1];
                        if (nextSouthEast != null)
                        {
                            cell.setNext(Directions.SouthEast,nextSouthEast);
                            nextSouthEast.setNext(Directions.NorthWest,cell); 
                        }

                        if (columnNumber < roomLayout.GetSizeColumn() - 1)
                        {
                            var nextSouthWest = matrixCells[rowNumber -1, columnNumber + 1];
                            if (nextSouthWest != null)
                            {
                                cell.setNext(Directions.SouthWest,nextSouthWest);
                                nextSouthWest.setNext(Directions.NorthEast,cell); 
                            }
                        }
                    }
                }
                    
                if (rowNumber >= 1)
                {
                    var nextSouth = matrixCells[rowNumber-1, columnNumber];
                    if (nextSouth != null)
                    {
                        cell.setNext(Directions.South,nextSouth);
                        nextSouth.setNext(Directions.North,cell); 
                    }
                }
   
                roomCells.Add(cell);
            }
        }
        return roomCells;
    }
    
    private void Awake()
    {
        _roomPrefab = Resources.Load<GameObject>("Prefabs/Room");
    }
}

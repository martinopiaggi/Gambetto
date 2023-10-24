using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gambetto.Scripts.Utils;
using Unity.Mathematics;
using UnityEngine.Serialization;

public class GridManager : MonoBehaviour
{
    private GameObject _roomPrefab;
    private int _lastColor = 1;
    private bool _changed = false;

    private List<List<Cell>> _grid;

    
    
    #region just_for_testing
    
    public GameObject prefabTest;
    public bool north = false;
    public bool east = false;
    public bool west = false;
    public bool south = false;
    public bool northEast = false;
    public bool northWest = false;
    public bool southEast = false;
    public bool southWest = false; 
    
    public Cell currentCell = null;
    public GameObject pawnTest = null;
    
    public void Start()
    {
        pawnTest = Instantiate(prefabTest, new Vector3(0,0,0), quaternion.identity);
    }

    public void Update()
    {
        if (north || south || east || west || southWest || southEast || northEast || northWest)
        {
            if (currentCell == null) currentCell = _grid[0][0];

            if (north)
            {
                currentCell = currentCell.getNext(Directions.North);
                pawnTest.transform.position = currentCell.getGlobalCoordinates();
                north = false;
            }
            
            if (south)
            {
                currentCell = currentCell.getNext(Directions.South);
                pawnTest.transform.position = currentCell.getGlobalCoordinates();
                south  = false;
            }
            
            if (east)
            {
                currentCell = currentCell.getNext(Directions.East);
                pawnTest.transform.position = currentCell.getGlobalCoordinates();
                east  = false;
            }
            
            if (west)
            {
                currentCell = currentCell.getNext(Directions.West);
                pawnTest.transform.position = currentCell.getGlobalCoordinates();
                west  = false;
            }
            
            if (southWest)
            {
                currentCell = currentCell.getNext(Directions.SouthWest);
                pawnTest.transform.position = currentCell.getGlobalCoordinates();
                southWest  = false;
            }
            
            if (southEast)
            {
                currentCell = currentCell.getNext(Directions.SouthEast);
                pawnTest.transform.position = currentCell.getGlobalCoordinates();
                southEast  = false;
            }
            
            if (northEast)
            {
                currentCell = currentCell.getNext(Directions.NorthEast);
                pawnTest.transform.position = currentCell.getGlobalCoordinates();
                northEast  = false;
            }
            
            if (northWest)
            {
                currentCell = currentCell.getNext(Directions.NorthWest);
                pawnTest.transform.position = currentCell.getGlobalCoordinates();
                northWest  = false;
            }
        }
    }
    
    #endregion
    
    
    public void CreateGrid(List<RoomLayout> roomLayouts)
    {
        _grid = new List<List<Cell>>();
        Vector3 translation = new Vector3(0,0,0);
        
        for (var roomIdx = 0; roomIdx < roomLayouts.Count; roomIdx++)
        {
            var roomLayout = roomLayouts[roomIdx];

            RoomLayout previousRoomLayout = null;
            if(roomIdx!=0) previousRoomLayout = roomLayouts[roomIdx-1];
            
            _grid.Add(PopulateRoomGraph(roomLayout,translation,roomIdx,previousRoomLayout));
            
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

        }
        Debug.Log("grid finished");
    }


    private List<Cell> cellBorder = new List<Cell>();

    private List<Cell> PopulateRoomGraph(RoomLayout roomLayout, Vector3 coordinateOrigin, int roomId, RoomLayout previousRoomLayout)
    {
        var borderDirection = roomLayout.GetExit(); //border direction of this room 
        List<Cell> currentCellBorder = new List<Cell>();
        
        //building first a temporary matrix to build easily the graph of cells
        var matrixCells = new Cell[roomLayout.GetSizeRow(), roomLayout.GetSizeColumn()];
        var roomCells = new List<Cell>();
        
        for(var rowNumber = 0; rowNumber < roomLayout.GetSizeRow();rowNumber++)
        {
            for (var columnNumber = 0; columnNumber < roomLayout.GetSizeColumn(); columnNumber++)
            {
                var square = roomLayout.GetRows()[rowNumber].GetColumns()[columnNumber];

                if (square == -1) //empty cell
                {
                    //even if there is no cell it's necessary to insert a null placeholder in the border to correctly update
                    // cell links at border in next room 
                    if(borderDirection==Directions.North) if(rowNumber == roomLayout.GetSizeRow()) currentCellBorder.Add(null);
                    if(borderDirection==Directions.South) if(rowNumber == 0) currentCellBorder.Add(null);
                    if(borderDirection==Directions.West) if(columnNumber == roomLayout.GetSizeColumn()) currentCellBorder.Add(null);
                    if(borderDirection==Directions.East) if(columnNumber == 0) currentCellBorder.Add(null);
                }
                else{ //not empty cell
                        
                    var cell = new Cell(coordinateOrigin + new Vector3(rowNumber, 0, columnNumber),roomId);
                    roomCells.Add(cell); //add cell to current room cells
                    
                    matrixCells[rowNumber, columnNumber] = cell; //temporary matrix as helper to update links between cells
                    
                    //add (eventually) this cell in the border list to update correctly the links in the next room cells population
                    if(borderDirection==Directions.North) if(rowNumber == roomLayout.GetSizeRow()-1) currentCellBorder.Add(cell);
                    if(borderDirection==Directions.South) if(rowNumber == 0) currentCellBorder.Add(cell);
                    if(borderDirection==Directions.West) if(columnNumber == roomLayout.GetSizeColumn()-1) currentCellBorder.Add(cell);
                    if(borderDirection==Directions.East) if(columnNumber == 0) currentCellBorder.Add(cell);
                    
                    
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
                    
                    //set all the neighbors links at BORDER updating also neighbors links in PREVIOUS ROOM

                    if (previousRoomLayout != null)
                    {
                        var borderCheckDirection = previousRoomLayout.GetExit() * -1; //opposite direction

                        if (borderCheckDirection == Directions.South)
                        {
                            if (rowNumber == 0 && columnNumber < cellBorder.Count)
                            {

                                var foreignCell = cellBorder[columnNumber];


                                if (foreignCell != null)
                                {
                                    cell.setNext(Directions.South, foreignCell);
                                    foreignCell.setNext(Directions.North, cell);
                                }

                                if (columnNumber - 1 > 0)
                                {
                                    foreignCell = cellBorder[columnNumber - 1];
                                    cell.setNext(Directions.SouthEast, foreignCell);
                                    foreignCell.setNext(Directions.NorthWest, cell);
                                }

                                if (columnNumber + 1 < cellBorder.Count)
                                {
                                    foreignCell = cellBorder[columnNumber + 1];
                                    cell.setNext(Directions.SouthWest, foreignCell);
                                    foreignCell.setNext(Directions.NorthEast, cell);
                                }
                            }
                        }

                        if (borderCheckDirection == Directions.North)
                        {
                            if (rowNumber == roomLayout.GetSizeRow()-1 && columnNumber < cellBorder.Count)
                            {

                                var foreignCell = cellBorder[columnNumber];


                                if (foreignCell != null)
                                {
                                    cell.setNext(Directions.North, foreignCell);
                                    foreignCell.setNext(Directions.South, cell);
                                }

                                if (columnNumber - 1 > 0)
                                {
                                    foreignCell = cellBorder[columnNumber - 1];
                                    cell.setNext(Directions.NorthEast, foreignCell);
                                    foreignCell.setNext(Directions.SouthWest, cell);
                                }

                                if (columnNumber + 1 < cellBorder.Count)
                                {
                                    foreignCell = cellBorder[columnNumber + 1];
                                    cell.setNext(Directions.NorthWest, foreignCell);
                                    foreignCell.setNext(Directions.SouthEast, cell);
                                }
                            }
                        }
                        
                        if (borderCheckDirection == Directions.East)
                        {
                            if (columnNumber == 0 && rowNumber < cellBorder.Count + 1)
                            {

                                if (rowNumber < cellBorder.Count)
                                {
                                    var foreignCell = cellBorder[rowNumber];
                                
                                    if (foreignCell != null)
                                    {
                                        cell.setNext(Directions.East, foreignCell);
                                        foreignCell.setNext(Directions.West, cell);
                                    }

                                    if (rowNumber - 1 >= 0)
                                    {
                                        foreignCell = cellBorder[rowNumber - 1];
                                        cell.setNext(Directions.SouthEast, foreignCell);
                                        foreignCell.setNext(Directions.NorthWest, cell);
                                    }

                                    if (rowNumber + 1 < cellBorder.Count)
                                    {
                                        foreignCell = cellBorder[rowNumber + 1];
                                        cell.setNext(Directions.NorthEast, foreignCell);
                                        foreignCell.setNext(Directions.SouthWest, cell);
                                    }
                                }
                                else if (rowNumber == cellBorder.Count)
                                {
                                    if (rowNumber - 1 >= 0)
                                    {
                                        var foreignCell = cellBorder[rowNumber - 1];
                                        cell.setNext(Directions.SouthEast, foreignCell);
                                        foreignCell.setNext(Directions.NorthWest, cell);
                                    }
                                }
                            }
                        }

                        if (borderCheckDirection == Directions.West)
                        {
                            if (columnNumber == roomLayout.GetSizeColumn() - 1 && rowNumber < cellBorder.Count + 1)
                            {

                                if (rowNumber < cellBorder.Count)
                                {
                                    var foreignCell = cellBorder[rowNumber];


                                    if (foreignCell != null)
                                    {
                                        cell.setNext(Directions.West, foreignCell);
                                        foreignCell.setNext(Directions.East, cell);
                                    }

                                    if (rowNumber - 1 > 0)
                                    {
                                        foreignCell = cellBorder[rowNumber - 1];
                                        cell.setNext(Directions.SouthWest, foreignCell);
                                        foreignCell.setNext(Directions.NorthEast, cell);
                                    }

                                    if (rowNumber + 1 < cellBorder.Count)
                                    {
                                        foreignCell = cellBorder[rowNumber + 1];
                                        cell.setNext(Directions.NorthWest, foreignCell);
                                        foreignCell.setNext(Directions.SouthEast, cell);
                                    }
                                }
                                else if (rowNumber == cellBorder.Count)
                                {
                                    if (rowNumber - 1 >= 0)
                                    {
                                        var foreignCell = cellBorder[rowNumber - 1];
                                        cell.setNext(Directions.SouthWest, foreignCell);
                                        foreignCell.setNext(Directions.NorthEast, cell);
                                    }
                                }

                            }
                        }

                    }
                }

                
            }
        }


        cellBorder = currentCellBorder;
        return roomCells;
    }
    
    
    private void Awake()
    {
        _roomPrefab = Resources.Load<GameObject>("Prefabs/Room");
    }
}

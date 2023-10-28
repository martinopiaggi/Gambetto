using System.Collections.Generic;
using Gambetto.Scripts.Utils;
using Pieces;
using Unity.Mathematics;
using UnityEngine;

namespace Gambetto.Scripts
{
    public class GridManager : MonoBehaviour
    {
        private GameObject _roomPrefab;
        private int _lastColor = 1;
        private bool _changed = false;

        private readonly List<List<Cell>> _grid = new List<List<Cell>>();
        private Dictionary<Piece, Cell> _pieces = new Dictionary<Piece, Cell>();
    
        #region just_for_testing
        
        public GameObject prefabTest;
        private GameObject _spawnGameObject;
        public bool north = true;
        public bool east = false;
        public bool west = false;
        public bool south = false;
        public bool northEast = false;
        public bool northWest = false;
        public bool southEast = false;
        public bool southWest = false; 
    
        public Cell CurrentCell = null;
        public GameObject pawnTest = null;
        public List<Vector3> positions = null;
        public Piece pieceTry = null;
    
        public void Start()
        {
            pawnTest = Instantiate(prefabTest, new Vector3(0,0,0), quaternion.identity);
             pieceTry = pawnTest.GetComponent<Piece>();
        }

        public void Update()
        {
            if (north || south || east || west || southWest || southEast || northEast || northWest)
            {
                if (CurrentCell == null) CurrentCell = _grid[0][0];

                if (north)
                {
                    var next = CurrentCell.getNext(Directions.North);
                    if (next != null)
                    {
                        CurrentCell = next;
                        pawnTest.transform.position = CurrentCell.getGlobalCoordinates();
                        north = false;
                    }
                    else
                    {
                        Debug.Log("ERROR");
                    }
                }

                if (south)
                {
                    var next = CurrentCell.getNext(Directions.South);
                    if (next != null)
                    {
                        CurrentCell = next;
                        pawnTest.transform.position = CurrentCell.getGlobalCoordinates();
                        south = false;
                    }
                    else
                    {
                        Debug.Log("ERROR");
                    }
                }

                if (east)
                {
                    var next = CurrentCell.getNext(Directions.East);
                    if (next != null)
                    {
                        CurrentCell = next;
                        pawnTest.transform.position = CurrentCell.getGlobalCoordinates();
                        east = false;
                    }
                    else
                    {
                        Debug.Log("ERROR");
                    }
                }

                if (west)
                {
                    var next = CurrentCell.getNext(Directions.West);
                    if (next != null)
                    {
                        CurrentCell = next;
                        pawnTest.transform.position = CurrentCell.getGlobalCoordinates();
                        west = false;
                    }
                    else
                    {
                        Debug.Log("ERROR");
                    }
                }

                if (southWest)
                {
                    var next = CurrentCell.getNext(Directions.SouthWest);
                    if (next != null)
                    {
                        CurrentCell = next;
                        pawnTest.transform.position = CurrentCell.getGlobalCoordinates();
                        southWest = false;
                    }
                    else
                    {
                        Debug.Log("ERROR");
                    }
                }

                if (southEast)
                {
                    var next = CurrentCell.getNext(Directions.SouthEast);
                    if (next != null)
                    {
                        CurrentCell = next;
                        pawnTest.transform.position = CurrentCell.getGlobalCoordinates();
                        southEast = false;
                    }
                    else
                    {
                        Debug.Log("ERROR");
                    }
                }

                if (northEast)
                {
                    var next = CurrentCell.getNext(Directions.NorthEast);
                    if (next != null)
                    {
                        CurrentCell = next;
                        pawnTest.transform.position = CurrentCell.getGlobalCoordinates();
                        northEast = false;
                    }
                    else
                    {
                        Debug.Log("ERROR");
                    }
                }

                if (northWest)
                {
                    var next = CurrentCell.getNext(Directions.NorthWest);
                    if (next != null)
                    {
                        CurrentCell = next;
                        pawnTest.transform.position = CurrentCell.getGlobalCoordinates();
                        northWest = false;
                    }
                    else
                    {
                        Debug.Log("ERROR");
                    }
                }
            }

            pawnTest.transform.position = new Vector3(0, 0, 0);
            CurrentCell = _grid[0][0];

            positions = GetPossibleMovements(pieceTry, CurrentCell);
            foreach (Vector3 position in positions)
            {
                //_spawnGameObject = Instantiate(pawnPrefab, transform.position, Quaternion.identity);
                Debug.Log(position);
            }
        }

        #endregion
        
        public List<Vector3> GetPossibleMovements(Piece player, Cell currentPosition)
        {
            List<Vector3> possibleMovement = new List<Vector3>();
            Cell startingCell = currentPosition;
            Cell tempCell = startingCell;
            List<Vector2> directions = player.PossibleMoves;
            switch (player.PieceType)
            {
                case PieceType.Bishop:
                    //Debug.Log("bishop");
                    foreach (Vector2 direction in directions)
                    {
                        tempCell = startingCell;
                        while (tempCell != null)
                        {
                            tempCell = tempCell.getNext(direction);
                            if (tempCell != null)
                            {
                                possibleMovement.Add(tempCell.getGlobalCoordinates());
                            }
                        }
                    }
                    break;
                case PieceType.King:
                    //Debug.Log("king");
                    foreach (Vector2 direction in directions)
                    {
                        tempCell = startingCell;
                        tempCell = tempCell.getNext(direction);
                        if (tempCell != null)
                        {
                            possibleMovement.Add(tempCell.getGlobalCoordinates());
                        }
                    }
                    break;
                case PieceType.Knight:
                    //Debug.Log("knight");
                    int i = 0;
                    while(i < directions.Count)
                    {
                        tempCell = startingCell;
                        for (int j = 0; j < 3; j++)
                        {
                            tempCell = tempCell.getNext(directions[i+j]);
                            if (tempCell == null) break;
                        }
                        i = i + 3;
                        if (tempCell != null) possibleMovement.Add(tempCell.getGlobalCoordinates());
                    }
                    break;
                    /*
                    foreach (Vector2 direction in directions)
                    {   
                        tempCell = startingCell;
                        tempCell = tempCell.getNext(Directions.North);
                        tempCell = tempCell.getNext(Directions.North);
                        tempCell = tempCell.getNext(Directions.East);
                        if (tempCell != null) possibleMovement.Add(tempCell.getGlobalCoordinates());
                        
                        tempCell = startingCell;
                        tempCell = tempCell.getNext(Directions.North);
                        tempCell = tempCell.getNext(Directions.North);
                        tempCell = tempCell.getNext(Directions.West);
                        if (tempCell != null) possibleMovement.Add(tempCell.getGlobalCoordinates());
                        
                        tempCell = startingCell;
                        tempCell = tempCell.getNext(Directions.South);
                        tempCell = tempCell.getNext(Directions.South);
                        tempCell = tempCell.getNext(Directions.East);
                        if (tempCell != null) possibleMovement.Add(tempCell.getGlobalCoordinates());
                        
                        tempCell = startingCell;
                        tempCell = tempCell.getNext(Directions.South);
                        tempCell = tempCell.getNext(Directions.South);
                        tempCell = tempCell.getNext(Directions.West);
                        if (tempCell != null) possibleMovement.Add(tempCell.getGlobalCoordinates());
                        
                        tempCell = startingCell;
                        tempCell = tempCell.getNext(Directions.East);
                        tempCell = tempCell.getNext(Directions.East);
                        tempCell = tempCell.getNext(Directions.North);
                        if (tempCell != null) possibleMovement.Add(tempCell.getGlobalCoordinates());
                        
                        tempCell = startingCell;
                        tempCell = tempCell.getNext(Directions.East);
                        tempCell = tempCell.getNext(Directions.East);
                        tempCell = tempCell.getNext(Directions.South);
                        if (tempCell != null) possibleMovement.Add(tempCell.getGlobalCoordinates());
                        
                        tempCell = startingCell;
                        tempCell = tempCell.getNext(Directions.West);
                        tempCell = tempCell.getNext(Directions.West);
                        tempCell = tempCell.getNext(Directions.North);
                        if (tempCell != null) possibleMovement.Add(tempCell.getGlobalCoordinates());
                        
                        tempCell = startingCell;
                        tempCell = tempCell.getNext(Directions.West);
                        tempCell = tempCell.getNext(Directions.West);
                        tempCell = tempCell.getNext(Directions.South);
                        if (tempCell != null) possibleMovement.Add(tempCell.getGlobalCoordinates());
                        
                    }
                    break;
                    */
                case PieceType.Pawn:
                    //Debug.Log("pawn");
                    foreach (Vector2 direction in directions)
                    {
                        tempCell = startingCell;
                        tempCell = tempCell.getNext(direction);
                        if (tempCell != null)
                        {
                            possibleMovement.Add(tempCell.getGlobalCoordinates());
                        }
                    }

                    break;
                case PieceType.Queen:
                    //Debug.Log("queen");
                    foreach (Vector2 direction in directions)
                    {
                        tempCell = startingCell;
                        while (tempCell != null)
                        {
                            tempCell = tempCell.getNext(direction);
                            if (tempCell != null)
                            {
                                possibleMovement.Add(tempCell.getGlobalCoordinates());
                            }
                        }
                    }
                    break;
                case PieceType.Rook:
                    //Debug.Log("rook");
                    foreach (Vector2 direction in directions)
                    {
                        tempCell = startingCell;
                        while (tempCell != null)
                        {
                            tempCell = tempCell.getNext(direction);
                            if (tempCell != null)
                            {
                                possibleMovement.Add(tempCell.getGlobalCoordinates());
                            }
                        }
                    }
                    break;
                default:
                    Debug.Log("Invalid type of piece");
                    break;
            }

            return possibleMovement;
        }
    
    
        public void CreateGrid(List<RoomLayout> roomLayouts)
        {
            var translation = new Vector3(0,0,0);
        
            for (var roomIdx = 0; roomIdx < roomLayouts.Count; roomIdx++)
            {
                var roomLayout = roomLayouts[roomIdx];

                RoomLayout previousRoomLayout = null;
                if(roomIdx!=0) previousRoomLayout = roomLayouts[roomIdx-1];
            
                _grid.Add(PopulateRoomGraph(roomLayout,translation,roomIdx,previousRoomLayout));
            
                var roomObj = Instantiate(_roomPrefab,  transform.position, Quaternion.identity);
            
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
            
            
                //change the translation of the next room according to the exit of the previous room
                if (roomLayout.GetExit() != Directions.South && roomLayout.GetExit() != Directions.East)
                {
                    translation = translation + new Vector3(roomLayout.GetExit().x*roomLayout.GetSizeRow(), 
                        0,
                        roomLayout.GetExit().y*roomLayout.GetSizeColumn());
                }
                else
                {
                    //we have to compute the correct translation considering **next** roomLayout size in case of South/East
                
                    if(roomIdx!=(roomLayouts.Count-1)){
                        var nextRoomLayout = roomLayouts[roomIdx+1];
                        translation = translation + new Vector3(roomLayout.GetExit().x*nextRoomLayout.GetSizeRow(), 
                            0,
                            roomLayout.GetExit().y*nextRoomLayout.GetSizeColumn());
                    }
                }
            
            
                //this if determine what is the last color of the room, 1 (blue), 0 (white)
                //if the room has an even lenght and was not changed its last color is 1 (blue)
                //if the room has an odd lenght and was not changed its last color is 0 (white)
                if (roomLayout.GetExit() == Directions.East || roomLayout.GetExit() == Directions.West)
                {
                    if (roomLayout.GetSizeColumn() % 2 == 0)
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
                else
                {
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
            }
            Debug.Log("grid finished");
        }


        private List<Cell> _cellBorder = new List<Cell>();
    
        private List<Cell> PopulateRoomGraph(RoomLayout roomLayout, Vector3 coordinateOrigin, int roomId, RoomLayout previousRoomLayout)
        {
            var borderDirection = roomLayout.GetExit(); //border direction of this room 
            var currentCellBorder = new List<Cell>();
        
            //building first a temporary matrix to build easily the graph of cells
            var matrixCells = new Cell[roomLayout.GetSizeRow(), roomLayout.GetSizeColumn()];
            var roomCells = new List<Cell>();
        
            for(var rowNumber = 0; rowNumber < roomLayout.GetSizeRow(); rowNumber++)
            {
                for (var columnNumber = 0; columnNumber < roomLayout.GetSizeColumn(); columnNumber++)
                {
                    var square = roomLayout.GetRows()[rowNumber].GetColumns()[columnNumber];

                    if (square == -1) //empty cell
                    {
                        //even if there is no cell it's necessary to insert a null placeholder in the border to correctly update
                        // cell links at border in next room 
                        AddNullPlaceholder(rowNumber, columnNumber, roomLayout, currentCellBorder, borderDirection);
                    }
                    else{ //not empty cell
                        var cell = CreateCell(coordinateOrigin, roomLayout.GetExit(), rowNumber, columnNumber, roomId, roomLayout,
                            currentCellBorder);
                        roomCells.Add(cell); //add cell to current room cells
                        matrixCells[rowNumber, columnNumber] = cell; //temporary matrix as helper to update links between cells
                    
                        SolveLinksNeighbors(cell, rowNumber, columnNumber,matrixCells, roomLayout.GetSizeColumn());
                    
                        //set all the neighbors links at BORDER updating also neighbors links in PREVIOUS ROOM
                        if (roomId > 0) //check if it's not the first room.
                            SolveInterRoomConsistencies(cell, rowNumber, columnNumber, previousRoomLayout.GetExit() * -1,
                                _cellBorder, roomLayout.GetSizeRow(),roomLayout.GetSizeColumn());
                    }
                }
            }

            _cellBorder = currentCellBorder;
            return roomCells;
        }


        private static void AddNullPlaceholder(int rowNumber, int columnNumber, RoomLayout r, List<Cell> currentCellBorder, Vector2 borderDirection)
        {
            if(borderDirection==Directions.North) if(rowNumber == r.GetSizeRow()-1) currentCellBorder.Add(null);
            if(borderDirection==Directions.South) if(rowNumber == 0) currentCellBorder.Add(null);
            if(borderDirection==Directions.West) if(columnNumber == r.GetSizeColumn()-1) currentCellBorder.Add(null);
            if (borderDirection != Directions.East) return;
            if(columnNumber == 0) currentCellBorder.Add(null);
        }

        private static Cell CreateCell(Vector3 coordinateOrigin, Vector2 borderDirection, int rowNumber, int columnNumber, int roomId, RoomLayout r, List<Cell> currentCellBorder)
        {
            var cell = new Cell(new Vector2(coordinateOrigin.x,coordinateOrigin.z) + new Vector2(rowNumber, columnNumber),roomId);
                  
            //add (eventually) this cell in the border list to update correctly the links in the next room cells population
            if(borderDirection==Directions.North) if(rowNumber == r.GetSizeRow()-1) currentCellBorder.Add(cell);
            if(borderDirection==Directions.South) if(rowNumber == 0) currentCellBorder.Add(cell);
            if(borderDirection==Directions.West) if(columnNumber == r.GetSizeColumn()-1) currentCellBorder.Add(cell);
            if(borderDirection==Directions.East) if(columnNumber == 0) currentCellBorder.Add(cell);

            return cell;
        }
    
        private static void SolveLinksNeighbors(Cell cell, int rowNumber, int columnNumber, Cell[,] matrixCells,
            int roomColumnSize)
        {
        
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
                        
                if (columnNumber < roomColumnSize - 1)
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
    

        private void SolveInterRoomConsistencies(Cell cell, int rowNumber, int columnNumber, Vector2 borderCheckDirection,
            List<Cell> border, int roomRowsSize, int roomColumnsSize)
        {
            //the border direction answers: "from which direction respect to the cell in the current room, the previous room is?
            if (borderCheckDirection == Directions.South)
                if (rowNumber == 0 && columnNumber < _cellBorder.Count + 1)
                {
                    if (columnNumber < _cellBorder.Count)
                    {

                        var foreignCell = _cellBorder[columnNumber];

                        if (foreignCell != null)
                        {
                            cell.setNext(Directions.South, foreignCell);
                            foreignCell.setNext(Directions.North, cell);
                        }

                        if (columnNumber + 1 < _cellBorder.Count)
                        {
                            foreignCell = _cellBorder[columnNumber + 1];
                            if (foreignCell != null)
                            {
                                cell.setNext(Directions.SouthWest, foreignCell);
                                foreignCell.setNext(Directions.NorthEast, cell);
                            }
                        }

                        if (columnNumber - 1 >= 0)
                        {
                            foreignCell = _cellBorder[columnNumber - 1];
                            if (foreignCell != null)
                            {
                                cell.setNext(Directions.SouthEast, foreignCell);
                                foreignCell.setNext(Directions.NorthWest, cell);
                            }
                        }
                    }

                    if (columnNumber - 1 >= 0)
                    {
                        var foreignCell = _cellBorder[columnNumber - 1];
                        if (foreignCell != null)
                        {
                            cell.setNext(Directions.SouthEast, foreignCell);
                            foreignCell.setNext(Directions.NorthWest, cell);
                        }
                    }
                }

            if (borderCheckDirection == Directions.North)
                if (rowNumber == roomRowsSize - 1 && columnNumber < _cellBorder.Count + 1)
                {

                    if (columnNumber < _cellBorder.Count)
                    {
                        var foreignCell = _cellBorder[columnNumber];
                        if (foreignCell != null)
                        {
                            cell.setNext(Directions.North, foreignCell);
                            foreignCell.setNext(Directions.South, cell);
                        }

                        if (columnNumber - 1 > 0)
                        {
                            foreignCell = _cellBorder[columnNumber - 1];
                            if (foreignCell != null)
                            {
                                cell.setNext(Directions.NorthEast, foreignCell);
                                foreignCell.setNext(Directions.SouthWest, cell);
                            }
                        }

                        if (columnNumber + 1 < _cellBorder.Count)
                        {
                            foreignCell = _cellBorder[columnNumber + 1];
                            if (foreignCell != null)
                            {
                                cell.setNext(Directions.NorthWest, foreignCell);
                                foreignCell.setNext(Directions.SouthEast, cell);
                            }
                        }
                    }

                    if (columnNumber - 1 >= 0)
                    {
                        var foreignCell = _cellBorder[columnNumber - 1];
                        if (foreignCell != null)
                        {
                            cell.setNext(Directions.NorthEast, foreignCell);
                            foreignCell.setNext(Directions.SouthWest, cell);
                        }
                    }
                }

            if (borderCheckDirection == Directions.East && columnNumber == 0 && rowNumber < _cellBorder.Count + 1)
            {

                if (rowNumber < _cellBorder.Count)
                {
                    var foreignCell = _cellBorder[rowNumber];
                    if (foreignCell != null)
                    {
                        cell.setNext(Directions.East, foreignCell);
                        foreignCell.setNext(Directions.West, cell);
                    }

                    if (rowNumber - 1 >= 0)
                    {
                        foreignCell = _cellBorder[rowNumber - 1];
                        if (foreignCell != null)
                        {
                            cell.setNext(Directions.SouthEast, foreignCell);
                            foreignCell.setNext(Directions.NorthWest, cell);
                        }
                    }

                    if (rowNumber + 1 < _cellBorder.Count)
                    {
                        foreignCell = _cellBorder[rowNumber + 1];
                        if (foreignCell != null)
                        {
                            cell.setNext(Directions.NorthEast, foreignCell);
                            foreignCell.setNext(Directions.SouthWest, cell);
                        }
                    }
                }
                                
                if (rowNumber - 1 >= 0)
                {
                    var foreignCell = _cellBorder[rowNumber - 1];
                    if (foreignCell != null)
                    {
                        cell.setNext(Directions.SouthEast, foreignCell);
                        foreignCell.setNext(Directions.NorthWest, cell);
                    }
                }
            }

            if (borderCheckDirection != Directions.West) return;
            {
                if (columnNumber != roomColumnsSize - 1 || rowNumber >= _cellBorder.Count + 1) return;
                if (rowNumber < _cellBorder.Count)
                {
                    var foreignCell = _cellBorder[rowNumber];
                
                    if (foreignCell != null)
                    {
                        cell.setNext(Directions.West, foreignCell);
                        foreignCell.setNext(Directions.East, cell);
                    }

                    if (rowNumber - 1 > 0)
                    {
                        foreignCell = _cellBorder[rowNumber - 1];
                        if (foreignCell != null)
                        {
                            cell.setNext(Directions.SouthWest, foreignCell);
                            foreignCell.setNext(Directions.NorthEast, cell);
                        }
                    }

                    if (rowNumber + 1 < _cellBorder.Count)
                    {
                        foreignCell = _cellBorder[rowNumber + 1];
                        if (foreignCell != null)
                        {
                            cell.setNext(Directions.NorthWest, foreignCell);
                            foreignCell.setNext(Directions.SouthEast, cell);
                        }
                    }
                }

                if (rowNumber - 1 < 0) return;
                {
                    var foreignCell = _cellBorder[rowNumber - 1];
                    if (foreignCell != null)
                    {
                        cell.setNext(Directions.SouthWest, foreignCell);
                        foreignCell.setNext(Directions.NorthEast, cell);
                    }
                }
            }
        }
    
    
        private void Awake()
        {
            _roomPrefab = Resources.Load<GameObject>("Prefabs/Room");
        }
    }
}

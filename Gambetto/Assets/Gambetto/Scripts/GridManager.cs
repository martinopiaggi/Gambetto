using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts.Pieces;
using Gambetto.Scripts.Utils;
using Pieces;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gambetto.Scripts
{
    public class GridManager : MonoBehaviour
    {
        private GameObject _roomPrefab;
        private int _lastColor = 1;
        private bool _changed = false;
        [FormerlySerializedAs("_playerControllerGambetto")] public PlayerController playerController; 

        private readonly List<List<Cell>> _grid = new List<List<Cell>>();
        private Dictionary<Piece, Cell> _pieces = new Dictionary<Piece, Cell>();
        
        public GameObject prefabTest;
        private GameObject _spawnGameObject;

        private Cell _playerCell = null;
        public GameObject pawnTest = null;
        
        public Piece pieceTry = null;
        public bool gridFinished = false;

        public void Start()
        {
            pawnTest = Instantiate(prefabTest, new Vector3(1,0,1), quaternion.identity);
            pieceTry = pawnTest.GetComponent<Piece>();
            GameClock.Instance.ClockTick += OnClockTick;
            GameClock.Instance.StartClock();
        }

        /// <summary>
        /// Called every clock tick, it updates the player position and starts the choosing of the next move for each piece
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnClockTick(object source,ClockEventArgs args)
        {
            if (!gridFinished) return;
            if (playerController.ChosenMove == null)
            {
                _playerCell = _grid[0][10];
                playerController.ChosenMove = _playerCell;
            }
            // apply movements from the previous tick
            UpdatePiecesPosition();
            
            // Compute Cpu behaviour and Start the choosing animation for the player
            ComputeCpuBehaviour(_pieces); //todo: implement cpu behaviour
            playerController.StartChoosing(pieceTry, _playerCell);
        }

        private void ComputeCpuBehaviour(Dictionary<Piece, Cell> pieces)
        {
            //cpu behaviour stuff/
        }
        
        private void UpdatePiecesPosition()
        {
            // todo: fetch movements from cpubehaviour
            // for each piece in cpubehaviour move
            UpdatePlayerPosition();
        }
        
        private void UpdatePlayerPosition()
        {
            var nextCellPlayer = playerController.ChosenMove;
            //At the start of the level the player has not chosen a cell, it remains in the same position
            if(nextCellPlayer == null) return;
            _playerCell = nextCellPlayer;
            var list = new List<Vector3>();
            list.Add(nextCellPlayer.getGlobalCoordinates());
            pieceTry.Move(list);
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
                    roomObj.GetComponent<RoomBuilder>().setColorStart(0);
                    _changed = false;
                }
                else
                {
                    roomObj.GetComponent<RoomBuilder>().setColorStart(1);
                    _changed = true;
                }
                
                roomObj.GetComponent<RoomBuilder>().InitializeRoom(roomLayout);
                roomObj.transform.position = translation;
                
                //change the translation of the next room according to the exit of the previous room
                if (roomLayout.GetExit() != Directions.South && roomLayout.GetExit() != Directions.East)
                {
                    translation += new Vector3(roomLayout.GetExit().x*roomLayout.GetSizeRow(), 
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

                _lastColor = ColorConsistencyUpdate(roomLayout, _changed);
            }
            
            //Debug.Log("grid finished");
            gridFinished = true;
        }

        private int ColorConsistencyUpdate(RoomLayout roomLayout, bool changed)
        {
            //this if determine what is the last color of the room, 1 (dark), 0 (bright)
            //if the room has an even lenght and was not changed its last color is 1 (dark)
            //if the room has an odd lenght and was not changed its last color is 0 (bright)
            if (roomLayout.GetExit() == Directions.East || roomLayout.GetExit() == Directions.West)
            {
                if (roomLayout.GetSizeColumn() % 2 == 0) return changed ? 0 : 1;
                return changed ? 1 : 0;
            }

            if (roomLayout.GetSizeRow() % 2 != 0) return changed ? 1 : 0;
            
            return changed ? 0 : 1;
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

                    var cell = CreateCell(coordinateOrigin, roomLayout.GetExit(), rowNumber, columnNumber, roomId, roomLayout,
                            currentCellBorder);
                        if (square == -1) cell.setEmpty();
                        roomCells.Add(cell); //add cell to current room cells
                        matrixCells[rowNumber, columnNumber] = cell; //temporary matrix as helper to update links between cells
                    
                        SolveLinksNeighbors(cell, rowNumber, columnNumber,matrixCells, roomLayout.GetSizeColumn());
                    
                        //set all the neighbors links at BORDER updating also neighbors links in PREVIOUS ROOM
                        if (roomId > 0) //check if it's not the first room.
                            SolveInterRoomConsistencies(cell, rowNumber, columnNumber, previousRoomLayout.GetExit() * -1,
                                _cellBorder, roomLayout.GetSizeRow(),roomLayout.GetSizeColumn());
                    
                }
            }

            _cellBorder = currentCellBorder;
            return roomCells;
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
    

        private void SolveInterRoomConsistencies(Cell cell, int rowNumber, int columnNumber, Vector2Int borderCheckDirection,
            List<Cell> border, int roomRowsSize, int roomColumnsSize)
        {
            //the border direction answers: "from which direction respect to the cell in the current room, the previous room is?
            if (borderCheckDirection == Directions.South)
                if (rowNumber == 0 && columnNumber < _cellBorder.Count + 1)
                {
                    if (columnNumber < _cellBorder.Count)
                    {

                        var foreignCell = _cellBorder[columnNumber];
                        cell.setNext(Directions.South, foreignCell);
                        foreignCell.setNext(Directions.North, cell);
                        

                        if (columnNumber + 1 < _cellBorder.Count)
                        {
                            foreignCell = _cellBorder[columnNumber + 1];
                            cell.setNext(Directions.SouthWest, foreignCell);
                            foreignCell.setNext(Directions.NorthEast, cell);
                        }

                        if (columnNumber - 1 >= 0)
                        {
                            foreignCell = _cellBorder[columnNumber - 1];
                            cell.setNext(Directions.SouthEast, foreignCell);
                            foreignCell.setNext(Directions.NorthWest, cell);
                        }
                    }

                    if (columnNumber - 1 >= 0)
                    {
                        var foreignCell = _cellBorder[columnNumber - 1];
                        cell.setNext(Directions.SouthEast, foreignCell);
                        foreignCell.setNext(Directions.NorthWest, cell);
                    }
                }

            if (borderCheckDirection == Directions.North)
                if (rowNumber == roomRowsSize - 1 && columnNumber < _cellBorder.Count + 1)
                {

                    if (columnNumber < _cellBorder.Count)
                    {
                        var foreignCell = _cellBorder[columnNumber];
                        cell.setNext(Directions.North, foreignCell);
                        foreignCell.setNext(Directions.South, cell);
                        
                        if (columnNumber - 1 > 0)
                        {
                            foreignCell = _cellBorder[columnNumber - 1];
                            cell.setNext(Directions.NorthEast, foreignCell);
                            foreignCell.setNext(Directions.SouthWest, cell);
                        }

                        if (columnNumber + 1 < _cellBorder.Count)
                        {
                            foreignCell = _cellBorder[columnNumber + 1];
                            cell.setNext(Directions.NorthWest, foreignCell);
                            foreignCell.setNext(Directions.SouthEast, cell);
                        }
                    }

                    if (columnNumber - 1 >= 0)
                    {
                        var foreignCell = _cellBorder[columnNumber - 1];
                        cell.setNext(Directions.NorthEast, foreignCell);
                        foreignCell.setNext(Directions.SouthWest, cell);
                    }
                }

            if (borderCheckDirection == Directions.East && columnNumber == 0 && rowNumber < _cellBorder.Count + 1)
            {

                if (rowNumber < _cellBorder.Count)
                {
                    var foreignCell = _cellBorder[rowNumber];
                    cell.setNext(Directions.East, foreignCell);
                    foreignCell.setNext(Directions.West, cell);

                    if (rowNumber - 1 >= 0)
                    {
                        foreignCell = _cellBorder[rowNumber - 1];
                        cell.setNext(Directions.SouthEast, foreignCell);
                        foreignCell.setNext(Directions.NorthWest, cell);
                    }

                    if (rowNumber + 1 < _cellBorder.Count)
                    {
                        foreignCell = _cellBorder[rowNumber + 1];
                        cell.setNext(Directions.NorthEast, foreignCell);
                        foreignCell.setNext(Directions.SouthWest, cell);
                    }
                }
                                
                if (rowNumber - 1 >= 0)
                {
                    var foreignCell = _cellBorder[rowNumber - 1];
                    cell.setNext(Directions.SouthEast, foreignCell);
                    foreignCell.setNext(Directions.NorthWest, cell);
                }
            }

            if (borderCheckDirection != Directions.West) return;
            {
                if (columnNumber != roomColumnsSize - 1 || rowNumber >= _cellBorder.Count + 1) return;
                if (rowNumber < _cellBorder.Count)
                {
                    var foreignCell = _cellBorder[rowNumber];
                    cell.setNext(Directions.West, foreignCell);
                    foreignCell.setNext(Directions.East, cell);
                    

                    if (rowNumber - 1 > 0)
                    {
                        foreignCell = _cellBorder[rowNumber - 1];
                        cell.setNext(Directions.SouthWest, foreignCell);
                        foreignCell.setNext(Directions.NorthEast, cell);
                    }

                    if (rowNumber + 1 < _cellBorder.Count)
                    {
                        foreignCell = _cellBorder[rowNumber + 1];
                        cell.setNext(Directions.NorthWest, foreignCell);
                        foreignCell.setNext(Directions.SouthEast, cell);
                    }
                }

                if (rowNumber - 1 < 0) return;
                {
                    var foreignCell = _cellBorder[rowNumber - 1];
                    cell.setNext(Directions.SouthWest, foreignCell);
                    foreignCell.setNext(Directions.NorthEast, cell);
                }
            }
        }
    
    
        private void Awake()
        {
            _roomPrefab = Resources.Load<GameObject>("Prefabs/Room");
        }
    }
}

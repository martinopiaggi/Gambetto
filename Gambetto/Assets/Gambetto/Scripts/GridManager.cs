using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Gambetto.Scripts.Pieces;
using Gambetto.Scripts.Utils;
using Pieces;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Gambetto.Scripts
{
    public class GridManager : MonoBehaviour
    {
        private GameObject _roomPrefab;
        private int _lastColor = 1;
        private bool _changed = false;
        public PlayerController playerController;
        public CPUBehavior CPUBehavior;

        private readonly List<List<Cell>> _grid = new List<List<Cell>>(); //maybe we can remove _grid? (never used)
        private Dictionary<Piece, Cell> _enemies = new Dictionary<Piece, Cell>();
        private Dictionary<Piece, Cell> _initialEnemiesPositions = new Dictionary<Piece, Cell>();
        private Dictionary<PieceType, Cell> powerUps = new Dictionary<PieceType, Cell>();

        [SerializeField] public GameObject prefabPawn;

        [SerializeField] public GameObject prefabBishop;

        [SerializeField] public GameObject prefabKnight;

        [SerializeField] public GameObject prefabRook;

        [SerializeField] public GameObject prefabKing;

        [SerializeField] public GameObject prefabQueen;

        [SerializeField] public GameObject knightPowerUp;

        [SerializeField] public GameObject rookPowerUp;

        [SerializeField] public GameObject bishopPowerUp;

        [SerializeField] public GameObject queenPowerUp;

        [SerializeField] public GameObject endLevel;

        public Material lightMaterial;
        public Material darkMaterial;

        public bool isDead = false;

        private GameObject _spawnGameObject;

        private Cell _playerCell = null;
        private Cell _initialplayerCell = null;
        private Cell _endLevelCell = null;
        private Piece _playerPiece = null;
        private List<Vector3> _playerPath = null;

        private bool _gridFinished = false;

        public void Start()
        {
            GameClock.Instance.ClockTick += OnClockTick;
            GameClock.Instance.StartClock();
        }

        /// <summary>
        /// Called every clock tick, it updates the player position and starts the choosing of the next move for each piece
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnClockTick(object source, ClockEventArgs args)
        {
            if (!_gridFinished)
                return;
            if (CPUBehavior.ChosenMoves.Count != _enemies.Count)
            {
                foreach (var enemy in _enemies)
                {
                    CPUBehavior.ChosenMoves[enemy.Key] = enemy.Value;
                }
            }

            if (playerController.ChosenMove == null)
            {
                playerController.ChosenMove = _playerCell;
            }

            // Compute Cpu behaviour and Start the choosing animation for the player
            if (!isDead)
            {
                UpdatePiecesPosition();
            } // apply movements from the previous tick

            if (_playerCell.isEmpty())
            {
                isDead = true;
                StartCoroutine(restartLevel());
            }

            if (!isDead)
            {
                CPUBehavior.StartComputing(_playerCell, _enemies);
                playerController.StartChoosing(_playerPiece, _playerCell);
            }
        }

        public Vector3 getPlayerPosition()
        {
            return _playerCell.getGlobalCoordinates();
        }

        public IEnumerator restartLevel()
        {
            GameClock.Instance.StopClock();
            _enemies.Clear();
            _enemies = new Dictionary<Piece, Cell>(_initialEnemiesPositions);

            _playerCell = _initialplayerCell;
            playerController.ChosenMove = null;
            CPUBehavior.ChosenMoves.Clear();

            yield return new WaitForSeconds(2f);

            foreach (var enemy in _enemies)
            {
                MovePiece(enemy.Key, enemy.Value, false);
            }

            //MovePiece(_playerPiece, _playerCell);
            Destroy(_playerPiece.gameObject);
            var playerObj = Instantiate(
                prefabKnight,
                _playerCell.getGlobalCoordinates(),
                quaternion.identity
            );
            playerObj.GetComponent<MeshRenderer>().material = lightMaterial;
            _playerPiece = playerObj.GetComponent<Piece>();
            _playerPiece.PieceRole = PieceRole.Player;
            GameClock.Instance.StartClock();
            isDead = false;
            yield return null;
        }

        private void UpdatePiecesPosition()
        {
            _enemies = new Dictionary<Piece, Cell>(CPUBehavior.ChosenMoves);

            foreach (var enemy in _enemies)
            {
                MovePiece(enemy.Key, _enemies[enemy.Key]);
            }

            _playerCell = playerController.ChosenMove;
            _playerPath = playerController.MovePath;
            MovePiece(_playerPiece, _playerCell, _playerPath);
        }

        private void MovePiece(Piece piece, Cell nextCell, bool gravity = true)
        {
            //todo: this is a temporary fix pieces should always be in correct position
            if (Vector3.Distance(nextCell.getGlobalCoordinates(), piece.transform.position) < 0.1f)
                return;
            var list = new List<Vector3>();
            list.Add(nextCell.getGlobalCoordinates());
            piece.Move(list, gravity);
        }

        private void MovePiece(Piece piece, Cell nextCell, List<Vector3> path, bool gravity = true)
        {
            //todo: this is a temporary fix pieces should always be in correct position
            if (Vector3.Distance(nextCell.getGlobalCoordinates(), piece.transform.position) < 0.1f)
                return;
            piece.Move(path, gravity);
        }

        public void CreateGrid(List<RoomLayout> roomLayouts)
        {
            var translation = new Vector3(0, 0, 0);
            RoomLayout previousRoomLayout = null;
            
            for (var roomIdx = 0; roomIdx < roomLayouts.Count; roomIdx++)
            {
                var roomLayout = roomLayouts[roomIdx];

                var roomObj = Instantiate(_roomPrefab, translation, Quaternion.identity);

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
            
                if (roomIdx != 0) {
                    previousRoomLayout = roomLayouts[roomIdx - 1];

                    //change the translation of the current room
                    if (
                        previousRoomLayout.GetExit() != Directions.South
                        && previousRoomLayout.GetExit() != Directions.East
                    )
                    {
                        translation += new Vector3(
                            previousRoomLayout.GetExit().x * previousRoomLayout.GetSizeRow(),
                            0,
                            previousRoomLayout.GetExit().y * previousRoomLayout.GetSizeColumn()
                        );
                    }
                    else
                    {
                        //we have to compute the correct translation considering **this** roomLayout size in case of South/East
                        translation += new Vector3(
                            previousRoomLayout.GetExit().x * roomLayout.GetSizeRow(),
                            0,
                            previousRoomLayout.GetExit().y * roomLayout.GetSizeColumn());
                    }
                }
                
                roomObj.GetComponent<Transform>().SetPositionAndRotation(translation, Quaternion.identity);

                _grid.Add(PopulateRoomGraph(roomLayout, translation, roomIdx, previousRoomLayout));
                
                _lastColor = ColorConsistencyUpdate(roomLayout, _changed);
            }

            //Debug.Log("grid finished");
            _gridFinished = true;
        }

        private int ColorConsistencyUpdate(RoomLayout roomLayout, bool changed)
        {
            //this if determine what is the last color of the room, 1 (dark), 0 (bright)
            //if the room has an even lenght and was not changed its last color is 1 (dark)
            //if the room has an odd lenght and was not changed its last color is 0 (bright)
            if (roomLayout.GetExit() == Directions.East || roomLayout.GetExit() == Directions.West)
            {
                if (roomLayout.GetSizeColumn() % 2 == 0)
                    return changed ? 0 : 1;
                return changed ? 1 : 0;
            }

            if (roomLayout.GetSizeRow() % 2 != 0)
                return changed ? 1 : 0;

            return changed ? 0 : 1;
        }

        private List<Cell> _cellBorder = new List<Cell>();

        private List<Cell> PopulateRoomGraph(
            RoomLayout roomLayout,
            Vector3 coordinateOrigin,
            int roomId,
            RoomLayout previousRoomLayout
        )
        {
            var currentCellBorder = new List<Cell>();

            var roomSquares = roomLayout.Squares;
            
            //building first a temporary matrix to build easily the graph of cells
            var matrixCells = new Cell[roomLayout.GetSizeRow(), roomLayout.GetSizeColumn()];
            var roomCells = new List<Cell>();

            for (var i = 0; i < roomLayout.GetSizeRow(); i++)
            {
                for (var j = 0; j < roomLayout.GetSizeColumn(); j++)
                {
                    var square = roomSquares[i, j];

                    var cell = CreateCell(
                        coordinateOrigin,
                        roomLayout.GetExit(),
                        i,
                        j,
                        roomId,
                        roomLayout, //todo: maybe we can remove this parameter
                        currentCellBorder
                    );

                    if (square.Value == RoomLayout.MatrixValue.Empty)
                        cell.setEmpty();
                    else if (square.Value != RoomLayout.MatrixValue.Floor)
                    {
                        InstantiatePiece(cell, square);
                        InstantiateOther(cell, square);
                    }

                    roomCells.Add(cell); //add cell to current room cells
                    matrixCells[i, j] = cell; //temporary matrix as helper to update links between cells

                    SolveLinksNeighbors(
                        cell,
                        i,
                        j,
                        matrixCells,
                        roomLayout.GetSizeColumn()
                    );

                    //set all the neighbors links at BORDER updating also neighbors links in PREVIOUS ROOM
                    if (roomId > 0) //check if it's not the first room.
                        SolveInterRoomConsistencies(
                            cell,
                            i,
                            j,
                            previousRoomLayout.GetExit() * -1,
                            _cellBorder,
                            roomLayout.GetSizeRow(),
                            roomLayout.GetSizeColumn()
                        );
                }
            }

            _cellBorder = currentCellBorder;
            return roomCells;
        }

        private void InstantiatePiece(Cell cell, RoomLayout.Square square)
        {
            GameObject prefab = null;
            switch (square.Value)
            {
                case RoomLayout.MatrixValue.Spawn:
                    _playerCell = cell;
                    _initialplayerCell = cell;
                    var playerObj = Instantiate(
                        prefabKnight,
                        cell.getGlobalCoordinates(),
                        quaternion.identity
                    );
                    playerObj.GetComponent<MeshRenderer>().material = lightMaterial;
                    _playerPiece = playerObj.GetComponent<Piece>();
                    _playerPiece.PieceRole = PieceRole.Player;
                    return;
                case RoomLayout.MatrixValue.Pawn:
                    prefab = prefabPawn;
                    break;
                case RoomLayout.MatrixValue.Bishop:
                    prefab = prefabBishop;
                    break;
                case RoomLayout.MatrixValue.Knight:
                    prefab = prefabKnight;
                    break;
                case RoomLayout.MatrixValue.Rook:
                    prefab = prefabRook;
                    break;
                case RoomLayout.MatrixValue.Queen:
                    prefab = prefabQueen;
                    break;
                case RoomLayout.MatrixValue.King:
                    prefab = prefabKing;
                    break;
                default:
                    return;
            }


            var pieceObj = Instantiate(
                prefab,
                cell.getGlobalCoordinates(),
                quaternion.identity
            );
            var pieceScript = pieceObj.GetComponent<Piece>();
            pieceObj.tag = "Enemy"; // tag the enemy for collision detection
            pieceScript.PieceRole = PieceRole.Enemy;
            pieceObj.GetComponent<MeshRenderer>().material = darkMaterial;
            pieceObj.GetComponent<Rigidbody>().constraints =
                RigidbodyConstraints.FreezeRotation;
            _enemies.Add(pieceScript, cell);
            _initialEnemiesPositions.Add(pieceObj.GetComponent<Piece>(), cell);
        }


        private void InstantiateOther(Cell cell, RoomLayout.Square square)
        {
            switch (square.Value)
            {
                case RoomLayout.MatrixValue.PB: //Bishop power up
                    powerUps.Add(PieceType.Bishop, cell);
                    var bishopPowerUpObj = Instantiate(
                        bishopPowerUp,
                        cell.getGlobalCoordinates() + new Vector3(0, 0.05f, 0),
                        quaternion.identity
                    );
                    break;
                case RoomLayout.MatrixValue.PK: //Knight power up

                    powerUps.Add(PieceType.Knight, cell);
                    var knightPowerUpObj = Instantiate(
                        knightPowerUp,
                        cell.getGlobalCoordinates() + new Vector3(0, 0.05f, 0),
                        quaternion.identity
                    );
                    break;
                case RoomLayout.MatrixValue.PR: //Rook power up
                    powerUps.Add(PieceType.Rook, cell);
                    var rookPowerUpObj = Instantiate(
                        rookPowerUp,
                        cell.getGlobalCoordinates() + new Vector3(0, 0.05f, 0),
                        quaternion.identity
                    );
                    break;

                // case RoomLayout.MatrixValue.PB //Queen power up
                // :
                //     powerUps.Add(PieceType.Queen, cell);
                //     var queenPowerUpObj = Instantiate(
                //         queenPowerUp,
                //         cell.getGlobalCoordinates() + new Vector3(0, 0.05f, 0),
                //         quaternion.identity
                //     );
                //     break;

                case RoomLayout.MatrixValue.Exit: //End of level
                    _endLevelCell = cell;
                    var powerUpObj = Instantiate(
                        endLevel,
                        cell.getGlobalCoordinates() + new Vector3(0, 0.05f, 0),
                        quaternion.identity
                    );
                    break;
                default:
                    return;
            }
        }

        private static Cell CreateCell(
            Vector3 coordinateOrigin,
            Vector2 borderDirection,
            int rowNumber,
            int columnNumber,
            int roomId,
            RoomLayout r,
            List<Cell> currentCellBorder
        )
        {
            var cell = new Cell(
                new Vector2(coordinateOrigin.x, coordinateOrigin.z)
                + new Vector2(rowNumber, columnNumber),
                roomId
            );

            //add (eventually) this cell in the border list to update correctly the links in the next room cells population
            if (borderDirection == Directions.North)
                if (rowNumber == r.GetSizeRow() - 1)
                    currentCellBorder.Add(cell);
            if (borderDirection == Directions.South)
                if (rowNumber == 0)
                    currentCellBorder.Add(cell);
            if (borderDirection == Directions.West)
                if (columnNumber == r.GetSizeColumn() - 1)
                    currentCellBorder.Add(cell);
            if (borderDirection == Directions.East)
                if (columnNumber == 0)
                    currentCellBorder.Add(cell);

            return cell;
        }

        private static void SolveLinksNeighbors(
            Cell cell,
            int rowNumber,
            int columnNumber,
            Cell[,] matrixCells,
            int roomColumnSize
        )
        {
            //set all the neighbors links updating also neighbors links
            if (columnNumber >= 1)
            {
                var nextEast = matrixCells[rowNumber, columnNumber - 1];
                if (nextEast != null)
                {
                    cell.setNext(Directions.East, nextEast);
                    nextEast.setNext(Directions.West, cell);
                }

                if (rowNumber >= 1)
                {
                    var nextSouthEast = matrixCells[rowNumber - 1, columnNumber - 1];
                    if (nextSouthEast != null)
                    {
                        cell.setNext(Directions.SouthEast, nextSouthEast);
                        nextSouthEast.setNext(Directions.NorthWest, cell);
                    }
                }
            }

            if (rowNumber >= 1)
            {
                var nextSouth = matrixCells[rowNumber - 1, columnNumber];
                if (nextSouth != null)
                {
                    cell.setNext(Directions.South, nextSouth);
                    nextSouth.setNext(Directions.North, cell);
                }

                if (columnNumber < roomColumnSize - 1)
                {
                    var nextSouthWest = matrixCells[rowNumber - 1, columnNumber + 1];
                    if (nextSouthWest != null)
                    {
                        cell.setNext(Directions.SouthWest, nextSouthWest);
                        nextSouthWest.setNext(Directions.NorthEast, cell);
                    }
                }
            }
        }

        private void SolveInterRoomConsistencies(
            Cell cell,
            int rowNumber,
            int columnNumber,
            Vector2Int borderCheckDirection,
            List<Cell> border,
            int roomRowsSize,
            int roomColumnsSize
        )
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

            if (
                borderCheckDirection == Directions.East
                && columnNumber == 0
                && rowNumber < _cellBorder.Count + 1
            )
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

            if (borderCheckDirection != Directions.West)
                return;
            {
                if (columnNumber != roomColumnsSize - 1 || rowNumber >= _cellBorder.Count + 1)
                    return;
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

                if (rowNumber - 1 < 0)
                    return;
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
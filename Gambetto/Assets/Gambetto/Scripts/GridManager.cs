using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts.Pieces;
using Gambetto.Scripts.Utils;
using Pieces;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

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

        [SerializeField]
        public GameObject prefabPawn;

        [SerializeField]
        public GameObject prefabBishop;

        [SerializeField]
        public GameObject prefabKnight;

        [SerializeField]
        public GameObject prefabRook;

        [SerializeField]
        public GameObject prefabKing;

        [SerializeField]
        public GameObject prefabQueen;
        public Material lightMaterial;
        public Material darkMaterial;

        public bool isDead = false;

        private GameObject _spawnGameObject;

        private Cell _playerCell = null;
        private Cell _initialplayerCell = null;
        private Piece _playerPiece = null;

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
                UpdatePiecesPosition(); // apply movements from the previous tick

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

            //yield return new WaitForSeconds(GameClock.Instance.ClockPeriod + 0.5f);

            foreach (var enemy in _enemies)
            {
                MovePiece(enemy.Key, enemy.Value, false);
            }

            //MovePiece(_playerPiece, _playerCell);
            Destroy(_playerPiece.gameObject);
            var playerObj = Instantiate(
                prefabPawn,
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
            MovePiece(_playerPiece, _playerCell);
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

        public void CreateGrid(List<RoomLayout> roomLayouts)
        {
            var translation = new Vector3(0, 0, 0);

            for (var roomIdx = 0; roomIdx < roomLayouts.Count; roomIdx++)
            {
                var roomLayout = roomLayouts[roomIdx];

                RoomLayout previousRoomLayout = null;
                if (roomIdx != 0)
                    previousRoomLayout = roomLayouts[roomIdx - 1];

                _grid.Add(PopulateRoomGraph(roomLayout, translation, roomIdx, previousRoomLayout));

                var roomObj = Instantiate(_roomPrefab, transform.position, Quaternion.identity);

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
                if (
                    roomLayout.GetExit() != Directions.South
                    && roomLayout.GetExit() != Directions.East
                )
                {
                    translation += new Vector3(
                        roomLayout.GetExit().x * roomLayout.GetSizeRow(),
                        0,
                        roomLayout.GetExit().y * roomLayout.GetSizeColumn()
                    );
                }
                else
                {
                    //we have to compute the correct translation considering **next** roomLayout size in case of South/East
                    if (roomIdx != (roomLayouts.Count - 1))
                    {
                        var nextRoomLayout = roomLayouts[roomIdx + 1];
                        translation =
                            translation
                            + new Vector3(
                                roomLayout.GetExit().x * nextRoomLayout.GetSizeRow(),
                                0,
                                roomLayout.GetExit().y * nextRoomLayout.GetSizeColumn()
                            );
                    }
                }

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
            var borderDirection = roomLayout.GetExit(); //border direction of this room
            var currentCellBorder = new List<Cell>();

            //building first a temporary matrix to build easily the graph of cells
            var matrixCells = new Cell[roomLayout.GetSizeRow(), roomLayout.GetSizeColumn()];
            var roomCells = new List<Cell>();

            for (var rowNumber = 0; rowNumber < roomLayout.GetSizeRow(); rowNumber++)
            {
                for (
                    var columnNumber = 0;
                    columnNumber < roomLayout.GetSizeColumn();
                    columnNumber++
                )
                {
                    var square = roomLayout.GetRows()[rowNumber].GetColumns()[columnNumber];

                    var cell = CreateCell(
                        coordinateOrigin,
                        roomLayout.GetExit(),
                        rowNumber,
                        columnNumber,
                        roomId,
                        roomLayout,
                        currentCellBorder
                    );

                    if (square == -1)
                        cell.setEmpty();
                    else if (square != 0)
                        InstantiatePiece(cell, square);

                    roomCells.Add(cell); //add cell to current room cells
                    matrixCells[rowNumber, columnNumber] = cell; //temporary matrix as helper to update links between cells

                    SolveLinksNeighbors(
                        cell,
                        rowNumber,
                        columnNumber,
                        matrixCells,
                        roomLayout.GetSizeColumn()
                    );

                    //set all the neighbors links at BORDER updating also neighbors links in PREVIOUS ROOM
                    if (roomId > 0) //check if it's not the first room.
                        SolveInterRoomConsistencies(
                            cell,
                            rowNumber,
                            columnNumber,
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

        private void InstantiatePiece(Cell cell, int type)
        {
            if (type == 99) //it's the player
            {
                _playerCell = cell;
                _initialplayerCell = cell;
                var playerObj = Instantiate(
                    prefabBishop,
                    cell.getGlobalCoordinates(),
                    quaternion.identity
                );
                playerObj.GetComponent<MeshRenderer>().material = lightMaterial;
                _playerPiece = playerObj.GetComponent<Piece>();
                _playerPiece.PieceRole = PieceRole.Player;
            }
            else
            {
                var prefab = prefabPawn;
                switch (type)
                {
                    case 1:
                        prefab = prefabPawn;
                        break;
                    case 2:
                        prefab = prefabBishop;
                        break;
                    case 3:
                        prefab = prefabKnight;
                        break;
                    case 5:
                        prefab = prefabRook;
                        break;
                    case 8:
                        prefab = prefabQueen;
                        break;
                    case 9:
                        prefab = prefabKing;
                        break;
                }

                //todo instantiate enemy based on *type*
                var pieceObj = Instantiate(
                    prefab,
                    cell.getGlobalCoordinates(),
                    quaternion.identity
                );
                pieceObj.tag = "Enemy"; // tag the enemy for collision detection
                pieceObj.GetComponent<MeshRenderer>().material = darkMaterial;
                pieceObj.GetComponent<Rigidbody>().constraints =
                    RigidbodyConstraints.FreezeRotation;
                _enemies.Add(pieceObj.GetComponent<Piece>(), cell);
                _initialEnemiesPositions.Add(pieceObj.GetComponent<Piece>(), cell);
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

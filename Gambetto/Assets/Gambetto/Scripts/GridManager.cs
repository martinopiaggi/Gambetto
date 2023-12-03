using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gambetto.Scripts.Pieces;
using Gambetto.Scripts.Utils;
using Unity.Mathematics;
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
        private bool _changed;
        public PlayerController playerController;

        [FormerlySerializedAs("CPUBehavior")]
        public CPUBehavior cpuBehavior;

        private readonly List<List<Cell>> _grid = new List<List<Cell>>(); //maybe we can remove _grid? (never used)
        private Dictionary<Piece, Cell> _enemies = new Dictionary<Piece, Cell>();
        private Dictionary<Piece, List<Vector3>> _enemiesPath =
            new Dictionary<Piece, List<Vector3>>();
        private Dictionary<Piece, Cell> _initialEnemiesPositions = new Dictionary<Piece, Cell>();
        private Dictionary<PowerUp, Cell> _powerUps = new Dictionary<PowerUp, Cell>();

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

        [SerializeField]
        public GameObject knightPowerUp;

        [SerializeField]
        public GameObject rookPowerUp;

        [SerializeField]
        public GameObject bishopPowerUp;

        [SerializeField]
        public GameObject queenPowerUp;

        [SerializeField]
        public GameObject endLevel;

        public Material lightMaterial;
        public Material darkMaterial;

        public bool isDead;

        private GameObject _spawnGameObject;

        private Cell _playerCell;
        private Cell _initialPlayerCell;
        private Cell _endLevelCell;
        private Piece _playerPiece;
        private List<Vector3> _playerPath;

        private bool _gridFinished;

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
            if (cpuBehavior.ChosenMoves.Count != _enemies.Count)
            {
                foreach (var enemy in _enemies)
                {
                    cpuBehavior.ChosenMoves[enemy.Key] = enemy.Value;
                    cpuBehavior.MovePaths[enemy.Key] = new List<Vector3>
                    {
                        enemy.Value.GetGlobalCoordinates()
                    };
                }
            }

            if (playerController.ChosenMove == null)
            {
                playerController.MovePath = new List<Vector3>
                {
                    _playerCell.GetGlobalCoordinates()
                };
                playerController.ChosenMove = _initialPlayerCell;
            }

            // Compute Cpu behaviour and Start the choosing animation for the player
            if (!isDead)
            {
                UpdatePlayerPosition();
            } // apply movements from the previous tick

            if (_playerCell.IsEmpty())
            {
                isDead = true;
                StartCoroutine(RestartLevel());
            }

            if (!isDead)
            {
                cpuBehavior.ComputeCPUMoves(_playerCell, _enemies);
                UpdateEnemiesPosition();
                CheckPowerUp();
                playerController.StartChoosing(_playerPiece, _playerCell);
            }
        }

        public Vector3 GetPlayerPosition()
        {
            return _playerCell.GetGlobalCoordinates();
        }

        public IEnumerator RestartLevel()
        {
            GameClock.Instance.StopClock();
            _enemies.Clear();
            _enemies = new Dictionary<Piece, Cell>(_initialEnemiesPositions);

            _playerCell = _initialPlayerCell;

            playerController.ResetController();
            cpuBehavior.ChosenMoves.Clear();

            yield return new WaitForSeconds(1f);

            foreach (var enemy in _enemies)
            {
                MovePiece(
                    enemy.Key,
                    new List<Vector3> { enemy.Value.GetGlobalCoordinates() },
                    false
                );
            }

            //MovePiece(_playerPiece, _playerCell);
            Destroy(_playerPiece.gameObject);
            var playerObj = Instantiate(
                prefabPawn,
                _playerCell.GetGlobalCoordinates(),
                quaternion.identity
            );
            playerObj.GetComponent<MeshRenderer>().material = lightMaterial;
            _playerPiece = playerObj.GetComponent<Piece>();
            _playerPiece.PieceRole = PieceRole.Player;
            ResetPowerUps();
            GameClock.Instance.StartClock();
            isDead = false;
            yield return null;
        }

        private void UpdateEnemiesPosition()
        {
            _enemies = new Dictionary<Piece, Cell>(cpuBehavior.ChosenMoves);
            _enemiesPath = new Dictionary<Piece, List<Vector3>>(cpuBehavior.MovePaths);

            foreach (var enemy in _enemies)
            {
                MovePiece(enemy.Key, _enemiesPath[enemy.Key]);
            }
        }

        private void UpdatePlayerPosition()
        {
            _playerCell = playerController.ChosenMove;
            _playerPath = playerController.MovePath;
            MovePiece(_playerPiece, _playerPath);
        }

        private void MovePiece(Piece piece, List<Vector3> path, bool gravity = true)
        {
            // Check if the piece is already in the destination
            if (Vector3.Distance(piece.transform.position, path[^1]) < 0.1f)
                return;
            piece.Move(path, gravity);
        }

        public void CreateGrid(List<RoomLayout> roomLayouts)
        {
            var translation = new Vector3(0, 0, 0);
            RoomLayout previousRoomLayout = null;

            // for each room in the level
            for (var roomIdx = 0; roomIdx < roomLayouts.Count; roomIdx++)
            {
                var roomLayout = roomLayouts[roomIdx];

                roomLayout.LoadRoomData();

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
                // Compute the translation of the current room considering the previous room exit
                if (roomIdx != 0)
                {
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
                            previousRoomLayout.GetExit().y * roomLayout.GetSizeColumn()
                        );
                    }
                }

                roomObj
                    .GetComponent<Transform>()
                    .SetPositionAndRotation(translation, Quaternion.identity);

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
                        cell.SetEmpty();
                    else if (square.Value != RoomLayout.MatrixValue.Floor)
                    {
                        Behaviour behaviour = null;
                        if (square.Identifier != 0)
                        {
                            behaviour = roomLayout.Behaviours.Find(b => b.Id == square.Identifier);
                            //behaviour = roomLayout.Behaviours[square.Identifier];
                        }
                        InstantiatePiece(cell, square, behaviour);
                        InstantiateOther(cell, square);
                    }

                    roomCells.Add(cell); //add cell to current room cells
                    matrixCells[i, j] = cell; //temporary matrix as helper to update links between cells

                    SolveLinksNeighbors(cell, i, j, matrixCells, roomLayout.GetSizeColumn());

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

        private void InstantiatePiece(Cell cell, RoomLayout.Square square, Behaviour behaviour)
        {
            GameObject prefab;

            switch (square.Value)
            {
                case RoomLayout.MatrixValue.Spawn:
                    _playerCell = cell;
                    _initialPlayerCell = cell;
                    var playerObj = Instantiate(
                        prefabPawn,
                        cell.GetGlobalCoordinates(),
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

            var pieceObj = Instantiate(prefab, cell.GetGlobalCoordinates(), quaternion.identity);
            var pieceScript = pieceObj.GetComponent<Piece>();
            pieceObj.tag = "Enemy"; // tag the enemy for collision detection
            pieceScript.PieceRole = PieceRole.Enemy;

            if (square.Identifier != 0)
            {
                pieceScript.Pattern = behaviour;
                pieceScript.PatternAI = true;
            }

            pieceObj.GetComponent<MeshRenderer>().material = darkMaterial;
            pieceObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            _enemies.Add(pieceScript, cell);
            _initialEnemiesPositions.Add(pieceObj.GetComponent<Piece>(), cell);
        }

        private void InstantiateOther(Cell cell, RoomLayout.Square square)
        {
            switch (square.Value)
            {
                case RoomLayout.MatrixValue.PB: //Bishop power up
                    var bishopPowerUpObj = Instantiate(
                        bishopPowerUp,
                        cell.GetGlobalCoordinates() + new Vector3(0, 0.05f, 0),
                        quaternion.identity
                    );
                    var bishop = new PowerUp(PieceType.Bishop, bishopPowerUpObj, cell);
                    _powerUps.Add(bishop, cell);
                    break;

                case RoomLayout.MatrixValue.PK: //Knight power up
                    var knightPowerUpObj = Instantiate(
                        knightPowerUp,
                        cell.GetGlobalCoordinates() + new Vector3(0, 0.05f, 0),
                        quaternion.identity
                    );
                    var knight = new PowerUp(PieceType.Knight, knightPowerUpObj, cell);
                    _powerUps.Add(knight, cell);
                    break;
                case RoomLayout.MatrixValue.PR: //Rook power up
                    var rookPowerUpObj = Instantiate(
                        rookPowerUp,
                        cell.GetGlobalCoordinates() + new Vector3(0, 0.05f, 0),
                        quaternion.identity
                    );
                    var rook = new PowerUp(PieceType.Rook, rookPowerUpObj, cell);
                    _powerUps.Add(rook, cell);
                    break;

                case RoomLayout.MatrixValue.Exit: //End of level
                    _endLevelCell = cell;
                    Instantiate(
                        endLevel,
                        cell.GetGlobalCoordinates() + new Vector3(0, 0.05f, 0),
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
                    cell.SetNext(Directions.East, nextEast);
                    nextEast.SetNext(Directions.West, cell);
                }

                if (rowNumber >= 1)
                {
                    var nextSouthEast = matrixCells[rowNumber - 1, columnNumber - 1];
                    if (nextSouthEast != null)
                    {
                        cell.SetNext(Directions.SouthEast, nextSouthEast);
                        nextSouthEast.SetNext(Directions.NorthWest, cell);
                    }
                }
            }

            if (rowNumber >= 1)
            {
                var nextSouth = matrixCells[rowNumber - 1, columnNumber];
                if (nextSouth != null)
                {
                    cell.SetNext(Directions.South, nextSouth);
                    nextSouth.SetNext(Directions.North, cell);
                }

                if (columnNumber < roomColumnSize - 1)
                {
                    var nextSouthWest = matrixCells[rowNumber - 1, columnNumber + 1];
                    if (nextSouthWest != null)
                    {
                        cell.SetNext(Directions.SouthWest, nextSouthWest);
                        nextSouthWest.SetNext(Directions.NorthEast, cell);
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
                        cell.SetNext(Directions.South, foreignCell);
                        foreignCell.SetNext(Directions.North, cell);

                        if (columnNumber + 1 < _cellBorder.Count)
                        {
                            foreignCell = _cellBorder[columnNumber + 1];
                            cell.SetNext(Directions.SouthWest, foreignCell);
                            foreignCell.SetNext(Directions.NorthEast, cell);
                        }

                        if (columnNumber - 1 >= 0)
                        {
                            foreignCell = _cellBorder[columnNumber - 1];
                            cell.SetNext(Directions.SouthEast, foreignCell);
                            foreignCell.SetNext(Directions.NorthWest, cell);
                        }
                    }

                    if (columnNumber - 1 >= 0)
                    {
                        var foreignCell = _cellBorder[columnNumber - 1];
                        cell.SetNext(Directions.SouthEast, foreignCell);
                        foreignCell.SetNext(Directions.NorthWest, cell);
                    }
                }

            if (borderCheckDirection == Directions.North)
                if (rowNumber == roomRowsSize - 1 && columnNumber < _cellBorder.Count + 1)
                {
                    if (columnNumber < _cellBorder.Count)
                    {
                        var foreignCell = _cellBorder[columnNumber];
                        cell.SetNext(Directions.North, foreignCell);
                        foreignCell.SetNext(Directions.South, cell);

                        if (columnNumber - 1 > 0)
                        {
                            foreignCell = _cellBorder[columnNumber - 1];
                            cell.SetNext(Directions.NorthEast, foreignCell);
                            foreignCell.SetNext(Directions.SouthWest, cell);
                        }

                        if (columnNumber + 1 < _cellBorder.Count)
                        {
                            foreignCell = _cellBorder[columnNumber + 1];
                            cell.SetNext(Directions.NorthWest, foreignCell);
                            foreignCell.SetNext(Directions.SouthEast, cell);
                        }
                    }

                    if (columnNumber - 1 >= 0)
                    {
                        var foreignCell = _cellBorder[columnNumber - 1];
                        cell.SetNext(Directions.NorthEast, foreignCell);
                        foreignCell.SetNext(Directions.SouthWest, cell);
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
                    cell.SetNext(Directions.East, foreignCell);
                    foreignCell.SetNext(Directions.West, cell);

                    if (rowNumber - 1 >= 0)
                    {
                        foreignCell = _cellBorder[rowNumber - 1];
                        cell.SetNext(Directions.SouthEast, foreignCell);
                        foreignCell.SetNext(Directions.NorthWest, cell);
                    }

                    if (rowNumber + 1 < _cellBorder.Count)
                    {
                        foreignCell = _cellBorder[rowNumber + 1];
                        cell.SetNext(Directions.NorthEast, foreignCell);
                        foreignCell.SetNext(Directions.SouthWest, cell);
                    }
                }

                if (rowNumber - 1 >= 0)
                {
                    var foreignCell = _cellBorder[rowNumber - 1];
                    cell.SetNext(Directions.SouthEast, foreignCell);
                    foreignCell.SetNext(Directions.NorthWest, cell);
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
                    cell.SetNext(Directions.West, foreignCell);
                    foreignCell.SetNext(Directions.East, cell);

                    if (rowNumber - 1 > 0)
                    {
                        foreignCell = _cellBorder[rowNumber - 1];
                        cell.SetNext(Directions.SouthWest, foreignCell);
                        foreignCell.SetNext(Directions.NorthEast, cell);
                    }

                    if (rowNumber + 1 < _cellBorder.Count)
                    {
                        foreignCell = _cellBorder[rowNumber + 1];
                        cell.SetNext(Directions.NorthWest, foreignCell);
                        foreignCell.SetNext(Directions.SouthEast, cell);
                    }
                }

                if (rowNumber - 1 < 0)
                    return;
                {
                    var foreignCell = _cellBorder[rowNumber - 1];
                    cell.SetNext(Directions.SouthWest, foreignCell);
                    foreignCell.SetNext(Directions.NorthEast, cell);
                }
            }
        }

        private void Awake()
        {
            _roomPrefab = Resources.Load<GameObject>("Prefabs/Room");
        }

        private void CheckPowerUp()
        {
            //check if the player has activated a power up
            if (_powerUps.ContainsValue(_playerCell))
            {
                var powerUp = _powerUps.FirstOrDefault(x => x.Value == _playerCell).Key;
                if (!powerUp.IsUsed)
                {
                    //change player piece
                    var tempPrefab = powerUp.Type switch
                    {
                        PieceType.Bishop => prefabBishop,
                        PieceType.Knight => prefabKnight,
                        _ => prefabRook
                    };

                    Destroy(_playerPiece.gameObject);
                    var playerObj = Instantiate(
                        tempPrefab,
                        _playerCell.GetGlobalCoordinates(),
                        quaternion.identity
                    );
                    playerObj.GetComponent<MeshRenderer>().material = lightMaterial;
                    _playerPiece = playerObj.GetComponent<Piece>();
                    _playerPiece.PieceRole = PieceRole.Player;

                    //set powerUp to used
                    powerUp.SetActive();
                }
            }
        }

        public void ResetPowerUps()
        {
            foreach (var p in _powerUps.Keys)
            {
                p.SetInactive();
            }
        }
    }
}

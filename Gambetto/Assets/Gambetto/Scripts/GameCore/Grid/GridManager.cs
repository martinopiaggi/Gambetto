using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gambetto.Scripts.GameCore.Piece;
using Gambetto.Scripts.GameCore.Room;
using Gambetto.Scripts.UI;
using Gambetto.Scripts.Utils;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Behaviour = Gambetto.Scripts.GameCore.Room.Behaviour;
using Object = UnityEngine.Object;

namespace Gambetto.Scripts.GameCore.Grid
{
    public class GridManager : MonoBehaviour
    {
        private GameObject _roomPrefab;
        private int _lastColor = 1;
        private bool _changed;
        public PlayerController playerController;

        //todo: Think where to place it
        private List<Vector3> _roomsCenter = new();

        [FormerlySerializedAs("CPUBehavior")]
        public CPUBehavior cpuBehavior;

        private readonly List<List<Cell>> _grid = new(); //maybe we can remove _grid? (never used)
        private Dictionary<Piece.Piece, Cell> _enemies = new();
        private Dictionary<Piece.Piece, List<Vector3>> _enemiesPath = new();
        private Dictionary<Piece.Piece, Cell> _initialEnemiesPositions = new();
        private Dictionary<PowerUp, Cell> _powerUps = new();

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
        public GameObject pawnPowerUp;

        [SerializeField]
        public GameObject endLevel;

        [SerializeField]
        public GameObject keyTile;

        [SerializeField]
        public GameObject bombTile;

        [SerializeField]
        public GameObject deathScreen;

        [SerializeField]
        public GameObject pauseButton;

        public Material lightMaterial;
        public Material darkMaterial;

        public bool IsDead
        {
            get => isDead;
            set
            {
                // if player is set to dead and was not dead before, increment the death count
                if (!isDead && value && GameManager.Instance != null)
                {
                    GameManager.Instance.DeathCount++;
                }
                isDead = value;
            }
        }
        private bool isDead;

        private GameObject _spawnGameObject;

        private Cell _playerCell;
        private Cell _initialPlayerCell;
        private Cell _endLevelCell;
        private Piece.Piece _playerPiece;
        private List<Vector3> _playerPath;

        private bool _gridFinished;

        private GameObject _endLevelMenu;

        private float _timeSinceLastInput;

        // input is taken every 150ms to let all the pieces the time to move
        private float InputTimeInterval = TimeManager.InputTimeInterval;

        private void Start()
        {
            StartCoroutine(StartClockDelayedCoroutine());
        }

        private void Awake()
        {
            _roomPrefab = Resources.Load<GameObject>("Prefabs/Room");
            _endLevelMenu = GameObject
                .FindWithTag("UI")
                .transform
                .Find("EndOfLevelMenu")
                .gameObject;
        }

        /// <summary>
        /// This method is used to handle the player input
        /// </summary>
        private void Update()
        {
            _timeSinceLastInput += Time.deltaTime;
            if (
                (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                && _timeSinceLastInput >= InputTimeInterval
                && playerController.choosing
                && !IsDead
                && Time.timeScale != 0f
                && !PauseButton.mouseOverItemDropLocation
            )
            {
                playerController.OnClick();
                _timeSinceLastInput = 0f;
            }
        }

        //delayed start clock coroutine
        private static IEnumerator StartClockDelayedCoroutine()
        {
            yield return new WaitForSeconds(2f);
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

            if (!IsDead)
            {
                UpdatePlayerPosition();
            }

            if (_playerCell.IsEmpty())
            {
                IsDead = true;
                AudioManager.Instance.PlaySfx(AudioManager.Instance.deathByFall);
                GameClock.Instance.StopClock();
                pauseButton.SetActive(false);
                playerController.choosing = false;
                StartCoroutine(ShowDelayed(deathScreen, 1f));
            }

            if (!IsDead)
            {
                if (GameClock.Instance.CurrentTick() != 0)
                {
                    if (CheckEndLevel())
                        return;
                    // if it's the first tick, don't compute the cpu moves and dont update the enemies position
                    cpuBehavior.ComputeCPUMoves(_playerCell, _enemies);
                    UpdateEnemiesPosition();
                    CheckPowerUp();
                    CheckKeyDoor();
                    CheckBombTrigger();
                    CheckBombExplosion();
                    // check if the player is dead after bomb is exploded
                    if (IsDead)
                        return;
                }
                playerController.StartChoosing(_playerPiece, _playerCell);
            }
        }

        private void CheckKeyDoor()
        {
            if (_playerCell != _key)
                return;
            if (_doors.Count == 0)
                return;
            if (!_doors[0].IsEmpty())
                return; //it means that the doors are already open
            foreach (var door in _doors)
                door.SetEmpty(false);
            CubesRuntimeManager.instance.ToggleAllDoors(true);
        }

        Dictionary<Cell, GameObject> _bombs = new();
        private List<Cell> _detonatedCells = new List<Cell>();

        Dictionary<Cell, int> _detonatedCellsTimer = new Dictionary<Cell, int>();
        List<Cell> _emptyDetonateCells = new List<Cell>();

        private void CheckBombTrigger()
        {
            //check if any player or enemy is on a bomb
            var playerEnemies = new List<Cell>();
            playerEnemies.AddRange(_enemies.Values.ToList());
            playerEnemies.Add(_playerCell);

            //check if any player or enemy is on a bomb
            foreach (var cell in playerEnemies)
            {
                if (cell.IsEmpty())
                    continue; //bomb already exploded and enemy is on an empty
                if (!_bombs.ContainsKey(cell))
                    continue;

                //find in _bombs the cell which is the same as _playerCell
                var bombCell = _bombs.Keys.ToList().Find(c => c == cell);
                if (_detonatedCellsTimer.ContainsKey(bombCell))
                    continue; //already activated

                //find the powerup in _powerups which has value = bomb to hide it during explosion
                var powerUp = _powerUps.Keys.ToList().Find(p => _powerUps[p] == bombCell);
                powerUp.SetInactive(); //disactivate the powerup
                _detonatedCellsTimer.Add(bombCell, 3);
            }
        }

        private void CheckBombExplosion()
        {
            if (_detonatedCellsTimer.Count == 0)
                return;

            foreach (var cell in _detonatedCellsTimer.Keys.ToList())
            {
                _detonatedCellsTimer[cell]--;
                CubesRuntimeManager.instance.PulsingNeighborhood(cell.Neighborhood());
                _bombs[cell].GetComponentInChildren<TextMeshPro>().text =
                    RomanNumeralGenerator.GenerateNumeral(_detonatedCellsTimer[cell]);
                if (_detonatedCellsTimer[cell] == 0)
                {
                    BombExplosion(cell);
                    _detonatedCellsTimer.Remove(cell);
                }
            }
        }

        private void BombExplosion(Cell bombCell)
        {
            //find the powerup in _powerups which has value = bomb to hide it during explosion
            var powerUp = _powerUps.Keys.ToList().Find(p => _powerUps[p] == bombCell);
            powerUp.PowerUpObject.SetActive(false);

            var bombNeighborhood = new List<Cell>();
            bombNeighborhood.Add(bombCell);
            bombNeighborhood.AddRange(bombCell.Neighborhood());

            foreach (var detonatedCell in bombNeighborhood)
            {
                //before set each detonated cell as empty, save if it's actual empty
                //second member is necessary in case of multiple overlapping bombs:
                //the cell would be empty after the first explosion
                //and marked wrongly as "original empty"
                //checking over _detonatedCells this bug is avoided
                if (detonatedCell.IsEmpty() && !_detonatedCells.Contains(detonatedCell))
                {
                    _emptyDetonateCells.Add(detonatedCell);
                }
                else
                {
                    //set detonated cell as empty
                    detonatedCell.SetEmpty();
                }
            }

            //kill immediately player if inside the bomb explosion
            if (bombNeighborhood.Contains(_playerCell))
                IsDead = true;

            //kill immediately enemies if inside the bomb explosion
            foreach (var enemy in _enemies)
            {
                if (bombNeighborhood.Contains(enemy.Value))
                {
                    enemy.Key.SetIsKinematic(false);
                }
            }

            //move down physical cubes of the detonated cells
            CubesRuntimeManager.instance.DetonateNeighborhood(bombNeighborhood);

            //accumulate all the cells detonated (also of different bombs) in a list
            _detonatedCells.AddRange(bombNeighborhood);
        }

        public Cell GetPlayerPosition()
        {
            return _playerCell;
        }

        public List<Vector3> GetRoomsCenter()
        {
            return _roomsCenter;
        }

        public void RestartLevel()
        {
            StartCoroutine(RestartLevelCoroutine());
        }

        /// <summary>
        /// This method is used to restart the level after the player death
        /// resetting enemies position, player position, cleaning all stuff of the previous play
        /// and starting the clock again
        /// </summary>
        private IEnumerator RestartLevelCoroutine()
        {
            GameClock.Instance.StopClock();
            //after the effects, all the cubes are in the fog, resetting positions:
            CubesRuntimeManager.instance.ResetEndOfLevelEffect();
            _enemies.Clear();
            _enemies = new Dictionary<Piece.Piece, Cell>(_initialEnemiesPositions);
            _playerCell = _initialPlayerCell;

            playerController.ResetController();
            cpuBehavior.ChosenMoves.Clear();
            cpuBehavior.MovePaths.Clear();

            //yield return new WaitForSeconds(1f);

            foreach (var enemy in _enemies)
            {
                enemy
                    .Key
                    .ResetAndMovePiece(new List<Vector3> { enemy.Value.GetGlobalCoordinates() });
                enemy.Key.SetIsKinematic(false);
            }

            Destroy(_playerPiece.gameObject);
            var playerObj = Instantiate(
                prefabPawn,
                _playerCell.GetGlobalCoordinates(),
                quaternion.identity
            );
            playerObj.GetComponent<MeshRenderer>().material = lightMaterial;
            _playerPiece = playerObj.GetComponent<Piece.Piece>();
            _playerPiece.PieceRole = PieceRole.Player;
            ResetPowerUps();
            ResetDoor();
            ResetDetonatedCells();
            ResetBombTimers();
            IsDead = false;
            pauseButton.SetActive(true);
            GameClock.Instance.StartClock();
            yield return null;
        }

        /// <summary>
        /// This method is used to update the enemies position
        /// </summary>
        private void UpdateEnemiesPosition()
        {
            _enemies = new Dictionary<Piece.Piece, Cell>(cpuBehavior.ChosenMoves);
            _enemiesPath = new Dictionary<Piece.Piece, List<Vector3>>(cpuBehavior.MovePaths);

            foreach (var enemy in _enemies)
            {
                if (enemy.Value.IsEmpty())
                    continue; //skip if the enemy is dead in an explosion
                enemy.Key.Move(_enemiesPath[enemy.Key]);
            }
        }

        private void UpdatePlayerPosition()
        {
            _playerCell = playerController.ChosenMove;
            _playerPath = playerController.MovePath;
            // set if the player moved in the last turn
            playerController.PlayerIsStill = (
                Vector3.Distance(_playerPiece.transform.position, _playerPath[^1]) < 0.1f
            );
            _playerPiece.Move(_playerPath);
        }

        /// <summary>
        /// It creates the grid data structure from a list of roomLayouts (CSV files)
        /// While builds the data structure, it calls the RoomBuilder to actually spawn the gameobjects
        /// of the chessboard.
        /// </summary>
        public void CreateGrid(List<RoomLayout> roomLayouts, ColorScheme colorScheme)
        {
            var translation = new Vector3(0, 0, 0);
            RoomLayout previousRoomLayout = null;

            // for each room in the level
            for (var roomIdx = 0; roomIdx < roomLayouts.Count; roomIdx++)
            {
                var roomLayout = roomLayouts[roomIdx];

                roomLayout.LoadRoomData();

                var roomObj = Instantiate(_roomPrefab, translation, Quaternion.identity);
                var roomBuilder = roomObj.GetComponent<RoomBuilder>();

                //if the last color is 0 (white) the starting color will be changed in 1 (blue)
                if (_lastColor == 1)
                {
                    roomBuilder.SetColorStart(0);
                    _changed = false;
                }
                else
                {
                    roomBuilder.SetColorStart(1);
                    _changed = true;
                }

                roomBuilder.InitializeRoom(roomLayout);
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
                // I find the center if the current room and store it in a list
                Vector3 roomCenter =
                    _grid[roomIdx][0].GetGlobalCoordinates()
                    + _grid[roomIdx][_grid[roomIdx].Count - 1].GetGlobalCoordinates();
                roomCenter.x /= 2.0f;
                roomCenter.y = 0.0f;
                roomCenter.z /= 2.0f;
                _roomsCenter.Add(roomCenter);
                _lastColor = ColorConsistencyUpdate(roomLayout, _changed);
            }

            //Debug.Log("grid finished");
            _gridFinished = true;

            // start the clock after the grid is created
            GameClock.Instance.ClockTick += OnClockTick;
        }

        /// <summary>
        /// This method is used to make consistent the color of the chessboard between rooms
        /// </summary>
        private int ColorConsistencyUpdate(RoomLayout roomLayout, bool changed)
        {
            //this if determine what is the last color of the room, 1 (dark), 0 (bright)
            //if the room has an even length and was not changed its last color is 1 (dark)
            //if the room has an odd length and was not changed its last color is 0 (bright)
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

        /// <summary>
        /// This method is used to populate the graph data structure (using multiple support methods)
        /// of the grid from a roomLayout.
        /// It returns a list of cells that are the cells of the current room.
        /// Also calls the methods to create pieces and power ups.
        /// </summary>
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
                        var behaviour = roomLayout.Behaviours.Find(b => b.Id == square.Identifier);

                        if (behaviour == null)
                        {
                            behaviour = ScriptableObject.CreateInstance<Behaviour>();
                        }

                        InstantiatePiece(cell, square, behaviour);
                        InstantiateTiles(cell, square);
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

        /// <summary>
        /// This method is used to support the creation of pieces and to store them in a dictionary (_enemies)
        /// </summary>
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
                    _playerPiece = playerObj.GetComponent<Piece.Piece>();
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
            var pieceScript = pieceObj.GetComponent<Piece.Piece>();
            pieceObj.tag = "Enemy"; // tag the enemy for collision detection
            pieceScript.PieceRole = PieceRole.Enemy;

            pieceScript.Behaviour = behaviour;
            if (pieceScript.Behaviour.Movements.Count != 0)
            {
                pieceScript.HasPattern = true;
            }

            pieceObj.GetComponent<MeshRenderer>().material = darkMaterial;
            pieceObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            pieceObj.GetComponent<Rigidbody>().isKinematic = true;
            _enemies.Add(pieceScript, cell);
            _initialEnemiesPositions.Add(pieceObj.GetComponent<Piece.Piece>(), cell);
        }

        Cell _key;
        private List<Cell> _doors = new List<Cell>();

        /// <summary>
        /// This method is used to support the creation of power ups/ end of level
        /// and to store them in a dictionary (_powerUps) or in a variable (_endLevelCell)
        /// </summary>
        private void InstantiateTiles(Cell cell, RoomLayout.Square square)
        {
            switch (square.Value)
            {
                case RoomLayout.MatrixValue.Bomb: //Bomb
                    _bombs.Add(cell, InstantiatePowerUp(bombTile, PieceType.Pawn, cell));

                    break;
                case RoomLayout.MatrixValue.Key: //Key
                    _key = cell;
                    InstantiatePowerUp(keyTile, PieceType.Pawn, cell);
                    break;
                case RoomLayout.MatrixValue.Door: //door
                    _doors.Add(cell);
                    cell.SetEmpty();
                    CubesRuntimeManager.instance.AddDoorCoords(cell.GetGlobalCoordinates());
                    break;
                case RoomLayout.MatrixValue.PB: //Bishop power up
                    InstantiatePowerUp(bishopPowerUp, PieceType.Bishop, cell);
                    break;
                case RoomLayout.MatrixValue.PK: //Knight power up
                    InstantiatePowerUp(knightPowerUp, PieceType.Knight, cell);
                    break;
                case RoomLayout.MatrixValue.PR: //Rook power up
                    InstantiatePowerUp(rookPowerUp, PieceType.Rook, cell);
                    break;
                case RoomLayout.MatrixValue.PP: //Pawn power up
                    InstantiatePowerUp(pawnPowerUp, PieceType.Pawn, cell);
                    break;
                case RoomLayout.MatrixValue.Exit: //End of level
                    _endLevelCell = cell;
                    Instantiate(
                        endLevel,
                        cell.GetGlobalCoordinates() + new Vector3(0, 0.001f, 0),
                        quaternion.identity
                    );
                    CubesRuntimeManager.instance.AddExitCoords(cell.GetGlobalCoordinates());
                    break;
                default:
                    return;
            }
        }

        private GameObject InstantiatePowerUp(
            GameObject prefab,
            PieceType type,
            Cell cell,
            Quaternion rotation = default
        )
        {
            var powerUpObj = Instantiate(
                prefab,
                cell.GetGlobalCoordinates() + new Vector3(0, 0.001f, 0),
                quaternion.identity
            );
            if (prefab != keyTile)
            {
                var obj = new PowerUp(type, powerUpObj, cell);
                _powerUps.Add(obj, cell);
            }

            CubesRuntimeManager.instance.AddTile(powerUpObj);
            return powerUpObj;
        }

        /// <summary>
        /// This method is used to support the creation of a cell in the graph data structure
        /// </summary>
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

        /// <summary>
        /// This method is used to update the links between cells in the SAME room
        /// </summary>
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

        /// <summary>
        /// This method is used to update the links between cells in different rooms
        /// </summary>
        private void SolveInterRoomConsistencies(
            Cell cell,
            int rowNumber,
            int columnNumber,
            Vector2Int borderCheckDirection,
            // ReSharper disable once UnusedParameter.Local
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

        private bool CheckEndLevel()
        {
            if (_playerCell == _endLevelCell)
            {
                GameClock.Instance.StopClock();
                Destroy(_playerPiece.gameObject);

                var playerObj = Instantiate(
                    prefabKing,
                    _playerCell.GetGlobalCoordinates(),
                    quaternion.identity
                );
                playerObj.GetComponent<MeshRenderer>().material = lightMaterial;
                _playerPiece = playerObj.GetComponent<Piece.Piece>();

                ExecuteAfterDelay(
                    0.5f,
                    () => CubesRuntimeManager.instance.FireEffect(_playerPiece.gameObject)
                );

                // but they don't fall
                foreach (var e in _enemies)
                {
                    e.Key.EnableGravity();
                    e.Key.SetIsKinematic(false);
                }

                // we need to wait a bit before showing the end level menu and stopping time
                AudioManager.Instance.PlaySfx(AudioManager.Instance.levelFinished);

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SetLevelCompleted(SceneManager.GetActiveScene().name);
                }

                pauseButton.SetActive(false);
                playerController.choosing = false;
                StartCoroutine(ShowDelayed(_endLevelMenu, 3.0f));
                pauseButton.SetActive(false);
                return true;
            }

            return false;
        }

        // a method that executes a method after a delay
        public void ExecuteAfterDelay(float delay, Action action)
        {
            StartCoroutine(ExecuteAfterDelayCoroutine(delay, action));
        }

        private static IEnumerator ExecuteAfterDelayCoroutine(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action();
        }

        /// <summary>
        /// This method is used to show a gameobject after a delay at the end of the level
        /// </summary>
        public IEnumerator ShowDelayed(GameObject g, float delay = 0.5f)
        {
            yield return new WaitForSeconds(delay);
            g.SetActive(true);
            TimeManager.StopTime();
        }

        /// <summary>
        /// This method is used to check if the player has activated a power up
        /// </summary>
        private void CheckPowerUp()
        {
            //check if the player has activated a power up
            if (_powerUps.ContainsValue(_playerCell))
            {
                var powerUp = _powerUps.FirstOrDefault(x => x.Value == _playerCell).Key;
                // check if the power up is not already used
                if (!powerUp.IsConsumed)
                {
                    AudioManager.Instance.PlaySfx(AudioManager.Instance.powerUp);
                    //change player piece
                    var tempPrefab = powerUp.Type switch
                    {
                        PieceType.Bishop => prefabBishop,
                        PieceType.Knight => prefabKnight,
                        PieceType.Rook => prefabRook,
                        //PieceType.King => prefabKing,
                        _ => prefabPawn
                    };

                    //set the correct clock period
                    GameClock
                        .Instance
                        .ChangeClockPeriod(PieceConstants.TypesClockPeriods[powerUp.Type]);

                    Destroy(_playerPiece.gameObject);
                    var playerObj = Instantiate(
                        tempPrefab,
                        _playerCell.GetGlobalCoordinates(),
                        quaternion.identity
                    );
                    playerObj.GetComponent<MeshRenderer>().material = lightMaterial;
                    _playerPiece = playerObj.GetComponent<Piece.Piece>();
                    _playerPiece.PieceRole = PieceRole.Player;

                    //set powerUp to used
                    powerUp.SetActive();
                }
            }
        }

        private void ResetPowerUps()
        {
            foreach (var p in _powerUps.Keys)
                p.SetInactive();
            GameClock.Instance.ChangeClockPeriod(GameClock.DefaultClockPeriod);
        }

        private void ResetDoor()
        {
            if (_doors.Count == 0)
                return;
            if (_doors[0].IsEmpty())
                return; //it means that the doors are already open
            CubesRuntimeManager.instance.ToggleAllDoors(false, true);
            foreach (var door in _doors)
                door.SetEmpty();
        }

        private void ResetDetonatedCells()
        {
            if (_detonatedCells.Count == 0)
                return;

            foreach (var detonatedCell in _detonatedCells)
            {
                //if not original empty, it must be set as not empty
                if (!_emptyDetonateCells.Contains(detonatedCell))
                {
                    detonatedCell.SetEmpty(false);
                }
            }

            foreach (var powerUp in _powerUps)
                powerUp.Key.PowerUpObject.SetActive(true);

            CubesRuntimeManager.instance.ResetDetonatedCubes();

            _emptyDetonateCells.Clear();
            _detonatedCells.Clear(); //clear the list of detonated cells IMPORTANT
        }

        private void ResetBombTimers()
        {
            _detonatedCellsTimer.Clear();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Gambetto.Scripts.GameCore.Grid;
using Gambetto.Scripts.GameCore.Piece;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Gambetto.Scripts.GameCore
{
    public class CPUBehavior : MonoBehaviour
    {
        private GameObject _selectedSquare;
        private Dictionary<Piece.Piece, Cell> _chosenMoves; //todo this could be reworked and replaced with _movePaths ?
        private Dictionary<Piece.Piece, List<Vector3>> _movePaths;
        private List<List<Vector3>> _possiblePaths;
        private Cell _playerCell;
        private Coroutine _computingCoroutine;

        public Dictionary<Piece.Piece, Cell> ChosenMoves
        {
            get => _chosenMoves;
            set => _chosenMoves = value;
        }

        public Dictionary<Piece.Piece, List<Vector3>> MovePaths
        {
            get => _movePaths;
            set => _movePaths = value;
        }

        // Start is called before the first frame update
        private void Awake()
        {
            var selectedSquarePrefab = Resources.Load<GameObject>("Prefabs/Square");
            //In start I create the light used to illuminate the grid
            _selectedSquare = Instantiate(selectedSquarePrefab);
            _selectedSquare.SetActive(false);
            _chosenMoves = new Dictionary<Piece.Piece, Cell>();
            _movePaths = new Dictionary<Piece.Piece, List<Vector3>>();
            _possiblePaths = new List<List<Vector3>>();
        }
        /// <summary>
        /// This method is used to compute the next move for the AI.
        /// </summary>
        /// <param name="playerCell">Starting cell of the player.</param>
        /// <param name="enemies">Starting cells of the enemies.</param>
        public void ComputeCPUMoves(Cell playerCell, Dictionary<Piece.Piece, Cell> enemies) //todo enemies!
        {
            _playerCell = playerCell;

            var patternEnemies = enemies.Where(enemy => enemy.Key.HasPattern);
            var aiEnemies = enemies.Where(enemy => !enemy.Key.HasPattern);

            // Compute first all enemies with patterns
            foreach (var enemyRef in patternEnemies)
            {
                ComputeNextMove(enemyRef.Key, enemyRef.Value);
            }

            foreach (var enemyRef in aiEnemies)
            {
                ComputeNextMove(enemyRef.Key, enemyRef.Value);
            }
        }
        
        /// <summary>
        /// This method computes the next move for the single enemy.
        /// A distinction is made between enemies with patterns and enemies without patterns.
        /// </summary>
        /// <param name="piece">Single enemy piece.</param>
        /// <param name="cell">Single enemy's current cell.</param>
        private void ComputeNextMove(Piece.Piece piece, Cell cell)
        {
            //if the enemy is on a detonated cell, it doesn't move
            if (cell.IsEmpty()) return; 
            
            //pattern based AI behavior
            if (piece.HasPattern)
            {
                var i =
                    GameClock.Instance.CurrentTick() - 1 > 0
                        ? GameClock.Instance.CurrentTick() - 1
                        : 0;

                var move = piece.Behaviour.Movements[
                    (i + piece.Behaviour.Offset) % piece.Behaviour.Movements.Count
                ];
                var nextCell = cell;
                var tempListMoves = new List<Vector3>();

                //code used to calculate next cell even in case the movement is not a single step
                if (move.x != 0)
                {
                    for (
                        var j = 1 * Sign(move.x);
                        (j * Sign(j)) <= move.x * Sign(move.x);
                        j += (1 * Sign(move.x))
                    )
                    {
                        nextCell = nextCell.GetNext(new Vector2Int(1 * Sign(move.x), 0));
                    }
                }

                if (piece.PieceType == PieceType.Knight)
                {
                    tempListMoves.Add(nextCell.GetGlobalCoordinates());
                }

                if (move.y != 0)
                {
                    for (
                        var k = 1 * Sign(move.y);
                        (k * Sign(k)) <= move.y * Sign(move.y);
                        k += (1 * Sign(k))
                    )
                    {
                        nextCell = nextCell.GetNext(new Vector2Int(0, 1 * Sign(move.y)));
                    }
                }

                tempListMoves.Add(nextCell.GetGlobalCoordinates());
                _chosenMoves[piece] = nextCell;
                _movePaths[piece] = tempListMoves;
                return;
            }

            //standard behavior for the AI
            var dist =
                Mathf.Abs(cell.GetGlobalCoordinates().x - _playerCell.GetGlobalCoordinates().x)
                + Mathf.Abs(cell.GetGlobalCoordinates().z - _playerCell.GetGlobalCoordinates().z);
            var moves = new List<Vector3>();

            if (
                dist <= piece.Behaviour.ActivationDistanceCells
                || piece.Behaviour.Aggressive
                || piece.IsAwake
            )
            {
                if (!piece.IsAwake)
                {
                    piece.IsAwake = true;
                    return;
                }

                var found = MinimumPath(piece, cell, _playerCell);
                if (found)
                    return;
                var nearCells = new List<Cell>
                {
                    _playerCell.GetNext(Vector2Int.up),
                    _playerCell.GetNext(Vector2Int.down),
                    _playerCell.GetNext(Vector2Int.right),
                    _playerCell.GetNext(Vector2Int.left),
                    _playerCell.GetNext(Vector2Int.up + Vector2Int.right),
                    _playerCell.GetNext(Vector2Int.up + Vector2Int.left),
                    _playerCell.GetNext(Vector2Int.down + Vector2Int.right),
                    _playerCell.GetNext(Vector2Int.down + Vector2Int.left)
                };
                // null check here is a fallback that should never happen only of the map
                // csv is not correct and isn't surrounded by void cells
                nearCells = nearCells.Where(c => c != null && IsAvailable(c)).ToList();
                foreach (var c in nearCells)
                {
                    found = MinimumPath(piece, cell, c);
                    if (found)
                        return;
                }
            }
            else
            {
                moves.Add(cell.GetGlobalCoordinates());
                _movePaths[piece] = moves;
            }
        }

        /// <summary>
        /// This method is used to calculate the minimum path between the enemy's current cell and the player cell.
        /// It uses a BFS algorithm to calculate the minimum path.
        /// </summary>
        /// <param name="piece">Current enemy piece.</param>
        /// <param name="startCell">Starting cell of the enemy piece.</param>
        /// <param name="playerCell">Final cell of the player piece.</param>
        /// <returns>True if a path is found, false otherwise.</returns>
        private bool MinimumPath(Piece.Piece piece, Cell startCell, Cell playerCell)
        {
            var queue = new Queue<(Cell, List<Cell>)>();
            var visited = new HashSet<Cell>();
            queue.Enqueue((startCell, new List<Cell>()));
            var tempListMoves = new List<Vector3>();
            var initialCell = startCell;

            while (queue.Count > 0)
            {
                var (currentCell, path) = queue.Dequeue();
                if (currentCell.Equals(playerCell)) // end of the algorithm
                {
                    // path will be empty if the enemy is already on the player cell
                    startCell = path.Count > 0 ? path[0] : startCell;
                    tempListMoves.Add(startCell.GetGlobalCoordinates());
                    _chosenMoves[piece] = startCell;
                    if (piece.PieceType.Equals(PieceType.Knight))
                    {
                        tempListMoves = ComputeKnightMovementPattern(
                            initialCell.GetGlobalCoordinates(),
                            startCell.GetGlobalCoordinates()
                        );
                    }
                    _movePaths[piece] = tempListMoves;
                    return true;
                }

                var possibleMovements = PieceMovement.GetPossibleMovements(
                    piece,
                    currentCell,
                    _chosenMoves,
                    out _possiblePaths
                );

                foreach (var nextCell in possibleMovements.Where(nextCell => !visited.Contains(nextCell) && IsAvailable(nextCell)))
                {
                    visited.Add(nextCell);
                    var newPath = new List<Cell>(path) { nextCell };
                    queue.Enqueue((nextCell, newPath));
                }
            }

            return false;
        }

        private static List<Vector3> ComputeKnightMovementPattern(Vector3 startingCell, Vector3 finalCell)
        {
            var temp = finalCell - startingCell;
            var tempListMoves = new List<Vector3>();
            var temp1 = startingCell;
            temp1.x += temp.x;
            tempListMoves.Add(temp1);
            tempListMoves.Add(finalCell);
            return tempListMoves;
        }
        /// <summary>
        /// Checks if a cell is available (is not empty and is not occupied by any enemy).
        /// </summary>
        /// <param name="cell">Cell to check.</param>
        /// <returns>returns true if the cell is not void and if no other Enemies have chosen it as a move.</returns>
        private bool IsAvailable(Cell cell)
        {
            return !cell.IsEmpty() && _chosenMoves.All(enemy => enemy.Value != cell);
        }

        private static int Sign(int x)
        {
            return Convert.ToInt32(x > 0) - Convert.ToInt32(x < 0);
        }
    }
}

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
        private Dictionary<Piece.Piece, Cell> _chosenMoves;
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

        public void ComputeCPUMoves(Cell playerCell, Dictionary<Piece.Piece, Cell> enemies) //todo enemies!
        {
            _playerCell = playerCell;
            foreach (var enemyRef in enemies)
            {
                ComputeNextMove(enemyRef.Key, enemyRef.Value);
            }
        }

        private void ComputeNextMove(Piece.Piece piece, Cell cell)
        {
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
                Cell nextCell = cell;
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
            var dist = (cell.GetGlobalCoordinates() - _playerCell.GetGlobalCoordinates()).magnitude;
            var moves = new List<Vector3>();
            
            
            if (dist <= 6.0f || piece.Behaviour.Aggressive) 
                MinimumPath(piece, cell);
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
        private void MinimumPath(Piece.Piece piece, Cell startCell)
        {
            var playerCell = _playerCell;
            var queue = new Queue<(Cell, List<Cell>)>();
            var visited = new HashSet<Cell>();
            queue.Enqueue((startCell, new List<Cell>()));
            var tempListMoves = new List<Vector3>();

            while (queue.Count > 0)
            {
                var (currentCell, path) = queue.Dequeue();
                if (currentCell.Equals(playerCell)) // end of the algorithm
                {
                    startCell = path[0];
                    tempListMoves.Add(startCell.GetGlobalCoordinates());
                    _chosenMoves[piece] = startCell; //todo: deprecated?
                    _movePaths[piece] = tempListMoves;
                    return;
                }

                var possibleMovements = PieceMovement.GetPossibleMovements(piece, currentCell, out _possiblePaths);

                foreach (var nextCell in possibleMovements.Where(nextCell => !visited.Contains(nextCell)))
                {
                    visited.Add(nextCell);
                    var newPath = new List<Cell>(path) { nextCell };
                    queue.Enqueue((nextCell, newPath));
                }
            }
        }

        [Obsolete("MinimumDistance is deprecated, please use MinimumPath instead.", true)]
        private void MinimumDistance(Piece.Piece piece, Cell cell)
        {
            var possibleMovements = PieceMovement.GetPossibleMovements(piece, cell, out _possiblePaths);
            var minDist = float.MaxValue;
            var index = 0;
            var chosenIndex = 0;
            foreach (var move in possibleMovements)
            {
                index++;
                var dist = move.GetGlobalCoordinates() - _playerCell.GetGlobalCoordinates();
                if (move.IsEmpty() || IsOccupied(move) || !(dist.magnitude < minDist))
                    continue;
                minDist = dist.magnitude;
                cell = move;
                chosenIndex = index - 1;
            }

            _chosenMoves[piece] = cell;
            _movePaths[piece] = _possiblePaths[chosenIndex];
        }

        private bool IsOccupied(Cell cell)
        {
            return _chosenMoves.Any(enemy => enemy.Value == cell);
        }

        private static int Sign(int x)
        {
            return Convert.ToInt32(x > 0) - Convert.ToInt32(x < 0);
        }
    }
}
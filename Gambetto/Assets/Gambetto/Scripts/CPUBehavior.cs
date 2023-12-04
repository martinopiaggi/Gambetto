using System;
using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts.Pieces;
using Gambetto.Scripts.Utils;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Gambetto.Scripts
{
    public class CPUBehavior : MonoBehaviour
    {
        private GameObject _selectedSquare;
        private Dictionary<Piece, Cell> _chosenMoves;
        private Dictionary<Piece, List<Vector3>> _movePaths;
        private List<List<Vector3>> _possiblePaths;
        private Cell _playerCell;
        private Coroutine _computingCoroutine;

        public Dictionary<Piece, Cell> ChosenMoves
        {
            get => _chosenMoves;
            set => _chosenMoves = value;
        }

        public Dictionary<Piece, List<Vector3>> MovePaths
        {
            get => _movePaths;
            set => _movePaths = value;
        }

        // Start is called before the first frame update
        void Awake()
        {
            var selectedSquarePrefab = Resources.Load<GameObject>("Prefabs/Square");
            //In start I create the light used to illuminate the grid
            _selectedSquare = Instantiate(selectedSquarePrefab);
            _selectedSquare.SetActive(false);
            _chosenMoves = new Dictionary<Piece, Cell>();
            _movePaths = new Dictionary<Piece, List<Vector3>>();
            _possiblePaths = new List<List<Vector3>>();
        }

        public void ComputeCPUMoves(Cell playerCell, Dictionary<Piece, Cell> enemies) //todo enemies!
        {
            _playerCell = playerCell;
            foreach (var enemyRef in enemies)
            {
                ComputeNextMove(enemyRef.Key, enemyRef.Value);
            }
        }

        private void ComputeNextMove(Piece piece, Cell cell)
        {
            //pattern based AI behavior
            if (piece.PatternAI)
            {
                Debug.Log("Sono qui");
                var i =
                    GameClock.Instance.CurrentTick() - 1 > 0
                        ? GameClock.Instance.CurrentTick() - 1
                        : 0;

                var move = piece.Pattern.Movements[
                    (i + piece.Pattern.Offset) % piece.Pattern.Movements.Count
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
            var possibleMovements = PieceMovement.GetPossibleMovements(piece, cell, _possiblePaths);
            var chosenMove = cell;
            var minDist = float.MaxValue;

            int index = 0;
            int chosenIndex = 0;
            foreach (var move in possibleMovements)
            {
                index++;
                var dist = move.GetGlobalCoordinates() - _playerCell.GetGlobalCoordinates();
                if (move.IsEmpty())
                    continue;
                if (ThereIsSomeone(move))
                    continue;
                if (dist.magnitude < minDist)
                {
                    minDist = dist.magnitude;
                    chosenMove = move;
                    chosenIndex = index - 1;
                }
            }
            // Debug.Log(
            //     "Old position: "
            //         + cell.getGlobalCoordinates()
            //         + "\nCPU has chosen: "
            //         + chosenMove.getGlobalCoordinates()
            //         + " as next move"
            // );
            _chosenMoves[piece] = chosenMove;
            _movePaths[piece] = _possiblePaths[chosenIndex];
        }

        public bool ThereIsSomeone(Cell here)
        {
            //if (_playerCell == here)
            //    return true;
            foreach (var enemy in _chosenMoves)
            {
                if (enemy.Value == here)
                {
                    return true;
                }
            }

            return false;
        }

        public int Sign(int x)
        {
            return Convert.ToInt32(x > 0) - Convert.ToInt32(x < 0);
        }
    }
}

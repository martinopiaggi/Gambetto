using System;
using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts.Pieces;
using Gambetto.Scripts.Utils;
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

        public void StartComputing(Cell playerCell, Dictionary<Piece, Cell> enemies) //todo enemies!
        {
               if (_computingCoroutine != null)
                StopCoroutine(_computingCoroutine);

               _computingCoroutine = StartCoroutine(ComputingCoroutine(playerCell, enemies));
        }

        public IEnumerator ComputingCoroutine(Cell playerCell, Dictionary<Piece, Cell> enemies)
        {
            _playerCell = playerCell;
            foreach (var enemyRef in enemies)
            {
                ComputeNextMove(enemyRef.Key, enemyRef.Value);
            }
            yield return null;
        }

        private void ComputeNextMove(Piece piece, Cell cell)
        {
            _possiblePaths.Clear();
            var possibleMovements = PieceMovement.GetPossibleMovements(piece, cell, _possiblePaths);
            var chosenMove = cell;
            var minDist = float.MaxValue;

            int index = 0;
            int chosenIndex = 0;
            foreach (var move in possibleMovements)
            {
                index++;
                var dist = move.getGlobalCoordinates() - _playerCell.getGlobalCoordinates();
                if (move.isEmpty())
                    continue;
                if (ThereIsSomeone(move))
                    continue;
                if (dist.magnitude < minDist)
                {
                    minDist = dist.magnitude;
                    chosenMove = move;
                    chosenIndex = index-1;
                }
            }
            // Debug.Log(
            //     "Old position: "
            //         + cell.getGlobalCoordinates()
            //         + "\nCPU has chosen: "
            //         + chosenMove.getGlobalCoordinates()
            //         + " as next move"
            // );
            _chosenMoves[piece] = chosenMove; //todo now for testing
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
    }
}

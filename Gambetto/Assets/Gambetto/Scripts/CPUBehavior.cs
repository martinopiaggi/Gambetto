using System;
using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts.Pieces;
using UnityEngine;

namespace Gambetto.Scripts
{
    public class CPUBehavior : MonoBehaviour
    {
        private GameObject _selectedSquare;
        private Dictionary<Piece, Cell> _chosenMoves;
        private Cell _playerCell;
        private Coroutine _computingCoroutine;

        public Dictionary<Piece, Cell> ChosenMoves
        {
            get => _chosenMoves;
            set => _chosenMoves = value;
        }

        // Start is called before the first frame update
        void Awake()
        {
            var selectedSquarePrefab = Resources.Load<GameObject>("Prefabs/Square");
            //In start I create the light used to illuminate the grid
            _selectedSquare = Instantiate(selectedSquarePrefab);
            _selectedSquare.SetActive(false);
            _chosenMoves = new Dictionary<Piece, Cell>();
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
            var possibleMovements = GetPossibleMovementsEnemy(piece, cell);
            var chosenMove = cell;
            var minDist = float.MaxValue;

            foreach (var move in possibleMovements)
            {
                var dist = move.getGlobalCoordinates() - _playerCell.getGlobalCoordinates();
                if (move.isEmpty())
                    continue;
                if (ThereIsSomeone(move))
                    continue;
                if (dist.magnitude < minDist)
                {
                    minDist = dist.magnitude;
                    chosenMove = move;
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
        }
        
        public List<Cell> GetPossibleMovementsEnemy(Piece piece, Cell currentCell)
        {
            var possibleMovement = new List<Cell>();
            Cell tempCell;
            var directions = piece.PossibleMoves;

            switch (piece.PieceType)
            {
                case PieceType.Bishop:
                case PieceType.Queen:
                case PieceType.Rook:
                    foreach (var direction in directions)
                    {
                        tempCell = currentCell;
                        while (tempCell?.getNext(direction) != null)
                        {
                            var nextCell = tempCell.getNext(direction);
                            if (tempCell.isEmpty())
                                break;
                            tempCell = nextCell;
                            possibleMovement.Add(tempCell);
                        }
                    }
                    break;
                case PieceType.Pawn:
                case PieceType.King:
                    foreach (var direction in directions)
                    {
                        tempCell = currentCell;
                        tempCell = tempCell.getNext(direction);
                        if (tempCell != null)
                        {
                            possibleMovement.Add(tempCell);
                        }
                    }
                    break;
                case PieceType.Knight:
                    var i = 0;
                    while (i < directions.Count)
                    {
                        tempCell = currentCell;
                        for (var j = 0; j < 3; j++)
                        {
                            tempCell = tempCell.getNext(directions[i + j]);
                            if (tempCell == null)
                                break;
                        }

                        i = 3 + i;
                        if (tempCell != null)
                            possibleMovement.Add(tempCell);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return possibleMovement;
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

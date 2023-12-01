using System;
using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts.Pieces;
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
        
        public List<Cell> GetPossibleMovements(Piece piece, Cell currentCell)
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
        
        public List<Cell> GetPossibleMovementsEnemy(Piece piece, Cell currentCell)
        {
            var possibleMovement = new List<Cell>();
            var possibleMovementPath = new List<List<Vector3>>();
            Cell tempCell;
            List<Vector3> tempPath;
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
                            tempPath = new List<Vector3>();
                            tempPath.Add(tempCell.getGlobalCoordinates());
                            possibleMovement.Add(tempCell);
                            possibleMovementPath.Add(tempPath);
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
                            tempPath = new List<Vector3>();
                            possibleMovement.Add(tempCell);
                            tempPath.Add(tempCell.getGlobalCoordinates());
                            possibleMovementPath.Add(tempPath);
                        }
                    }
                    break;
                case PieceType.Knight:
                    var i = 0;
                    while (i < directions.Count)
                    {
                        tempCell = currentCell;
                        tempPath = new List<Vector3>();
                        for (var j = 0; j < 3; j++)
                        {
                            tempCell = tempCell.getNext(directions[i + j]);
                            if (tempCell == null)
                                break;
                            if (j == 1)
                            {
                                tempPath.Add(tempCell.getGlobalCoordinates());
                            }
                        }

                        i = 3 + i;
                        if (tempCell != null)
                        {
                            possibleMovement.Add(tempCell);
                            tempPath.Add(tempCell.getGlobalCoordinates());
                            possibleMovementPath.Add(tempPath);
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _possiblePaths = possibleMovementPath;
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

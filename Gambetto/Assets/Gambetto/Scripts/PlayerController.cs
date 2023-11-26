using System;
using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts.Pieces;
using UnityEngine;

namespace Gambetto.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        private Light _playerLight;
        private List<Cell> _possibleMovements;
        private Cell _possibleChoice;
        private GameObject _selectedSquare;
        private Coroutine _cycleMovesCoroutine;
        private Vector3 _lastDirection;

        private bool _choosing;
        private Cell _currentCell; // TODD: no need to pass it to methods

        private void Start()
        {
            var selectedSquarePrefab = Resources.Load<GameObject>("Prefabs/Square");
            // In start I create the light used to illuminate the grid
            _selectedSquare = Instantiate(selectedSquarePrefab);
            _selectedSquare.SetActive(false);

            _possibleMovements = new List<Cell>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && _choosing)
            {
                ChosenMove = _possibleChoice;
                _lastDirection =
                    ChosenMove.getGlobalCoordinates() - _currentCell.getGlobalCoordinates();
                _choosing = false;
                GameClock.Instance.ForceClockTick();
            }
        }

        public Cell ChosenMove { get; set; }

        public void StartChoosing(Piece piece, Cell currentCell)
        {
            if (_cycleMovesCoroutine != null)
                StopCoroutine(_cycleMovesCoroutine);

            _currentCell = currentCell;
            ChosenMove = _currentCell;
            _possibleMovements.Clear();
            _possibleMovements = GetPossibleMovements(piece, currentCell);
            _cycleMovesCoroutine = StartCoroutine(CycleMoves());
        }

        private IEnumerator CycleMoves()
        {
            var clockPeriod = GameClock.Instance.ClockPeriod;
            _choosing = true;
            // find the index of the first move in the direction of the last move
            var firstMove = _possibleMovements.FindIndex(
                cell =>
                    (cell.getGlobalCoordinates() - _currentCell.getGlobalCoordinates())
                    == _lastDirection
            );

            var i = firstMove == -1 ? 0 : firstMove;
            var j = 0;
            // start the cycle from the first move in the direction of the last move
            while (j < _possibleMovements.Count)
            {
                var move = _possibleMovements[i];
                if (_choosing == false)
                    break;
                _selectedSquare.SetActive(true);
                _selectedSquare.transform.position =
                    move.getGlobalCoordinates() + new Vector3(0, 0.05f, 0);
                _possibleChoice = move;
                yield return new WaitForSeconds((clockPeriod / _possibleMovements.Count));
                i = (i + 1) % _possibleMovements.Count;
                j++;
            }
            _selectedSquare.SetActive(false);
        }

        //static just because now we are reusing it in the CPUBehavior
        // todo: maybe considering to move to another class
        public static List<Cell> GetPossibleMovements(Piece piece, Cell currentCell)
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
    }
}

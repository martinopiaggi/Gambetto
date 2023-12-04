using System;
using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts.Pieces;
using Gambetto.Scripts.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace Gambetto.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        private Light _playerLight;
        private List<Cell> _possibleMovements;
        private Cell _possibleChoice;
        private List<Vector3> _possiblePath;
        private List<List<Vector3>> _possibleMovementsPath;
        private GameObject _selectedSquare;
        private Coroutine _cycleMovesCoroutine;
        private Vector3 _lastDirection;

        private bool _choosing;
        private Cell _currentCell; // TODD: no need to pass it to methods
        
        private void Awake()
        {
            var selectedSquarePrefab = Resources.Load<GameObject>("Prefabs/Square");
            // In start I create the light used to illuminate the grid
            _selectedSquare = Instantiate(selectedSquarePrefab);
            _selectedSquare.SetActive(false);

            _possibleMovements = new List<Cell>();
            _possibleMovementsPath = new List<List<Vector3>>();
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && _choosing)
            {
                // AudioManager.Instance.PlaySfx(AudioManager.Instance.chosenMove);
                ChosenMove = _possibleChoice;
                MovePath = _possiblePath;
                _lastDirection = ChosenMove.GetGlobalCoordinates() - _currentCell.GetGlobalCoordinates();
                _choosing = false;
                GameClock.Instance.ForceClockTick();
            }
        }

        public Cell ChosenMove { get; set; }
        public List<Vector3> MovePath { get; set; }

        public void StartChoosing(Piece piece, Cell currentCell)
        {
            if (_cycleMovesCoroutine != null)
                StopCoroutine(_cycleMovesCoroutine);

            _currentCell = currentCell;
            ChosenMove = _currentCell;
            MovePath = new List<Vector3>();
            MovePath.Add(_currentCell.GetGlobalCoordinates());
            _possibleMovementsPath.Clear();
            _possibleMovements = PieceMovement.GetPossibleMovements(piece, currentCell,out _possibleMovementsPath);
            _cycleMovesCoroutine = StartCoroutine(CycleMoves());
        }
        
        /// <summary>
        /// reset the controller to its initial state
        /// </summary>
        public void ResetController()
        {
            _choosing = false;
            ChosenMove = null;
            _selectedSquare.SetActive(false);
            _lastDirection = default;
            if (_cycleMovesCoroutine != null)
                StopCoroutine(_cycleMovesCoroutine);
        }

        private IEnumerator CycleMoves()
        {
            var clockPeriod = GameClock.Instance.ClockPeriod;
            _choosing = true;
            // find the index of the first move in the direction of the last move
            var firstMove = _possibleMovements.FindIndex(
                cell =>
                    (cell.GetGlobalCoordinates() - _currentCell.GetGlobalCoordinates())
                    == _lastDirection
            );

            var i = firstMove == -1 ? 0 : firstMove;
            var j = 0;
            // start the cycle from the first move in the direction of the last move
            while (j < _possibleMovements.Count)
            {
                AudioManager.Instance.PlaySfx(AudioManager.Instance.clockTick);
                var move = _possibleMovements[i];
                var movePath = _possibleMovementsPath[i];
                if (_choosing == false)
                    break;
                _selectedSquare.SetActive(true);
                _selectedSquare.transform.position =
                    move.GetGlobalCoordinates() + new Vector3(0, 0.02f, 0);
                _possibleChoice = move;
                _possiblePath = movePath;
                yield return new WaitForSeconds((clockPeriod / _possibleMovements.Count));
                i = (i + 1) % _possibleMovements.Count;
                j++;
            }
            _selectedSquare.SetActive(false);
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts.GameCore.Grid;
using Gambetto.Scripts.GameCore.Piece;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gambetto.Scripts.GameCore
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

        [FormerlySerializedAs("_choosing")]
        public bool choosing;
        private Cell _currentCell; // TODO: no need to pass it to methods

        private void Awake()
        {
            var selectedSquarePrefab = Resources.Load<GameObject>("Prefabs/Square");
            // In start I create the light used to illuminate the grid
            _selectedSquare = Instantiate(selectedSquarePrefab);
            _selectedSquare.SetActive(false);

            _possibleMovements = new List<Cell>();
            _possibleMovementsPath = new List<List<Vector3>>();
        }

        //previously called 'update'
        public void OnClick()
        {
            ChosenMove = _possibleChoice;
            MovePath = _possiblePath;
            _lastDirection =
                ChosenMove.GetGlobalCoordinates() - _currentCell.GetGlobalCoordinates();
            choosing = false;
            GameClock.Instance.ForceClockTick();
        }

        public Cell ChosenMove { get; set; }
        public List<Vector3> MovePath { get; set; }

        public void StartChoosing(Piece.Piece piece, Cell currentCell)
        {
            if (_cycleMovesCoroutine != null)
                StopCoroutine(_cycleMovesCoroutine);

            _currentCell = currentCell;
            ChosenMove = _currentCell;
            MovePath = new List<Vector3> { _currentCell.GetGlobalCoordinates() };
            _possibleMovementsPath.Clear();
            _possibleMovements = PieceMovement.GetPossibleMovements(
                piece,
                currentCell,
                new Dictionary<Piece.Piece, Cell>(),
                out _possibleMovementsPath
            );
            _cycleMovesCoroutine = StartCoroutine(CycleMoves());
        }

        /// <summary>
        /// reset the controller to its initial state
        /// </summary>
        public void ResetController()
        {
            choosing = false;
            ChosenMove = null;
            _selectedSquare.SetActive(false);
            _lastDirection = default;
            if (_cycleMovesCoroutine != null)
                StopCoroutine(_cycleMovesCoroutine);
        }

        private IEnumerator CycleMoves()
        {
            var clockPeriod = GameClock.Instance.ClockPeriod;
            choosing = true;
            // find the index of the first move in the direction of the last move
            var firstMove = _possibleMovements.FindIndex(
                cell =>
                    (cell.GetGlobalCoordinates() - _currentCell.GetGlobalCoordinates())
                    == _lastDirection
            );

            var i = firstMove == -1 ? 0 : firstMove;
            var j = 0;
            var numberOfMoves = _possibleMovements.Count;
            var firstShowingPeriod = (clockPeriod / numberOfMoves) * 1.35f;
            var showingPeriod = 0.0f;
            if (numberOfMoves > 1) showingPeriod = (clockPeriod - firstShowingPeriod) / (numberOfMoves - 1);
            else showingPeriod = firstShowingPeriod;
            // start the cycle from the first move in the direction of the last move
            while (j < numberOfMoves)
            {
                AudioManager.Instance.PlaySfx(AudioManager.Instance.clockTick);
                var move = _possibleMovements[i];
                var movePath = _possibleMovementsPath[i];
                if (choosing == false)
                    break;
                _selectedSquare.SetActive(true);
                _selectedSquare.transform.position =
                    move.GetGlobalCoordinates() + new Vector3(0, 0.0001f, 0);
                _possibleChoice = move;
                _possiblePath = movePath;
                // the first moves have a bit more time
                if(j==0) yield return new WaitForSeconds(firstShowingPeriod);
                else yield return new WaitForSeconds(showingPeriod);
                i = (i + 1) % _possibleMovements.Count;
                j++;
            }
            _selectedSquare.SetActive(false);
        }
    }
}

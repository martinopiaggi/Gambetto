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
        private GameObject highlightedSquarePrefab;
        private Coroutine _cycleMovesCoroutine;
        private Vector3 _lastDirection;
        //varible that  says if the player stayed still in the last turn
        public bool _playerIsStill { get; set; }

        [FormerlySerializedAs("_choosing")]
        public bool choosing;
        private Cell _currentCell; // TODO: no need to pass it to methods

        private void Awake()
        {
            var selectedSquarePrefab = Resources.Load<GameObject>("Prefabs/Square");
            // In start I create the light used to illuminate the grid
            _selectedSquare = Instantiate(selectedSquarePrefab);
            _selectedSquare.SetActive(false);

            highlightedSquarePrefab = Resources.Load<GameObject>("Prefabs/HighlightedSquare");
            InitializePooledSquares();

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
            DeactivateAllSquares();
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
            var multiplicatorFirstMove = 1.0f;
            if (!_playerIsStill) multiplicatorFirstMove = 1.4f; 
            var firstShowingPeriod = (clockPeriod / numberOfMoves) * multiplicatorFirstMove;
            var showingPeriod =
                numberOfMoves > 1
                    ? (clockPeriod - firstShowingPeriod) / (numberOfMoves - 1)
                    : firstShowingPeriod;
            // start the cycle from the first move in the direction of the last move
            while (j < numberOfMoves)
            {
                AudioManager.Instance.PlaySfx(AudioManager.Instance.clockTick);
                if (choosing == false)
                    break;
                _possibleChoice = _possibleMovements[i];
                _possiblePath = _possibleMovementsPath[i];
                _selectedSquare.SetActive(true);
                _selectedSquare.transform.position =
                    _possibleChoice.GetGlobalCoordinates() + new Vector3(0, 0.0001f, 0);

                HighlightSquares(
                    PieceMovement.HighlightedCellsForPath(
                        _currentCell.GetGlobalCoordinates(),
                        _possibleMovementsPath[i]
                    )
                );
                // the first moves have a bit more time
                yield return j == 0
                    ? new WaitForSeconds(firstShowingPeriod)
                    : new WaitForSeconds(showingPeriod);
                i = (i + 1) % _possibleMovements.Count;
                j++;
            }

            _selectedSquare.SetActive(false);
            DeactivateAllSquares();
        }

        private readonly List<GameObject> highLightedSquares = new List<GameObject>();

        private const int AmountToPool = 60;

        private void InitializePooledSquares()
        {
            var container = new GameObject("Squares");
            for (var i = 0; i < AmountToPool; i++)
            {
                var obj = Instantiate(highlightedSquarePrefab, container.transform);
                obj.SetActive(false);
                highLightedSquares.Add(obj);
            }
        }

        private void HighlightSquares(List<Vector3> positions)
        {
            DeactivateAllSquares();
            foreach (var t in positions)
            {
                var obj = GetPooledObject();
                if (obj == null)
                    return;
                obj.transform.position = t + new Vector3(0, 0.0001f, 0);
                obj.SetActive(true);
            }
        }

        private void DeactivateAllSquares()
        {
            highLightedSquares.ForEach(square => square.SetActive(false));
        }

        private GameObject GetPooledObject()
        {
            for (int i = 0; i < AmountToPool; i++)
            {
                if (!highLightedSquares[i].activeInHierarchy)
                {
                    return highLightedSquares[i];
                }
            }
            return null;
        }
    }
}

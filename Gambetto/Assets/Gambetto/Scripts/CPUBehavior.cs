using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts.Pieces;
using UnityEngine;

namespace Gambetto.Scripts
{
    public class CPUBehavior : MonoBehaviour
    {
        private GameObject _selectedSquare;
        private Cell _chosenMoves;

        public Cell ChosenMoves
        {
            get => _chosenMoves;
            set => _chosenMoves = value;
        }


        private Cell _playerCell;
        // Start is called before the first frame update
        void Start()
        {
            var selectedSquarePrefab = Resources.Load<GameObject>("Prefabs/Square");
            //In start I create the light used to illuminate the grid
            _selectedSquare = Instantiate(selectedSquarePrefab);
            _selectedSquare.SetActive(false);
        }
        
        
        public void StartComputing(Piece enemy, Cell enemyCell, Cell playerCell) //todo enemies!
        {
            _playerCell = playerCell;
            //foreach (var enemyRef in enemies)
            //{
            StartCoroutine(ComputeNextMove(enemy, enemyCell));
         //   }
        }

        private IEnumerator ComputeNextMove(Piece piece,Cell cell)
        {
            var clockPeriod = GameClock.Instance.ClockPeriod;
            var possibleMovements = PlayerController.GetPossibleMovements(piece, cell);
            var choosenMove = cell;
            var minDist = float.MaxValue;
            
            foreach (var move in possibleMovements)
            {
                var dist = move.getGlobalCoordinates() - _playerCell.getGlobalCoordinates();
                if (dist.magnitude < minDist)
                {
                    minDist = dist.magnitude;
                    choosenMove = move;
                }
            }
            
            Debug.Log("CPU has chosen");
            _chosenMoves = choosenMove; //todo now for testing
            yield return null;
        }
        
    }
}

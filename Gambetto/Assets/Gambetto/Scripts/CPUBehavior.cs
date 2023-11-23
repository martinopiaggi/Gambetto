using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts.Pieces;
using UnityEngine;

namespace Gambetto.Scripts
{
    public class CPUBehavior : MonoBehaviour
    {
        private GameObject _selectedSquare;

        // Start is called before the first frame update
        void Start()
        {
            var selectedSquarePrefab = Resources.Load<GameObject>("Prefabs/Square");
            //In start I create the light used to illuminate the grid
            _selectedSquare = Instantiate(selectedSquarePrefab);
            _selectedSquare.SetActive(false);
        }
        
        
        public void StartComputing(Dictionary<Piece, Cell> enemies)
        {
            foreach (var enemyRef in enemies)
            {
                StartCoroutine(ComputeNextMove(enemyRef.Key, enemyRef.Value));
            }
        }

        private IEnumerator ComputeNextMove(Piece piece,Cell cell)
        {
            var clockPeriod = GameClock.Instance.ClockPeriod;
            var possibleMovements = PlayerController.GetPossibleMovements(piece, cell);
            foreach (var move in possibleMovements)
            {
                if (_choosing == false) break;
                _selectedSquare.SetActive(true);
                _selectedSquare.transform.position = move.getGlobalCoordinates() + new Vector3(0, 0.05f, 0);
                possibleChoice = move;
                yield return new WaitForSeconds((clockPeriod / possibleMovements.Count) * 0.9f);
            }
            

            _selectedSquare.SetActive(false);
            Debug.Log("CPU has chosen");
        }
    }
}

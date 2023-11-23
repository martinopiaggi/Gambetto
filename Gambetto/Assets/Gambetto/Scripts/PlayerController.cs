using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts.Pieces;
using UnityEngine;

namespace Gambetto.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        public GridManager GridManager;
        private Light playerLight;
        private List<Cell> possibleMovements;
        private Cell possibleChoice;
        private GameObject _selectedSquare;
        
        private bool _choosing = false;

        private void Start()
        {
            var selectedSquarePrefab = Resources.Load<GameObject>("Prefabs/Square");
            //In start I create the light used to illuminate the grid
            _selectedSquare = Instantiate(selectedSquarePrefab);
            _selectedSquare.SetActive(false);
            
            possibleMovements = new List<Cell>();
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Space) || !_choosing) return;
            ChosenMove = possibleChoice;
            _choosing = false;
        }
        
        public Cell ChosenMove { get; set; }

        public void StartChoosing(Piece piece, Cell currentPosition)
        {
            possibleMovements.Clear();
            possibleMovements = GetPossibleMovements(piece, currentPosition);
            StartCoroutine(CycleMoves());
        }
        
        private IEnumerator CycleMoves()
        {
            var clockPeriod = GameClock.Instance.ClockPeriod;
            _choosing = true;
            foreach (var move in possibleMovements)
            {
                if(_choosing == false) break;
                _selectedSquare.SetActive(true);
                _selectedSquare.transform.position = move.getGlobalCoordinates() + new Vector3(0, 0.05f, 0);
                possibleChoice = move;
                yield return new WaitForSeconds((clockPeriod/possibleMovements.Count)*0.9f); 
            }
            _selectedSquare.SetActive(false);
        }
        

        //Now this function suppose that in the grid there are cell with values -1 that resembles the fact that 
        // are empty
        //static just because now we are reusing it in the CPUBehavior 
        // todo: maybe considering to move to another class 
        public static List<Cell> GetPossibleMovements(Piece player, Cell currentPosition)
        {
            List<Cell> possibleMovement = new List<Cell>();
            Cell startingCell = currentPosition;
            Cell tempCell = startingCell;
            List<Vector2Int> directions = player.PossibleMoves;
            if (currentPosition.isEmpty())
            {
                //Debug.Log("starting cell is empty");
                return possibleMovement;
            }

            switch (player.PieceType)
            {
                case PieceType.Bishop:
                    //Debug.Log("bishop");
                    foreach (Vector2Int direction in directions)
                    {
                        tempCell = startingCell;
                        while (tempCell != null)
                        {
                            tempCell = tempCell.getNext(direction);
                            if (tempCell != null)
                            {
                                possibleMovement.Add(tempCell);
                            }
                        }
                    }

                    break;
                case PieceType.King:
                    //Debug.Log("king");
                    foreach (Vector2Int direction in directions)
                    {
                        tempCell = startingCell;
                        tempCell = tempCell.getNext(direction);
                        if (tempCell != null)
                        {
                            possibleMovement.Add(tempCell);
                        }
                    }

                    break;
                case PieceType.Knight:
                    //Debug.Log("knight");
                    int i = 0;
                    while (i < directions.Count)
                    {
                        tempCell = startingCell;
                        for (int j = 0; j < 3; j++)
                        {
                            tempCell = tempCell.getNext(directions[i + j]);
                            if (tempCell == null) break;
                        }

                        i = i + 3;
                        if (tempCell != null) possibleMovement.Add(tempCell);
                    }

                    break;

                case PieceType.Pawn:
                    //Debug.Log("pawn");
                    foreach (Vector2Int direction in directions)
                    {
                        tempCell = startingCell;
                        tempCell = tempCell.getNext(direction);
                        if (tempCell != null)
                        {
                            possibleMovement.Add(tempCell);
                        }
                    }

                    break;
                case PieceType.Queen:
                    //Debug.Log("queen");
                    foreach (Vector2Int direction in directions)
                    {
                        tempCell = startingCell;
                        while (tempCell != null)
                        {
                            tempCell = tempCell.getNext(direction);
                            if (tempCell != null)
                            {
                                possibleMovement.Add(tempCell);
                            }
                        }
                    }

                    break;
                case PieceType.Rook:
                    //Debug.Log("rook");
                    foreach (Vector2Int direction in directions)
                    {
                        tempCell = startingCell;
                        while (tempCell != null)
                        {
                            tempCell = tempCell.getNext(direction);
                            if (tempCell != null)
                            {
                                possibleMovement.Add(tempCell);
                            }
                        }
                    }
                    break;
            }
            return possibleMovement;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts.Pieces;
using Pieces;
using UnityEngine;

namespace Gambetto.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        public GridManager GridManager;
        private Light playerLight;
        private List<Cell> possibleMovements;
        private Cell possibleChoice;
        
        private bool _choosing = false;

        void Start()
        {
            //In start I create the light used to illuminate the grid
            GameObject lightObject = new GameObject("PlayerLight");
            playerLight = lightObject.AddComponent<Light>();

            // Set light properties (you can customize these)
            playerLight.type = LightType.Point;
            playerLight.range = 2.0f;
            playerLight.intensity = 6.0f;

            // Turn off the light initially
            playerLight.enabled = false;
            
            possibleMovements = new List<Cell>();
        }

        void Update()
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
                playerLight.enabled = true;
                // Move the light to the new coordinates
                playerLight.transform.position = move.getGlobalCoordinates() + new Vector3(0, 0.2f, 0);
                possibleChoice = move;
                yield return new WaitForSeconds((clockPeriod/possibleMovements.Count)*0.9f); 
            }
            playerLight.enabled = false;
        }
        
        

        //Now this function suppose that in the grid there are cell with values -1 that resembles the fact that 
        // are empty
        private List<Cell> GetPossibleMovements(Piece player, Cell currentPosition)
        {
            List<Cell> possibleMovement = new List<Cell>();
            Cell startingCell = currentPosition;
            Cell tempCell = startingCell;
            List<Vector2> directions = player.PossibleMoves;
            if (currentPosition.isEmpty())
            {
                //Debug.Log("starting cell is empty");
                return possibleMovement;
            }

            switch (player.PieceType)
            {
                case PieceType.Bishop:
                    //Debug.Log("bishop");
                    foreach (Vector2 direction in directions)
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
                    foreach (Vector2 direction in directions)
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
                    foreach (Vector2 direction in directions)
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
                    foreach (Vector2 direction in directions)
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
                    foreach (Vector2 direction in directions)
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

        IEnumerator WaitForFrames(int framesToWait)
        {
            // Aspetta il numero desiderato di frame
            for (int i = 0; i < framesToWait; i++)
            {
                yield return null; // Aspetta un frame
            }
        }
    }
}
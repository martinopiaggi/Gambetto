using System;
using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts;
using Pieces;
using UnityEngine;

public class PlayerControllerGambetto : MonoBehaviour
{
    public GridManager GridManager;
    private Light playerLight;
    private List<Vector3> possibleMovements;
    private Vector3 currentMove;
    private Vector3 possibleChoiche;
    
    private int framesToWait = 60; // frames to be able to choose
    private int currentFrame = 0;
    private bool enter = false;
    
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
        
        currentMove = new Vector3();
        possibleMovements = new List<Vector3>();

    }
    
    void Update()
    {
        currentFrame++;

        
        if (currentFrame >= framesToWait)
        {
            //in possible movements there are all the grid coordinates that need to be illuminated
            if (possibleMovements.Count > 0)
            {
                // Get the first movement from the list
                Vector3 nextMovement = possibleMovements[0]+new Vector3(0,0.2f,0);
                //Debug.Log(nextMovement);
                // Turn on the light
                playerLight.enabled = true;
                // Move the light to the new coordinates
                playerLight.transform.position = nextMovement;
                possibleChoiche = possibleMovements[0];
                possibleMovements.RemoveAt(0);
                //Debug.Log("setto a true");
                enter = true;
            }
            else
            {
                // If the list is empty, turn off the light
                playerLight.enabled = false;
                enter = false;
            }
            
            currentFrame = 0;
        }
    }

    //I use LateUpdate to control if the player has chosen a position, if yes the current move is update
    //and the list of possible positions is cleared
    private void LateUpdate()
    {
            if (enter)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    //Debug.Log("hai premuto spazio");
                    currentMove = possibleChoiche;
                    //Debug.Log("la scelta è"+currentMove);
                    possibleMovements.Clear();
                    //Currently the position is changed with a setter in GridManager
                    GridManager.setPositionOfPlayer(currentMove);
                    enter = false;
                }
            }
        
    }

    public void startChoosing(Piece piece, Cell currentPosition)
    {
        possibleMovements.Clear();
        possibleMovements = GetPossibleMovements(piece, currentPosition);
        //for (int i = 0; i < possibleMovements.Count; i++)
        //{
        //    Debug.Log(possibleMovements[i]);
        //}
        //StartCoroutine(WaitForFrames(possibleMovements.Count*100));
        //Debug.Log("ritorno val");
    }
    
    //Now this function suppose that in the grid there are cell with values -1 that resembles the fact that 
    // are empty
    private List<Vector3> GetPossibleMovements(Piece player, Cell currentPosition)
        {
            List<Vector3> possibleMovement = new List<Vector3>();
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
                                possibleMovement.Add(tempCell.getGlobalCoordinates());
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
                            possibleMovement.Add(tempCell.getGlobalCoordinates());
                        }
                    }
                    break;
                case PieceType.Knight:
                    //Debug.Log("knight");
                    int i = 0;
                    while(i < directions.Count)
                    {
                        tempCell = startingCell;
                        for (int j = 0; j < 3; j++)
                        {
                            tempCell = tempCell.getNext(directions[i+j]);
                            if (tempCell == null) break;
                        }
                        i = i + 3;
                        if (tempCell != null) possibleMovement.Add(tempCell.getGlobalCoordinates());
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
                            possibleMovement.Add(tempCell.getGlobalCoordinates());
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
                                possibleMovement.Add(tempCell.getGlobalCoordinates());
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
                                possibleMovement.Add(tempCell.getGlobalCoordinates());
                            }
                        }
                    }
                    break;
                default:
                    //Debug.Log("Invalid type of piece");
                    break;
            }

            //for (int l = 0; l < possibleMovement.Count; l++)
            //{
                //Debug.Log(possibleMovement[l]); 
            //}
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

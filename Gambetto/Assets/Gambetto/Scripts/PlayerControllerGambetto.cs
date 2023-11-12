using System;
using System.Collections;
using System.Collections.Generic;
using Pieces;
using UnityEngine;

public class PlayerControllerGambetto : MonoBehaviour
{
    
    private Light playerLight;
    private List<Vector3> possibleMovements;
    private Vector3 currentMove;
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject lightObject = new GameObject("PlayerLight");
        playerLight = lightObject.AddComponent<Light>();

        // Set light properties (you can customize these)
        playerLight.type = LightType.Point;
        playerLight.range = 5.0f;
        playerLight.intensity = 1.0f;

        // Turn off the light initially
        playerLight.enabled = false;
        
        currentMove = new Vector3();
        possibleMovements = new List<Vector3>();

    }

    // Update is called once per frame
    void Update()
    {
        if (possibleMovements.Count > 0)
        {
            // Get the first movement from the list
            Vector3 nextMovement = possibleMovements[0];
            // Turn on the light
            playerLight.enabled = true;
            // Move the light to the new coordinates
            transform.Translate(nextMovement);
        }
        else
        {
            // If the list is empty, turn off the light
            playerLight.enabled = false;
        }
    }

    private void LateUpdate()
    {
        if (possibleMovements.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("hai premuto spazio");
                Debug.Log(currentMove);
                currentMove = possibleMovements[0];
                possibleMovements.Clear();
            }
            else
            {
                // Remove the used movement from the list
                possibleMovements.RemoveAt(0);
            }
            
        }
    }

    public Vector3 startChoosing(Piece piece, Cell currentPosition)
    {
        possibleMovements.Clear();
        possibleMovements = GetPossibleMovements(piece, currentPosition);
        StartCoroutine(WaitForFrames(possibleMovements.Count));
        return currentMove;
    }
    
    private List<Vector3> GetPossibleMovements(Piece player, Cell currentPosition)
        {
            List<Vector3> possibleMovement = new List<Vector3>();
            Cell startingCell = currentPosition;
            Cell tempCell = startingCell;
            List<Vector2> directions = player.PossibleMoves;
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
                    Debug.Log("Invalid type of piece");
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

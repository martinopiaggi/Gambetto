using System.Collections.Generic;
using Gambetto.Scripts.Pieces;
using Pieces;
using UnityEngine;

public class PlayerControllerGambetto : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public List<Vector3> GetPossibleMovements(Piece player, Cell currentPosition)
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
}

using System;
using System.Collections.Generic;
using Gambetto.Scripts.Pieces;
using UnityEngine;

namespace Gambetto.Scripts.Utils
{
    public static class PieceMovement
    {
        public static List<Cell> GetPossibleMovements(Piece piece, Cell currentCell,  List<List<Vector3>> possiblePaths)
        {
            var possibleMovement = new List<Cell>();
            var possibleMovementPath = possiblePaths;
            Cell tempCell;
            List<Vector3> tempPath;
            var directions = piece.PossibleMoves;

            switch (piece.PieceType)
            {
                case PieceType.Bishop:
                case PieceType.Queen:
                case PieceType.Rook:
                    foreach (var direction in directions)
                    {
                        tempCell = currentCell;
                        while (tempCell?.getNext(direction) != null)
                        {
                            var nextCell = tempCell.getNext(direction);
                            if (tempCell.isEmpty())
                                break;
                            tempCell = nextCell;
                            tempPath = new List<Vector3>();
                            tempPath.Add(tempCell.getGlobalCoordinates());
                            possibleMovement.Add(tempCell);
                            possibleMovementPath.Add(tempPath);
                        }
                    }
                    break;
                case PieceType.Pawn:
                case PieceType.King:
                    foreach (var direction in directions)
                    {
                        tempCell = currentCell;
                        tempCell = tempCell.getNext(direction);
                        if (tempCell != null)
                        {
                            tempPath = new List<Vector3>();
                            possibleMovement.Add(tempCell);
                            tempPath.Add(tempCell.getGlobalCoordinates());
                            possibleMovementPath.Add(tempPath);
                        }
                    }
                    break;
                case PieceType.Knight:
                    var i = 0;
                    while (i < directions.Count)
                    {
                        tempCell = currentCell;
                        tempPath = new List<Vector3>();
                        for (var j = 0; j < 3; j++)
                        {
                            tempCell = tempCell.getNext(directions[i + j]);
                            if (tempCell == null)
                                break;
                            if (j == 1)
                            {
                                tempPath.Add(tempCell.getGlobalCoordinates());
                            }
                        }

                        i = 3 + i;
                        if (tempCell != null)
                        {
                            possibleMovement.Add(tempCell);
                            tempPath.Add(tempCell.getGlobalCoordinates());
                            possibleMovementPath.Add(tempPath);
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return possibleMovement;
        }
    }
}
using System;
using System.Collections.Generic;
using Gambetto.Scripts.GameCore.Grid;
using UnityEngine;

namespace Gambetto.Scripts.GameCore.Piece
{
    public static class PieceMovement
    {
        public static List<Cell> GetPossibleMovements(
            Piece piece,
            Cell currentCell,
            Dictionary<Piece, Cell> enemiesChosenMoves,
            out List<List<Vector3>> possiblePaths
        )
        {
            var possibleMovement = new List<Cell>();
            possiblePaths = new List<List<Vector3>>();
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
                        while (tempCell?.GetNext(direction) != null)
                        {
                            var nextCell = tempCell.GetNext(direction);
                            if (
                                tempCell.IsEmpty()
                                || (
                                    tempCell != currentCell
                                    && enemiesChosenMoves.ContainsValue(tempCell)
                                )
                            )
                                break;
                            tempCell = nextCell;
                            tempPath = new List<Vector3> { tempCell.GetGlobalCoordinates() };
                            possibleMovement.Add(tempCell);
                            possiblePaths.Add(tempPath);
                        }
                    }

                    break;
                case PieceType.Pawn:
                case PieceType.King:
                    foreach (var direction in directions)
                    {
                        tempCell = currentCell;
                        tempCell = tempCell.GetNext(direction);
                        if (tempCell != null)
                        {
                            tempPath = new List<Vector3>();
                            possibleMovement.Add(tempCell);
                            tempPath.Add(tempCell.GetGlobalCoordinates());
                            possiblePaths.Add(tempPath);
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
                            tempCell = tempCell.GetNext(directions[i + j]);
                            if (tempCell == null)
                                break;
                            if (j == 1)
                            {
                                tempPath.Add(tempCell.GetGlobalCoordinates());
                            }
                        }

                        i = 3 + i;
                        if (tempCell != null)
                        {
                            possibleMovement.Add(tempCell);
                            tempPath.Add(tempCell.GetGlobalCoordinates());
                            possiblePaths.Add(tempPath);
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return possibleMovement;
        }

        public static List<Vector3> HighlightedCellsForPath(Vector3 pieceCell, List<Vector3> path)
        {
            // return coordinates of cells to highlight for the given path
            var highlightedCells = new List<Vector3>();

            var startCell = pieceCell;
            foreach (var step in path)
            {
                // get a normalized direction vector for each step in the path
                var direction = step - startCell;

                // transforms the direction vector into a unit vector
                direction = new Vector3(
                    direction.x != 0 ? direction.x / Mathf.Abs(direction.x) : 0,
                    0,
                    direction.z != 0 ? direction.z / Mathf.Abs(direction.z) : 0
                );

                var tempCell = startCell;

                while (tempCell != step)
                {
                    tempCell += direction;
                    if (tempCell == step && path.IndexOf(step) == path.Count - 1)
                        break;
                    highlightedCells.Add(tempCell);
                }
                startCell = step;
            }

            return highlightedCells;
        }
    }
}

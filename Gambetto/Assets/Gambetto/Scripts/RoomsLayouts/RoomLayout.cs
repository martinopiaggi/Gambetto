using System;
using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts.Pieces;
using Gambetto.Scripts.Utils;
using Pieces;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "RoomLayout", menuName = "ScriptableObjects/RoomLayout")]
public class RoomLayout : ScriptableObject
{
    [SerializeField]
    private string topologyDescription;

    [SerializeField]
    private TextAsset roomCsvTextAsset;

    [SerializeField]
    private Vector2Int exitSide = Directions.North;

    private Square[,] _roomSquares;

    public Square[,] Squares => _roomSquares;

    public Vector2Int GetExit()
    {
        return exitSide;
    }

    public int GetSizeRow()
    {
        return _roomSquares.GetLength(0);
    }

    public int GetSizeColumn()
    {
        return _roomSquares.GetLength(1);
    }

    public Square[,] LoadRoom()
    {
        _roomSquares = ParseCSV();
        // Load default behaviours
        return _roomSquares;
    }

    private Square[,] ParseCSV()
    {
        // Split the CSV into lines
        string[] lines = roomCsvTextAsset
            .text
            .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        int rows = lines.Length;
        int cols = rows > 0 ? lines[0].Split(',').Length : 0;

        Square[,] matrix = new Square[rows, cols];

        for (int i = rows-1; i >= 0; i--)
        {
            string[] cells = lines[rows-1-i].Split(',');

            for (int j = cells.Length-1; j >= 0; j--)
            {
                matrix[i, j] = ParseCellValue(cells[cells.Length-1-j].Trim());
            }
        }
        return matrix;
    }

    private Square ParseCellValue(string cellValue)
    {
        var identifier = 0; // Default value if no number is found
        cellValue = cellValue.Replace(" ", ""); // Remove spaces
        if (cellValue.Length == 0)
            return new Square(MatrixValue.Floor, identifier);

        foreach (var mapping in CellValueMappings)
        {
            if (!cellValue.StartsWith(mapping.Key))
                continue;

            // If there's more content after the symbol, try to parse it as a number
            if (cellValue.Length > mapping.Key.Length)
            {
                string numberPart = cellValue.Substring(mapping.Key.Length);
                int.TryParse(numberPart, out identifier);
            }
            return new Square(mapping.Value, identifier);
        }
        throw new ArgumentException("Invalid cell value: " + cellValue);
    }

    public class Square
    {
        public Square(MatrixValue value, int identifier)
        {
            Value = value;
            Identifier = identifier;
        }

        public MatrixValue Value { get; set; }
        public int Identifier { get; set; }
        public List<Vector2Int> DefaultBehaviour { get; set; }
    }

    public enum MatrixValue
    {
        Empty, // x
        Floor, // cell with no value
        Spawn, // s
        Exit, // e
        Pawn, // ♟
        Rook, // ♖
        Bishop, // ♗
        Knight, // ♘
        Queen, // ♛
        King, // ♚
        PK, // pk
        PB, // pb
        PR // pt
    }

    private static readonly Dictionary<string, MatrixValue> CellValueMappings =
        new()
        {
            { "x", MatrixValue.Empty },
            { "X", MatrixValue.Empty },
            { "s", MatrixValue.Spawn },
            { "S", MatrixValue.Spawn },
            { "e", MatrixValue.Exit },
            { "E", MatrixValue.Exit },
            { "♟", MatrixValue.Pawn },
            { "♖", MatrixValue.Rook },
            { "♘", MatrixValue.Knight },
            { "♗", MatrixValue.Bishop },
            { "♛", MatrixValue.Queen },
            { "♚", MatrixValue.King },
            { "pk", MatrixValue.PK },
            { "pb", MatrixValue.PB },
            { "pr", MatrixValue.PR }
        };
}

using System;
using System.Collections.Generic;
using Gambetto.Scripts.Utils;
using UnityEngine;

namespace Gambetto.Scripts.GameCore.Room
{
    [CreateAssetMenu(fileName = "RoomLayout", menuName = "ScriptableObjects/RoomLayout")]
    public class RoomLayout : ScriptableObject
    {
        [SerializeField]
        public string topologyDescription;

        [SerializeField]
        private TextAsset roomCsvTextAsset;

        [SerializeField]
        private Vector2Int exitSide = Directions.North;

        private Square[,] _roomSquares;

        public Square[,] Squares => _roomSquares;

        [SerializeField]
        private List<Behaviour> behaviours;
    
        public List<Behaviour> Behaviours => behaviours;
    
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

        /// <summary>
        /// Loads the room data from the CSV file. And loads the default behaviours for the pieces.
        /// </summary>
        /// <returns>The created square matrix</returns>
        public Square[,] LoadRoomData()
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

            for (int i = rows - 1; i >= 0; i--)
            {
                string[] cells = lines[rows - 1 - i].Split(',');

                for (int j = cells.Length - 1; j >= 0; j--)
                {
                    matrix[i, j] = ParseCellValue(cells[cells.Length - 1 - j].Trim());
                }
            }
            return matrix;
        }

        private Square ParseCellValue(string cellValue)
        {
            var identifier = 0; // Default value if no number is found
            cellValue = cellValue.Replace(" ", ""); // Remove spaces
            cellValue = cellValue.ToUpper(); //transform the cellvalue to uppercase
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
            Pawn, // ♟ p P
            Rook, // ♖ r R
            Bishop, // ♗ b B
            Knight, // ♘ kn KN
            Queen, // ♛ q Q
            King, // ♚ k K
            PK, // pk
            PB, // pb
            PR, // pt
            PP // pp
        }

        private static readonly Dictionary<string, MatrixValue> CellValueMappings =
            new()
            {
                { "PK", MatrixValue.PK },
                { "PB", MatrixValue.PB },
                { "PR", MatrixValue.PR },
                { "PP", MatrixValue.PP },
                { "X", MatrixValue.Empty },
                { "S", MatrixValue.Spawn },
                { "E", MatrixValue.Exit },
                { "♟", MatrixValue.Pawn },
                { "P", MatrixValue.Pawn },
                { "♖", MatrixValue.Rook },
                { "R", MatrixValue.Rook },
                { "♘", MatrixValue.Knight },
                { "KN", MatrixValue.Knight },
                { "♗", MatrixValue.Bishop },
                { "B", MatrixValue.Bishop },
                { "♛", MatrixValue.Queen },
                { "Q", MatrixValue.Queen },
                { "♚", MatrixValue.King },
                { "K", MatrixValue.King }
            };
    }
}

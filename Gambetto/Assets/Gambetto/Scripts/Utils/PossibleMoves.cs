using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class PossibleMoves
    {
        public static readonly List<Vector2Int> PawnPossibleMoves = new List<Vector2Int>
        {
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1),
        };
        
        public static readonly List<Vector2Int> KingPossibleMoves = new List<Vector2Int>
        {
            new Vector2Int(1, 0),
            new Vector2Int(1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1)
        };
        
        public static readonly List<Vector2Int> RookPossibleMoves = new List<Vector2Int>
        {
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1)
        };
        
        public static readonly List<Vector2Int> KnightPossibleMoves = new List<Vector2Int>
        {
            new Vector2Int(1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            
            new Vector2Int(1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1),
            
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            
            new Vector2Int(0, 1),
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            
            new Vector2Int(0, 1),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            
            new Vector2Int(0, -1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            
            new Vector2Int(0, -1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0)
        };
        
        public static readonly List<Vector2Int> BishopPossibleMoves = new List<Vector2Int>
        {
            new Vector2Int(1, 1),
            new Vector2Int(-1, -1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1)
        };
        
        public static readonly List<Vector2Int> QueenPossibleMoves = new List<Vector2Int>
        {
            new Vector2Int(1, 0),
            new Vector2Int(1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1)
        };
    }
}
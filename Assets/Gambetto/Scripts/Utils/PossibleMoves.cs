using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class PossibleMoves
    {
        public static readonly List<Vector2> PawnPossibleMoves = new List<Vector2>
        {
            new Vector2(0, 1)
        };
        
        //rook can move up to 10 squares in any direction
        public static readonly List<Vector2> RookPossibleMoves = new List<Vector2>
        {
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(-1, 0),
            new Vector2(0, -1)
        };
        
        //knight can move 2 squares in any direction and then 1 square in a perpendicular direction
        public static readonly List<Vector2> KnightPossibleMoves = new List<Vector2>
        {
            new Vector2(1, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            
            new Vector2(1, 0),
            new Vector2(1, 0),
            new Vector2(0, -1),
            
            new Vector2(-1, 0),
            new Vector2(-1, 0),
            new Vector2(0, -1),
            
            new Vector2(-1, 0),
            new Vector2(-1, 0),
            new Vector2(0, 1),
            
            new Vector2(0, 1),
            new Vector2(0, 1),
            new Vector2(1, 0),
            
            new Vector2(0, 1),
            new Vector2(0, 1),
            new Vector2(-1, 0),
            
            new Vector2(0, -1),
            new Vector2(0, -1),
            new Vector2(-1, 0),
            
            new Vector2(0, -1),
            new Vector2(0, -1),
            new Vector2(1, 0)
        };
        
        //bishop can move up to 10 squares in any diagonal direction
        public static readonly List<Vector2> BishopPossibleMoves = new List<Vector2>
        {
            new Vector2(1, 1),
            new Vector2(-1, -1),
            new Vector2(1, -1),
            new Vector2(-1, 1)
        };
        
        //queen can move up to 5 squares in any direction
        public static readonly List<Vector2> QueenPossibleMoves = new List<Vector2>
        {
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(-1, 0),
            new Vector2(-1, -1),
            new Vector2(0, -1),
            new Vector2(1, -1),
            new Vector2(-1, 1)
        };
        
        //king can move up to 1 square in any direction
        public static readonly List<Vector2> KingPossibleMoves = new List<Vector2>
        {
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(-1, 0),
            new Vector2(-1, -1),
            new Vector2(0, -1),
            new Vector2(1, -1),
            new Vector2(-1, 1)
        };
    }
}
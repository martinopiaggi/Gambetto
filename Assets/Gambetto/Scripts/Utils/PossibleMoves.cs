using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class PossibleMoves
    {
        public static readonly List<Vector2> PawnPossibleMoves = new List<Vector2>
        {
            new Vector2(0, 1),
            new Vector2(0, 2),
            new Vector2(1, 1),
            new Vector2(-1, 1)
        };
        
        //rook can move up to 10 squares in any direction
        public static readonly List<Vector2> RookPossibleMoves = new List<Vector2>
        {
            new Vector2(0, 1),
            new Vector2(0, 2),
            new Vector2(0, 3),
            new Vector2(0, 4),
            new Vector2(0, 5),
            new Vector2(0, 6),
            new Vector2(0, 7),
            new Vector2(0, 8),
            new Vector2(0, 9),
            new Vector2(0, 10),
            new Vector2(0, -1),
            new Vector2(0, -2),
            new Vector2(0, -3),
            new Vector2(0, -4),
            new Vector2(0, -5),
            new Vector2(0, -6),
            new Vector2(0, -7),
            new Vector2(0, -8),
            new Vector2(0, -9),
            new Vector2(0, -10),
            new Vector2(1, 0),
            new Vector2(2, 0),
            new Vector2(3, 0),
            new Vector2(4, 0),
            new Vector2(5, 0),
            new Vector2(6, 0),
            new Vector2(7, 0),
            new Vector2(8, 0),
            new Vector2(9, 0),
            new Vector2(10, 0),
            new Vector2(-1, 0),
            new Vector2(-2, 0),
            new Vector2(-3, 0),
            new Vector2(-4, 0),
            new Vector2(-5, 0),
            new Vector2(-6, 0),
            new Vector2(-7, 0),
            new Vector2(-8, 0),
            new Vector2(-9, 0),
            new Vector2(-10, 0)
        };
        
        //knight can move 2 squares in any direction and then 1 square in a perpendicular direction
        public static readonly List<Vector2> KnightPossibleMoves = new List<Vector2>
        {
            new Vector2(1, 2),
            new Vector2(1, -2),
            new Vector2(-1, 2),
            new Vector2(-1, -2),
            new Vector2(2, 1),
            new Vector2(2, -1),
            new Vector2(-2, 1),
            new Vector2(-2, -1)
        };
        
        //bishop can move up to 10 squares in any diagonal direction
        public static readonly List<Vector2> BishopPossibleMoves = new List<Vector2>
        {
            new Vector2(1, 1),
            new Vector2(2, 2),
            new Vector2(3, 3),
            new Vector2(4, 4),
            new Vector2(5, 5),
            new Vector2(6, 6),
            new Vector2(7, 7),
            new Vector2(8, 8),
            new Vector2(9, 9),
            new Vector2(10, 10),
            new Vector2(-1, -1),
            new Vector2(-2, -2),
            new Vector2(-3, -3),
            new Vector2(-4, -4),
            new Vector2(-5, -5),
            new Vector2(-6, -6),
            new Vector2(-7, -7),
            new Vector2(-8, -8),
            new Vector2(-9, -9),
            new Vector2(-10, -10),
            new Vector2(1, -1),
            new Vector2(2, -2),
            new Vector2(3, -3),
            new Vector2(4, -4),
            new Vector2(5, -5),
            new Vector2(6, -6),
            new Vector2(7, -7),
            new Vector2(8, -8),
            new Vector2(9, -9),
            new Vector2(10, -10),
            new Vector2(-1, 1),
            new Vector2(-2, 2),
            new Vector2(-3, 3),
            new Vector2(-4, 4),
            new Vector2(-5, 5),
            new Vector2(-6, 6),
            new Vector2(-7, 7),
            new Vector2(-8, 8),
            new Vector2(-9, 9),
            new Vector2(-10, 10)
        };
        
        //queen can move up to 5 squares in any direction
        public static readonly List<Vector2> QueenPossibleMoves = new List<Vector2>
        {
            new Vector2(0, 1),
            new Vector2(0, 2),
            new Vector2(0, 3),
            new Vector2(0, 4),
            new Vector2(0, 5),
            new Vector2(0, -1),
            new Vector2(0, -2),
            new Vector2(0, -3),
            new Vector2(0, -4),
            new Vector2(0, -5),
            new Vector2(1, 0),
            new Vector2(2, 0),
            new Vector2(3, 0),
            new Vector2(4, 0),
            new Vector2(5, 0),
            new Vector2(-1, 0),
            new Vector2(-2, 0),
            new Vector2(-3, 0),
            new Vector2(-4, 0),
            new Vector2(-5, 0),
            new Vector2(1, 1),
            new Vector2(2, 2),
            new Vector2(3, 3),
            new Vector2(4, 4),
            new Vector2(5, 5),
            new Vector2(-1, -1),
            new Vector2(-2, -2),
            new Vector2(-3, -3),
            new Vector2(-4, -4),
            new Vector2(-5, -5),
            new Vector2(1, -1),
            new Vector2(2, -2),
            new Vector2(3, -3),
            new Vector2(4, -4),
            new Vector2(5, -5),
            new Vector2(-1, 1),
            new Vector2(-2, 2),
            new Vector2(-3, 3),
            new Vector2(-4, 4),
            new Vector2(-5, 5)
        };
        
        //king can move up to 1 square in any direction
        public static readonly List<Vector2> KingPossibleMoves = new List<Vector2>
        {
            new Vector2(0, 1),
            new Vector2(0, -1),
            new Vector2(1, 0),
            new Vector2(-1, 0),
            new Vector2(1, 1),
            new Vector2(1, -1),
            new Vector2(-1, 1),
            new Vector2(-1, -1)
        };
    }
}
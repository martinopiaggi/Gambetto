using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pieces
{
    public class Pawn : Piece
    {
        private protected void Awake()
        {
            pieceType = PieceType.Pawn;
            possibleMoves = new List<Vector2>
            {
                new Vector2(0, 1),
                new Vector2(1, 0),
                new Vector2(0, -1),
                new Vector2(-1, 0)
            };
            maxCountdown = Utils.PieceCountdowns.Pawn;
            countDown = maxCountdown;
            base.Awake();
        }
    }
}

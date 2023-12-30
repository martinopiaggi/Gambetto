using System.Collections.Generic;

namespace Gambetto.Scripts.GameCore.Piece
{
    public static class PieceConstants
    {
        /// <summary>
        /// Dictionary of clock period values for each piece.
        /// </summary>
        public static readonly Dictionary<PieceType, float> TypesClockPeriods =
            new()
            {
                { PieceType.Pawn, 2.5f },
                { PieceType.Bishop, 3.7f },
                { PieceType.Knight, 3f },
                { PieceType.Rook, 3f },
                { PieceType.King, 2.5f }, //king not implemented
                { PieceType.Queen, 2.5f } //queen not implemented
            };

        public const float PieceSpeed = 10f;
    }
}

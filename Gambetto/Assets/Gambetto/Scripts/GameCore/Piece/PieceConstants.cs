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

        public const int MaxPieceCountdown = 20;
        public const int MinPieceCountdown = 1;

        /// <summary>
        /// Countdown values for each piece.
        /// </summary>
        public enum PieceCountdown
        {
            Pawn = 1,
            Rook = 2,
            Knight = 2,
            Bishop = 2,
            Queen = 1,
            King = 1
        }

        public const float PieceSpeed = 10f;
    }
}

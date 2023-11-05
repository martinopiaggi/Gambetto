﻿namespace Gambetto.Scripts.Utils
{
    public static class Constants
    {
        public const int MaxPieceCountdown = 20;
        public const int MinPieceCountdown = 1;
        
        /// <summary>
        /// Countdown values for each piece.
        /// </summary>
        public enum PieceCountdown {
            Pawn = 1,
            Rook = 2,
            Knight = 2,
            Bishop = 2,
            Queen = 1,
            King = 1
        }
        
        public const float PieceSpeed = 0.025f;
        
        
  
    }
}
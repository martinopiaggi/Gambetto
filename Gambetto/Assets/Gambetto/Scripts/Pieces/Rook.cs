using Gambetto.Scripts.Utils;
using UnityEngine;

namespace Pieces
{
    public class Rook : Piece
    {
        ///<summary>
        ///   <para> Calls parent <see cref="Piece.Awake">Awake</see>, sets the <see cref="PieceType">Piece Type</see>, <see cref="Piece.Countdown">Countdown</see> and the possible moves for the piece</para>
        /// </summary>
        private protected void Awake()
        {
            base.Awake();
            _pieceType = PieceType.Rook;
            // Set the possible moves for the piece
            PossibleMoves = Utils.PossibleMoves.RookPossibleMoves;
            Countdown = (int) Constants.PieceCountdown.Rook;
        }
    }
}
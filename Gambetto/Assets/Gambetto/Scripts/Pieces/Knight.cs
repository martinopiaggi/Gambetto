using Gambetto.Scripts.Utils;
using UnityEngine;

namespace Gambetto.Scripts.Pieces
{
    public class Knight : Piece
    {
        ///<summary>
        ///   <para> Calls parent <see cref="Piece.Awake">Awake</see>, sets the <see cref="PieceType">Piece Type</see>, <see cref="Piece.Countdown">Countdown</see> and the possible moves for the piece</para>
        /// <para> Also sets the mesh for the piece.</para>
        /// </summary>
        private protected new void Awake()
        {
            base.Awake();
            _pieceType = PieceType.Knight;
            // Set the possible moves for the piece
            PossibleMoves = global::Utils.PossibleMoves.KnightPossibleMoves;
            Countdown = (int) Constants.PieceCountdown.Knight;
            GetComponent<MeshFilter>().mesh = Resources.Load<Mesh>("Models/Knight");
        }
    }
}
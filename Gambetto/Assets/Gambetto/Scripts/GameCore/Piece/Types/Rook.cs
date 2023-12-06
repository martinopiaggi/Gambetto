using UnityEngine;

namespace Gambetto.Scripts.GameCore.Piece.Types
{
    public class Rook : Piece
    {
        ///<summary>
        ///   <para> Calls parent <see cref="Piece.Awake">Awake</see>, sets the <see cref="PieceType">Piece Type</see>, <see cref="Piece.Countdown">Countdown</see> and the possible moves for the piece</para>
        ///     <para> Also sets the mesh for the piece.</para>     
        ///</summary>
        private protected new void Awake()
        {
            base.Awake();
            _pieceType = PieceType.Rook;
            // Set the possible moves for the piece
            PossibleMoves = global::Gambetto.Scripts.GameCore.Grid.PossibleMoves.RookPossibleMoves;
            Countdown = (int)PieceConstants.PieceCountdown.Rook;
            GetComponent<MeshFilter>().mesh = Resources.Load<Mesh>("Models/Rook");
        }
    }
}
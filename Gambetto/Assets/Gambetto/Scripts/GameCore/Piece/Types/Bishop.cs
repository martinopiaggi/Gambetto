using UnityEngine;

namespace Gambetto.Scripts.GameCore.Piece.Types
{
    public class Bishop : Piece
    {
        ///<summary>
        ///  <para> Calls parent <see cref="Piece.Awake">Awake</see>, sets the <see cref="PieceType">Piece Type</see>, <see cref="Piece.Countdown">Countdown</see> and the possible moves for the piece.</para>
        ///  <para> Also sets the mesh for the piece.</para>
        /// </summary>
        private protected new void Awake()
        {
            base.Awake();
            pieceType = PieceType.Bishop;
            // Set the possible moves for the piece
            PossibleMoves = Grid.PossibleMoves.BishopPossibleMoves;
            Countdown = (int)PieceConstants.PieceCountdown.Bishop;
            GetComponent<MeshFilter>().mesh = Resources.Load<Mesh>("Models/Bishop");
        }
    }
}

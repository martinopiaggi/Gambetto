using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gambetto.Scripts.GameCore.Piece.Types
{
    public class Knight : Piece
    {
        ///<summary>
        ///  <para> Calls parent <see cref="Piece.Awake">Awake</see>, sets the <see cref="PieceType">Piece Type</see> and the possible moves for the piece</para>
        /// <para> Also sets the mesh for the piece.</para>
        /// </summary>
        private protected new void Awake()
        {
            base.Awake();
            pieceType = PieceType.Knight;
            // Set the possible moves for the piece
            PossibleMoves = Grid.PossibleMoves.KnightPossibleMoves;
            GetComponent<MeshFilter>().mesh = Resources.Load<Mesh>("Models/Knight");
        }

        private protected override IEnumerator MoveCoroutine(
            IList<Vector3> positions,
            bool gravity = true
        )
        {
            Rb.useGravity = false; // enable/disable gravity

            foreach (var destPosition in positions)
            {
                // var text = "moving piece to " + destPosition;
                // if (Debugger.Instance != null)
                //     Debugger.Instance.Show(text, printConsole: false);

                var direction = destPosition - TR.position;
                while (direction != Vector3.zero)
                {
                    var piecePos = TR.position;
                    piecePos = Vector3.MoveTowards(
                        piecePos,
                        destPosition,
                        PieceConstants.PieceSpeed * Time.deltaTime
                    );
                    TR.position = piecePos;
                    direction = destPosition - piecePos;
                    yield return null;
                }
            }
            Rb.useGravity = true;

            // in the knight we check if its grounded after all the moves, so it cant fall while moving
            // over gaps
            if (IsGrounded() || !gravity)
                yield break;
            {
                var direction = positions[1] - positions[0];
                // if piece is not grounded and is affected by gravity, add a force to it like it was falling
                var boost = 1f;
                if (Random.Range(0f, 1f) > 0.9)
                    boost = 4f; // easter egg :)

                Rb.AddForce(direction.normalized * boost, ForceMode.Impulse);
                Rb.AddTorque(
                    Vector3.Cross(Vector3.up, direction.normalized) * boost,
                    ForceMode.Impulse
                );
            }
        }
    }
}

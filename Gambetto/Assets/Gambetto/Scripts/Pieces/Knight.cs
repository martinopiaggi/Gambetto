using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts.Utils;
using UnityEngine;

namespace Gambetto.Scripts.Pieces
{
    public class Knight : Piece
    {
        ///<summary>
        ///  <para> Calls parent <see cref="Piece.Awake">Awake</see>, sets the <see cref="PieceType">Piece Type</see>, <see cref="Piece.Countdown">Countdown</see> and the possible moves for the piece</para>
        /// <para> Also sets the mesh for the piece.</para>
        /// </summary>
        private protected new void Awake()
        {
            base.Awake();
            _pieceType = PieceType.Knight;
            // Set the possible moves for the piece
            PossibleMoves = global::Utils.PossibleMoves.KnightPossibleMoves;
            Countdown = (int)Constants.PieceCountdown.Knight;
            GetComponent<MeshFilter>().mesh = Resources.Load<Mesh>("Models/Knight");
        }

        private protected override IEnumerator MoveCoroutine(
            IList<Vector3> positions,
            bool gravity = true
        )
        {
            _rb.useGravity = false; // enable/disable gravity

            foreach (var destPosition in positions)
            {
                // var text = "moving piece to " + destPosition;
                // if (Debugger.Instance != null)
                //     Debugger.Instance.Show(text, printConsole: false);

                var direction = destPosition - _tr.position;
                while (direction != Vector3.zero)
                {
                    var piecePos = _tr.position;
                    piecePos = Vector3.MoveTowards(
                        piecePos,
                        destPosition,
                        Constants.PieceSpeed * Time.deltaTime
                    );
                    _tr.position = piecePos;
                    direction = destPosition - piecePos;
                    yield return null;
                }
            }
            _rb.useGravity = true;

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

                _rb.AddForce(direction.normalized * boost, ForceMode.Impulse);
                _rb.AddTorque(
                    Vector3.Cross(Vector3.up, direction.normalized) * boost,
                    ForceMode.Impulse
                );
            }
        }
    }
}

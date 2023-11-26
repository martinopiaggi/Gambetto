using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts.Utils;
using UnityEngine;
using Utils;

namespace Gambetto.Scripts.Pieces
{
    public abstract class Piece : MonoBehaviour
    {
        private protected PieceType _pieceType;

        [SerializeField]
        private protected PieceRole pieceRole;

        [SerializeField]
        private protected List<Vector2Int> possibleMoves;

        [Range(Constants.MinPieceCountdown, Constants.MaxPieceCountdown)]
        [SerializeField]
        private protected int countdown;

        [SerializeField]
        private protected Constants.PieceCountdown countdownStartValue;
        private Transform _tr;
        private Rigidbody _rb;
        private protected Mesh _mesh;

        public PieceRole PieceRole => pieceRole;
        public PieceType PieceType => _pieceType;

        /// <summary>
        /// List of possible moves for the piece.
        /// </summary>
        public List<Vector2Int> PossibleMoves
        {
            get => possibleMoves ?? new List<Vector2Int>();
            set => possibleMoves = value;
        }

        /// <summary>
        /// Active countdown after which the piece will move.
        /// </summary>
        public int Countdown
        {
            get => countdown;
            set
            {
                switch (value)
                {
                    case < Constants.MinPieceCountdown:
                        Debug.LogError(
                            "Piece countdown cannot be less than " + Constants.MinPieceCountdown
                        );
                        countdown = Constants.MinPieceCountdown;
                        break;
                    case > Constants.MaxPieceCountdown:
                        Debug.LogError(
                            "Piece countdown cannot be more than " + Constants.MaxPieceCountdown
                        );
                        countdown = Constants.MaxPieceCountdown;
                        break;
                    default:
                        countdown = value;
                        break;
                }
            }
        }

        /// <summary>
        ///  Value of the countdown when <see cref="Piece"/> gets initialized.
        /// </summary>
        public Constants.PieceCountdown CountDownStartValue
        {
            get => countdownStartValue;
            set => countdownStartValue = value;
        }

        ///<summary>
        ///  <para> On Awake, sets the references to the Transform and Mesh components.</para>
        /// </summary>
        private protected void Awake()
        {
            _tr = GetComponent<Transform>();
            _mesh = GetComponent<MeshFilter>().mesh;
            _rb = GetComponent<Rigidbody>();
        }

        /**
         * <summary>
         * Moves the piece smoothly following a given list of positions when <see cref="Countdown"/> reaches <see cref="Constants.MinPieceCountdown"/>.
         * </summary>
         * <param name="positions">The list of positions to follow</param>
         * <param name="gravity">Whether the piece should be affected by gravity or not</param>
         */
        public void Move(List<Vector3> positions, bool gravity = true)
        {
            StartCoroutine(MoveCoroutine(positions, gravity));
            //AudioManager.Instance.PlaySfx(AudioManager.Instance.pawnMovement);
        }

        private IEnumerator MoveCoroutine(IList<Vector3> positions, bool gravity = true)
        {
            if (!gravity)
                _rb.useGravity = false;
            foreach (var destPosition in positions)
            {
                yield return new WaitForSeconds(0.1f);
                var text = "moving piece to " + destPosition;
                if (Debugger.Instance != null)
                    Debugger.Instance.Show(text, printConsole: false);
                var direction = destPosition - _tr.position;
                while (direction != Vector3.zero)
                {
                    if (!IsGrounded() && gravity)
                    {
                        var boost = 2f;
                        if (Random.Range(0f,1f) > 0.9) boost = 10f;
                        _rb.AddForce(direction.normalized*boost, ForceMode.Impulse);
                        break;
                    }
                    var piecePos = _tr.position;
                    piecePos = Vector3.MoveTowards(piecePos, destPosition, Constants.PieceSpeed);
                    _tr.position = piecePos;
                    direction = destPosition - piecePos;
                    yield return null;
                }
            }
            _rb.useGravity = true;
        }

        private bool IsGrounded()
        {
            return Physics.Raycast(transform.position, Vector3.down, 10.0f);
        }
    }

    public enum PieceType
    {
        Rook,
        Knight,
        Bishop,
        Queen,
        King,
        Pawn
    }

    public enum PieceRole
    {
        Player,
        Enemy,
        Neutral
    }
}

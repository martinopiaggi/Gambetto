using System;
using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts.Utils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Utils;
using Random = UnityEngine.Random;

namespace Gambetto.Scripts.Pieces
{
    public abstract class Piece : MonoBehaviour
    {
        private protected PieceType _pieceType;

        [SerializeField]
        private protected PieceRole pieceRole;

        private bool _patternAI = false;
        private Behaviour _pattern = null;
        private int _patternIndex = 0;
        
        public bool PatternAI
        {
            get => _patternAI;
            set => _patternAI = value;
        }
        
        public Behaviour Pattern
        {
            set => _pattern = value;
            get => _pattern;
        }
        
        
        [SerializeField]
        private protected List<Vector2Int> possibleMoves;

        [Range(Constants.MinPieceCountdown, Constants.MaxPieceCountdown)]
        [SerializeField]
        private protected int countdown;

        [SerializeField]
        private protected Constants.PieceCountdown countdownStartValue;
        private protected Transform _tr;
        private protected Rigidbody _rb;

        private Coroutine _moveCoroutine;
        private List<Vector3> _oldPositions;

        public PieceRole PieceRole
        {
            get => pieceRole;
            set => pieceRole = value;
        }

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
            _rb = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Enemy") && pieceRole == PieceRole.Player)
            {
                _rb.useGravity = true; // force gravity (needed for the knight)
                // Debug.Log("Enemy hit");
                var direction = collision.transform.position - _tr.position;
                // add a force to the player in the opposite direction of the enemy to simulate a hit
                _rb.AddForce(-direction.normalized * 8f + Vector3.up, ForceMode.Impulse);
                var gridManger = FindObjectOfType<GridManager>();
                StartCoroutine(gridManger.RestartLevel());
            }
        }

        /**
         * <summary>
         * Moves the piece smoothly following a given list of positions when <see cref="Countdown"/> reaches <see cref="Constants.MinPieceCountdown"/>.
         * The piece is moved ONLY if the distance between the current position and the destination is greater than 0.1f.
         * </summary>
         * <param name="positions">The list of positions to follow</param>
         * <param name="gravity">Whether the piece should be affected by gravity or not</param>
         */
        public void Move(List<Vector3> positions, bool gravity = true)
        {
            if (_moveCoroutine != null)
            {
                // if a piece is still moving, stop it and force the position
                StopCoroutine(_moveCoroutine);
                _tr.position = _oldPositions[_oldPositions.Count - 1];
            }

            _oldPositions = positions;
            _moveCoroutine = StartCoroutine(MoveCoroutine(positions, gravity));
        }

        private protected virtual IEnumerator MoveCoroutine(
            IList<Vector3> positions,
            bool gravity = true
        )
        {
            _rb.useGravity = gravity; // enable/disable gravity

            foreach (var destPosition in positions)
            {
                // var text = "moving piece to " + destPosition;
                // if (Debugger.Instance != null)
                //     Debugger.Instance.Show(text, printConsole: false);

                var direction = destPosition - _tr.position;
                while (direction != Vector3.zero)
                {
                    if (!IsGrounded() && gravity)
                    {
                        _rb.useGravity = true; // force gravity (needed for the knight)
                        // if piece is not grounded and is affected by gravity, add a force to it like it was falling
                        var boost = 8f;
                        if (Random.Range(0f, 1f) > 0.9)
                            boost = 16f; // easter egg :)

                        _rb.AddForce(direction.normalized * boost, ForceMode.Impulse);
                        _rb.AddTorque(
                            Vector3.Cross(Vector3.up, direction.normalized) * boost,
                            ForceMode.Impulse
                        );
                        break;
                    }

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
        }

        private protected bool IsGrounded()
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

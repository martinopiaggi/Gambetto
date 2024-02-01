using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts.GameCore.Grid;
using Gambetto.Scripts.Utils;
using UnityEngine;
using Behaviour = Gambetto.Scripts.GameCore.Room.Behaviour;
using Random = UnityEngine.Random;

namespace Gambetto.Scripts.GameCore.Piece
{
    public abstract class Piece : MonoBehaviour
    {
        // ReSharper disable once InconsistentNaming
        private protected PieceType pieceType;

        [SerializeField]
        private protected PieceRole pieceRole;

        //private int _patternIndex = 0;

        public bool HasPattern { get; set; }

        private Behaviour _behaviour;

        public Behaviour Behaviour
        {
            get => _behaviour;
            set
            {
                _behaviour = value;
                IsAwake = _behaviour.Aggressive;
                animator.SetBool("Aggressive", _behaviour.Aggressive);
            }
        }

        private bool isAwake = false;

        public bool IsAwake
        {
            get => isAwake;
            set
            {
                isAwake = value;
                SetAnimatorInRange(isAwake);
            }
        }

        [SerializeField]
        private protected List<Vector2Int> possibleMoves;

        private protected Transform TR;
        private protected Rigidbody Rb;
        private protected Collider[] _colliders;

        private Coroutine _moveCoroutine;
        private List<Vector3> _oldPositions;

        private Animator animator;
        public PieceRole PieceRole
        {
            get => pieceRole;
            set => pieceRole = value;
        }

        public PieceType PieceType => pieceType;

        /// <summary>
        /// List of possible moves for the piece.
        /// </summary>
        public List<Vector2Int> PossibleMoves
        {
            get => possibleMoves ?? new List<Vector2Int>();
            set => possibleMoves = value;
        }

        ///<summary>
        ///  <para> On Awake, sets the references to the Transform and Mesh components.</para>
        /// </summary>
        private protected void Awake()
        {
            TR = GetComponent<Transform>();
            Rb = GetComponent<Rigidbody>();
            animator = GetComponentInChildren<Animator>();
            _colliders = GetComponents<Collider>();
            _hasCollided = false; //todo: temp solution for multiple collision issue
        }

        private bool _hasCollided;

        private void OnCollisionEnter(Collision collision)
        {
            if (
                collision.gameObject.CompareTag("Enemy")
                && pieceRole == PieceRole.Player
                && !_hasCollided
            )
            {
                GameClock.Instance.StopClock();
                // solution for multiple triggered collision issue
                _hasCollided = true;
                // stop the piece from moving
                if (_moveCoroutine != null)
                    StopCoroutine(_moveCoroutine);
                // force gravity (needed for the knight)
                Rb.useGravity = true;
                var direction = collision.transform.position - TR.position;
                // add a force to the player in the opposite direction of the enemy to simulate a hit
                Rb.AddForce(-direction.normalized * 10f + Vector3.up, ForceMode.Impulse);
                AudioManager.Instance.PlaySfx(AudioManager.Instance.deathByCollision);
                var gridManager = FindObjectOfType<GridManager>();
                gridManager.playerController.choosing = false;
                gridManager.isDead = true;
                gridManager.StartCoroutine(gridManager.ShowDelayed(gridManager.deathScreen, 1.42f));
                gridManager.pauseButton.SetActive(false);
            }
        }

        /**
         * <summary>
         * Moves the piece smoothly following a given list of positions.
         * The piece is moved ONLY if the distance between the current position and the destination is greater than 0.1f.
         * </summary>
         * <param name="positions">The list of positions to follow</param>
         * <param name="gravity">Whether the piece should be affected by gravity or not while moving</param>
         */
        public void Move(List<Vector3> positions, bool gravity = true)
        {
            if (Vector3.Distance(transform.position, positions[^1]) < 0.1f)
                return;
            if (gravity) EnableColliders();
            if (_moveCoroutine != null)
            {
                // if a piece is still moving, stop it and force the position
                StopCoroutine(_moveCoroutine);
                TR.position = _oldPositions[_oldPositions.Count - 1];
            }

            _oldPositions = positions;
            _moveCoroutine = StartCoroutine(MoveCoroutine(positions, gravity));
        }

        private protected virtual IEnumerator MoveCoroutine(
            IList<Vector3> positions,
            bool gravity = true
        )
        {
            if (!gravity) DisableColliders();
            
            // disable collider if gravity is disabled
            Rb.useGravity = gravity; // enable/disable gravity

            foreach (var destPosition in positions)
            {
                // var text = "moving piece to " + destPosition;
                // if (Debugger.Instance != null)
                //     Debugger.Instance.Show(text, printConsole: false);

                var direction = destPosition - TR.position;
                while (direction != Vector3.zero)
                {
                    if (!IsGrounded() && gravity)
                    {
                        Rb.useGravity = true; // force gravity (needed for the knight)
                        // if piece is not grounded and is affected by gravity, add a force to it like it was falling
                        var boost = 5f;
                        if (Random.Range(0f, 1f) > 0.9)
                            boost = 12f; // easter egg :)

                        Rb.AddForce(direction.normalized * boost, ForceMode.Impulse);
                        Rb.AddTorque(
                            Vector3.Cross(Vector3.up, direction.normalized) * boost,
                            ForceMode.Impulse
                        );
                        break;
                    }

                    var piecePos = TR.position;
                    var distance = Vector3.Distance(piecePos, destPosition);
                    // if the distance is less then 5 is set to 5 to avoid the movement to be to slow
                    if (distance < 2.0f) 
                        distance = 2.0f;
                    var inputTime = TimeManager.InputTimeInterval;
                    piecePos = Vector3.MoveTowards(
                        piecePos,
                        destPosition,
                        distance * (Time.deltaTime / inputTime)
                    );
                    TR.position = piecePos;
                    direction = destPosition - piecePos;
                    yield return null;
                }
            }
        }

        private protected bool IsGrounded()
        {
            return Physics.Raycast(transform.position, Vector3.down, 10.0f);
        }

        private protected void DisableColliders()
        {
            foreach (var col in _colliders)
            {
                col.enabled = false;
            }
        }

        private protected void EnableColliders()
        {
            foreach (var col in _colliders)
            {
                col.enabled = true;
            }
        }

        public void EnableGravity()
        {
            Rb.useGravity = true;
            EnableColliders();
        }

        public void SetIsKinematic(bool value)
        {
            Rb.isKinematic = value;
        }

        /// <summary>
        /// If set to true and <see cref="Behaviour"/> <b>isn't</b> aggressive, triggers the animation for the speech bubble.
        /// </summary>
        /// <param name="value"></param>
        private void SetAnimatorInRange(bool value)
        {
            if (value && !Behaviour.Aggressive && !animator.GetBool("InRange"))
                AudioManager.Instance.PlaySfx(AudioManager.Instance.enemyAlerted);
            animator.SetBool("InRange", value);
        }

        public void ResetAndMovePiece(List<Vector3> moves)
        {
            IsAwake = Behaviour.Aggressive;
            Move(moves, false);
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

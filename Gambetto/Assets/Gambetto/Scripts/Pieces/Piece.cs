using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Pieces
{
    public abstract class Piece : MonoBehaviour
    {
        private protected PieceType _pieceType;
        [SerializeField] private protected PieceRole pieceRole;
        [SerializeField] private protected List<Vector2> possibleMoves;

        [Range(Constants.MinPieceCountdown, Constants.MaxPieceCountdown)] [SerializeField]
        private protected int countdown;

        [SerializeField] private protected Constants.PieceCountdown countdownStartValue;
        private Transform _tr;
        private protected Mesh _mesh;

        public PieceRole PieceRole => pieceRole;
        public PieceType PieceType => _pieceType;

        /// <summary>
        /// List of possible moves for the piece.
        /// </summary>
        public List<Vector2> PossibleMoves
        {
            get => possibleMoves ?? new List<Vector2>();
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
                        Debug.LogError("Piece countdown cannot be less than " + Constants.MinPieceCountdown);
                        countdown = Constants.MinPieceCountdown;
                        break;
                    case > Constants.MaxPieceCountdown:
                        Debug.LogError("Piece countdown cannot be more than " + Constants.MaxPieceCountdown);
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
        }

        /**
         * <summary>
         * Moves the piece smoothly following a given list of positions when <see cref="Countdown"/> reaches <see cref="Constants.MinPieceCountdown"/>. 
         * </summary>
         * <param name="positions">The list of positions to follow</param>
         */
        public void Move(List<Vector3> positions)
        {
            if (countdown == Constants.MinPieceCountdown)
            {
                StartCoroutine(MoveCoroutine(positions));
                AudioManager.Instance.PlaySfx(AudioManager.Instance.pawnMovement);
                countdown = (int)countdownStartValue;
                return;
            }

            countdown--;
        }

        private IEnumerator MoveCoroutine(IList<Vector3> positions)
        {
            foreach (var destPosition in positions)
            {
                yield return new WaitForSeconds(1f);
                var text = "moving piece to " + destPosition;
                if (Debugger.Instance != null)
                    Debugger.Instance.Show(text, printConsole: false);
                var direction = destPosition - _tr.position;
                while (direction != Vector3.zero)
                {
                    var piecePos = _tr.position;
                    piecePos = Vector3.MoveTowards(piecePos, destPosition, Constants.PieceSpeed);
                    _tr.position = piecePos;
                    direction = destPosition - piecePos;
                    yield return null;
                }
            }
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
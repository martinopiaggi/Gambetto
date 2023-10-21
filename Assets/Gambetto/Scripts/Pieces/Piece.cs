using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Pieces
{
    public class Piece : MonoBehaviour
    {
        [SerializeField] private PieceType pieceType;
        [SerializeField] private PieceRole pieceRole;
        [SerializeField] private List<Vector2> possibleMoves;

        [Range(Constants.MinPieceCountdown, Constants.MaxPieceCountdown)] [SerializeField]
        private int countdown;

        [SerializeField] private Constants.PieceCountdown startCountdown;
        private Transform _tr;

        public PieceType PieceType => pieceType;
        public PieceRole PieceRole => pieceRole;

        public List<Vector2> PossibleMoves
        {
            get => possibleMoves ?? new List<Vector2>();
            set => possibleMoves = value;
        }

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

        public Constants.PieceCountdown StartCountdown
        {
            get => startCountdown;
            set => startCountdown = value;
        }

        private protected void Awake()
        {
            _tr = GetComponent<Transform>();
            possibleMoves = PieceType switch
            {
                PieceType.Pawn => Utils.PossibleMoves.PawnPossibleMoves,
                PieceType.Rook => Utils.PossibleMoves.RookPossibleMoves,
                PieceType.Knight => Utils.PossibleMoves.KnightPossibleMoves,
                PieceType.Bishop => Utils.PossibleMoves.BishopPossibleMoves,
                PieceType.Queen => Utils.PossibleMoves.QueenPossibleMoves,
                PieceType.King => Utils.PossibleMoves.KingPossibleMoves,
                _ => new List<Vector2>()
            };
        }

        public void MovePiece(List<Vector3> positions)
        {
            if (countdown == Constants.MinPieceCountdown)
            {
                StartCoroutine(MovePieceCoroutine(positions));
                countdown = (int)startCountdown;
                return;
            }

            countdown--;
        }

        private IEnumerator MovePieceCoroutine(IList<Vector3> positions)
        {
            foreach (var destPosition in positions)
            {
                yield return new WaitForSeconds(1f);
                var text = "moving piece to " + destPosition;
                Debugger.Instance.Show(text, Color.red, Debugger.Position.UpperLeft);
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
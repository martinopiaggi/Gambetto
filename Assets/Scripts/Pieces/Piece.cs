using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Pieces
{
    public class Piece : MonoBehaviour
    {
        [SerializeField] private protected PieceType pieceType;
        [SerializeField] private protected PieceRole pieceRole;
        [SerializeField] private protected List<Vector2> possibleMoves;
        [Range(Constants.MinPieceCountdown, Constants.MaxPieceCountdown)]
        [SerializeField] private protected int countdown;
        [SerializeField] private protected Constants.PieceCountdown startCountdown;
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
        }

        public void MovePiece(List<Vector3> positions)
        {
            if (countdown ==  Constants.MinPieceCountdown)
            {
                StartCoroutine(MovePieceCoroutine(positions));
                countdown = (int) startCountdown;
                return;
            }
            countdown--;
        }
    
        private IEnumerator MovePieceCoroutine(List<Vector3> positions)
        {
            yield return new WaitForSeconds(1f);
            if (positions.Count <= 0) yield break;
            
            var direction = positions[0] - _tr.position;
            while(direction != Vector3.zero)
            {
                var position = _tr.position;
                position = Vector3.MoveTowards(position, positions[0], Constants.PieceSpeed);
                _tr.position = position;
                direction = positions[0] - position;
                yield return null;
            }
            
            positions.RemoveAt(0);
            StartCoroutine(MovePieceCoroutine(positions));
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
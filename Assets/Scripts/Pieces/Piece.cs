using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pieces
{
    public abstract class Piece : MonoBehaviour
    {
        [SerializeField] private protected PieceType pieceType;
        [SerializeField] private protected PieceRole pieceRole;
        [SerializeField] private protected List<Vector2> possibleMoves;
        [SerializeField] private protected int countDown;
        [SerializeField] private protected int maxCountdown;
        private Transform tr;

        private protected void Awake()
        {
            tr = GetComponent<Transform>();
        }

        public void MovePiece(List<Vector3> positions)
        {
            if (countDown == 1)
            {
                StartCoroutine(MovePieceCoroutine(positions));
                countDown = maxCountdown;
                return;
            }
            countDown--;
        }
    
        private IEnumerator MovePieceCoroutine(List<Vector3> positions)
        {
            yield return new WaitForSeconds(0.5f);
            if (positions.Count <= 0) yield break;
            tr.position = positions[0];
            positions.RemoveAt(0);
            StartCoroutine(MovePieceCoroutine(positions));
        }
    
        public List<Vector2> GetPossibleMoves()
        {
            return possibleMoves ?? new List<Vector2>();
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
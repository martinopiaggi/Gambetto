using System.Collections.Generic;
using UnityEngine;

namespace Gambetto.Scripts.GameCore.Room
{
    [CreateAssetMenu(fileName = "Behaviour", menuName = "ScriptableObjects/Behaviour")]
    public class Behaviour : ScriptableObject
    {
        [SerializeField]
        private List<Vector2Int> _movements = new();

        [SerializeField]
        private int _offset;

        [SerializeField]
        private int id;

        [SerializeField]
        private bool aggressive = true;

        public int Id => id;
        public bool Aggressive => aggressive;

        public List<Vector2Int> Movements => _movements;

        public int Offset => _offset;
    }
}

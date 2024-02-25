using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gambetto.Scripts.GameCore.Room
{
    [CreateAssetMenu(fileName = "Behaviour", menuName = "ScriptableObjects/Behaviour")]
    public class Behaviour : ScriptableObject
    {
        [FormerlySerializedAs("_movements")]
        [SerializeField]
        private List<Vector2Int> movements = new();

        [FormerlySerializedAs("_offset")]
        [SerializeField]
        private int offset;

        [SerializeField]
        private int id;

        [SerializeField]
        private bool aggressive = true;

        [SerializeField]
        private int activationDistanceCells;

        [SerializeField]
        private int eventCountDown = 3;

        public List<Vector2Int> Movements => movements;

        public int Offset => offset;

        public int Id => id;

        public bool Aggressive => aggressive;

        public int ActivationDistanceCells => activationDistanceCells;

        public int EventCountDown => eventCountDown;
    }
}

using UnityEngine;
using UnityEditor;

namespace Gambetto.Scripts.Utils
{
    public static class Directions
    {

        /// <summary>
        /// Directions used for both Roomslayouts and Cells in GridManager
        /// </summary>
        public static readonly Vector3 North = new Vector3(1f, 0f, 0f);
        public static readonly Vector3 South = new Vector3(-1f, 0f, 0f);
        public static readonly Vector3 East = new Vector3(0f, 0f, -1f);
        public static readonly Vector3 West = new Vector3(0f, 0f, 1f);
        public static readonly Vector3 NorthWest= new Vector3(1f, 0f, 1f);
        public static readonly Vector3 NorthEast = new Vector3(1f, 0f, -1f);
        public static readonly Vector3 SouthEast = new Vector3(-1f, 0f, -1f);
        public static readonly Vector3 SouthWest = new Vector3(-1f, 0f, 1f);
    }
    
}
using UnityEngine;
using UnityEditor;

namespace Gambetto.Scripts.Utils
{
    public static class Directions
    {

        /// <summary>
        /// Directions used for both Roomslayouts and Cells in GridManager
        /// </summary>
        public static readonly Vector2 North = new Vector2(1f, 0f);
        public static readonly Vector2 South = new Vector2(-1f, 0f);
        public static readonly Vector2 East = new Vector2(0f, -1f);
        public static readonly Vector2 West = new Vector2(0f, 1f);
        public static readonly Vector2 NorthWest= new Vector2(1f, 1f);
        public static readonly Vector2 NorthEast = new Vector2(1f, -1f);
        public static readonly Vector2 SouthEast = new Vector2(-1f, -1f);
        public static readonly Vector2 SouthWest = new Vector2(-1f, 1f);
    }
    
}
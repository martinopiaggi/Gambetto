using UnityEngine;

namespace Gambetto.Scripts.Utils
{
    public static class Directions
    {

        /// <summary>
        /// Directions used for both Roomslayouts and Cells in GridManager
        /// </summary>
        public static readonly Vector2Int North = new Vector2Int(1, 0);
        public static readonly Vector2Int South = new Vector2Int(-1, 0);
        public static readonly Vector2Int East = new Vector2Int(0, -1);
        public static readonly Vector2Int West = new Vector2Int(0, 1);
        public static readonly Vector2Int NorthWest= new Vector2Int(1, 1);
        public static readonly Vector2Int NorthEast = new Vector2Int(1, -1);
        public static readonly Vector2Int SouthEast = new Vector2Int(-1, -1);
        public static readonly Vector2Int SouthWest = new Vector2Int(-1, 1);
    }
    
}
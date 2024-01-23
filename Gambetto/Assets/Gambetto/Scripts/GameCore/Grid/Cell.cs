using System.Collections.Generic;
using Gambetto.Scripts.Utils;
using UnityEngine;

namespace Gambetto.Scripts.GameCore.Grid
{
    public class Cell
    {
        private Vector3 globalCoordinates;
        private Vector2 globalCoordinates2D;

        private int _roomId;
        private bool _empty;

        private List<Cell> neighbors;

        private int room; //room id depends by the level, not by the roomLayout
        private Cell north;
        private Cell south;
        private Cell west;
        private Cell east;
        private Cell northWest;
        private Cell northEast;
        private Cell southWest;
        private Cell southEast;

        public Cell(Vector2 coordinates, int roomId)
        {
            _roomId = roomId;
            globalCoordinates = new Vector3(coordinates.x, 0f, coordinates.y);
            globalCoordinates2D = new Vector2(coordinates.x, coordinates.y);
        }

        public bool IsEmpty() => _empty;
        
        // by default is true 
        public void SetEmpty(bool isEmpty = true)
        {
            _empty = isEmpty;
        }

        public void SetNext(Vector2Int dir, Cell next)
        {
            //with a switch I have an error :( @todo/refactor
            if (dir == Directions.North)
                north = next;
            if (dir == Directions.South)
                south = next;
            if (dir == Directions.East)
                east = next;
            if (dir == Directions.West)
                west = next;
            if (dir == Directions.NorthWest)
                northWest = next;
            if (dir == Directions.NorthEast)
                northEast = next;
            if (dir == Directions.SouthEast)
                southEast = next;
            if (dir == Directions.SouthWest)
                southWest = next;
        }

        public Cell GetNext(Vector2Int dir)
        {
            //TODO extend to multiple next cells
            //with a switch I have an error :( @todo/refactor
            if (dir == Directions.North)
                return north;
            if (dir == Directions.South)
                return south;
            if (dir == Directions.East)
                return east;
            if (dir == Directions.West)
                return west;
            if (dir == Directions.NorthWest)
                return northWest;
            if (dir == Directions.NorthEast)
                return northEast;
            if (dir == Directions.SouthEast)
                return southEast;
            return southWest;
        }

        //roomId depends by the level, not by the roomLayout
        //same roomLayout can have different ID in different levels
        public int GetRoomNumber()
        {
            return _roomId;
        }

        public Vector3 GetGlobalCoordinates()
        {
            return globalCoordinates;
        }

        public Vector2 GetGlobalCoordinates2D()
        {
            return globalCoordinates2D;
        }
    }
}

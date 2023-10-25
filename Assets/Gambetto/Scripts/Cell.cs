using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gambetto.Scripts.Utils;
using Pieces;

public class Cell
{
    private Vector3 globalCoordinates;

    private int _roomId;
    
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
    }
    
    public void setNext(Vector2 dir,Cell next)
    {
        //with a switch I have an error :( @todo/refactor
        if (dir == Directions.North) north = next;
        if (dir == Directions.South) south = next;
        if (dir == Directions.East) east = next;
        if (dir == Directions.West) west = next;
        if (dir == Directions.NorthWest)  northWest = next;
        if (dir == Directions.NorthEast) northEast = next;
        if (dir == Directions.SouthEast ) southEast = next;
        if (dir == Directions.SouthWest) southWest = next;
    }
    
    public Cell getNext(Vector2 dir)
    {
        //with a switch I have an error :( @todo/refactor
        if (dir == Directions.North) return north;
        if (dir == Directions.South) return south;
        if (dir == Directions.East) return east;
        if (dir == Directions.West) return west;
        if (dir == Directions.NorthWest)  return northWest;
        if (dir == Directions.NorthEast) return northEast;
        if (dir == Directions.SouthEast ) return southEast;
        return southWest;
    }


    //roomId depends by the level, not by the roomLayout
    //same roomLayout can have different ID in different levels
    public int getRoomNumber()
    {
        return _roomId;
    }
    
    public Vector3 getGlobalCoordinates()
    {
        return globalCoordinates;
    }
    
}

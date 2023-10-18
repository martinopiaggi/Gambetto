using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomLayout", menuName = "ScriptableObjects/RoomLayout")]
public class RoomLayout : ScriptableObject
{
   [SerializeField] private List<Column> rows = new List<Column>();
   [SerializeField] private List<Position> initialPositions;
   
   public List<Column> GetRows()
   {
      return rows;
   }
   
   public List<Position> GetInitialPositions()
   {
      return initialPositions;
   }
   
   
   
   [Serializable]
   public class Column
   {
      [SerializeField] private List<int> array;
      
      public List<int> GetArray()
      {
         return array;
      }
   }
   
   [Serializable]
   public class Position
   {
      [SerializeField] private int x;
      [SerializeField] private int y;
      [SerializeField] private PieceType type;
   }
}






using System;
using System.Collections;
using System.Collections.Generic;
using Pieces;
using UnityEngine;
using UnityEditor;
using Gambetto.Scripts.Utils;

[CreateAssetMenu(fileName = "RoomLayout", menuName = "ScriptableObjects/RoomLayout")]
public class RoomLayout : ScriptableObject
{
   [SerializeField] private List<Column> rows = new List<Column>();
   [SerializeField] private List<Position> initialPositions;

   [SerializeField]  private Vector3 exitSide = Directions.North;
   
   
   public List<Column> GetRows()
   {
      return rows;
   }
   
   
   public Vector3 GetExit()
   {
      return exitSide;
   }
   
   public int GetSizeRow()
   {
      return rows.Count;
   }
   
   public int GetSizeColumn()
   {
      return rows[0].GetColumns().Count;
   }
   public List<Position> GetInitialPositions()
   {
      return initialPositions;
   }
   
   
   
   [Serializable]
   public class Column
   {
      [SerializeField] private List<int> columns;
      
      public List<int> GetColumns()
      {
         return columns;
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






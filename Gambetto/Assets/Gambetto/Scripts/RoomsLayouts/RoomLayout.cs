using System;
using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts.Pieces;
using Pieces;
using UnityEngine;
using UnityEditor;
using Gambetto.Scripts.Utils;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "RoomLayout", menuName = "ScriptableObjects/RoomLayout")]
public class RoomLayout : ScriptableObject
{
   
   [SerializeField]private string _topologyDescription;
   [SerializeField] private string _enemyDescription;
   [SerializeField] private List<Column> rows = new List<Column>();

   [SerializeField]  private Vector2Int exitSide = Directions.North;
   
   
   public List<Column> GetRows()
   {
      return rows;
   }
   
   
   public Vector2Int GetExit()
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






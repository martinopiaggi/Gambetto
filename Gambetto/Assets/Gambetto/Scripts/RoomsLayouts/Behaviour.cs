using System;
using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts.Pieces;
using Gambetto.Scripts.Utils;
using Pieces;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Behaviour", menuName = "ScriptableObjects/Behaviour")]
public class Behaviour : ScriptableObject
{

    [SerializeField] private List<Vector2Int> _movements;
    [SerializeField]
    private int _offset;
    [SerializeField]
    private int id;
    public int Id => id;

    public List<Vector2Int> Movements => _movements;

    public int Offset => _offset;
}

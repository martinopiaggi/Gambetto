using System;
using System.Collections;
using System.Collections.Generic;
using Pieces;
using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    private GameObject pawnPrefab;
    private GameObject _spawnGameObject;
    
    private void Awake()
    {
        pawnPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Rook");
    }

    private void Start()
    {
        SpawnPawn();
        var piece = _spawnGameObject.GetComponent<Piece>();
        piece.Move(new List<Vector3>
        {
            new(0, 0, 1),
            new(1, 0, 0),
            new(0, 0, 7),
            new(2, 0, 0),
            new(5, 0, 0),
        });
    }
    
    private void SpawnPawn()
    {
        var pos = new Vector3(2,1,1);
        _spawnGameObject = Instantiate(pawnPrefab, pos, Quaternion.identity);
        _spawnGameObject.transform.SetParent(transform);
    }

    private void Update()
    {
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Pieces;
using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    [SerializeField] private GameObject pawnPrefab;
    private GameObject spawnGameObject;

    private void Awake()
    {
        pawnPrefab = Resources.Load<GameObject>("Prefabs/Queen");
    }

    private void Start()
    {
        SpawnPawn();
        var piece = spawnGameObject.GetComponent<Piece>();
        // piece.MovePiece(new List<Vector3>
        // {
        //     new Vector3(0, 0, 1),
        //     new Vector3(1, 0, 0)
        // });
    }
    
    private void SpawnPawn()
    {
        spawnGameObject = Instantiate(pawnPrefab, transform.position, Quaternion.identity);
        spawnGameObject.transform.SetParent(transform);
    }

    private void Update()
    {
        
    }
}

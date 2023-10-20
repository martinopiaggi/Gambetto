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
        //load pawnPrefab from Resources folder
        pawnPrefab = Resources.Load<GameObject>("Prefabs/Pawn");
        return;
    }

    private void Start()
    {
        SpawnPawn();
    }
    
    private void SpawnPawn()
    {
        spawnGameObject = Instantiate(pawnPrefab, transform.position, Quaternion.identity);
        spawnGameObject.transform.SetParent(transform);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var pawn = spawnGameObject.GetComponent<Pawn>();
            pawn.MovePiece(new List<Vector3>
            {
                new Vector3(0, 0, 1),
                new Vector3(1, 0, 0)
            });
        }
    }
}

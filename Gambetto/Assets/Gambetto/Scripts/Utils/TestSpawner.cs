using System;
using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts;
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
        
        GameClock.ClockTick += OnClockTicked;
        GameClock.StartClock();
        
        SpawnPawn();
        var piece = _spawnGameObject.GetComponent<Piece>();
        piece.Countdown = 1;
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

    public void OnClockTicked(object source, ClockEventArgs args)
    {
        Debug.Log("Clock ticked " + args.CurrentTick);
    }
}

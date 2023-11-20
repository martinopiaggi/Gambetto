using System;
using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts;
using Gambetto.Scripts.Pieces;
using Pieces;
using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    private GameObject _pawnPrefab;
    private GameObject _spawnGameObject;

    private Piece _piece;

    private List<Vector3> _moves = new List<Vector3>
    {
        new(1, 0, 0),
        new(0, 0, 0),
        new(1, 0, 0),
        new(0, 0, 0),
        new(1, 0, 0),
    };
    
    private void Awake()
    {
        _pawnPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Rook");
    }

    private void Start()
    {
        GameClock.Instance.ClockTick += OnClockTicked;
        GameClock.Instance.StartClock();
        
        SpawnPawn();
        _piece = _spawnGameObject.GetComponent<Piece>();
        _piece.Countdown = 1;
    }

    private void OnDestroy()
    {
        
    }

    private void SpawnPawn()
    {
        var pos = new Vector3(2,1,1);
        _spawnGameObject = Instantiate(_pawnPrefab, pos, Quaternion.identity);
        _spawnGameObject.transform.SetParent(transform);
    }

    public void OnClockTicked(object source, ClockEventArgs args)
    {
        Debug.Log("Clock ticked " + args.CurrentTick);
    }
}

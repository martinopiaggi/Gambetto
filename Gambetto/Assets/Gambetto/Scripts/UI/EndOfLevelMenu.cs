using System;
using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts;
using Gambetto.Scripts.Utils;
using UnityEngine;

public class EndOfLevelMenu : MonoBehaviour
{
    GridManager gridManager;
    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
    }

    public void Retry()
    {
        TimeManager.ResumeTime();
        gridManager.RestartLevel();
    }
}

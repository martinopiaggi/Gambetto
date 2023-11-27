using System.Collections;
using System.Collections.Generic;
using Gambetto.Scripts;
using UnityEngine;

public class Camera_following : MonoBehaviour
{
    public GridManager GridManager; 
    private Vector3 OldPos, NewPos;
    private Vector3 OffSet, TotalOffset;
    private bool firstTime = true;
    // Start is called before the first frame update
    void Start()
    {
        OffSet = new Vector3();
        TotalOffset = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        if (firstTime)
        {
            OldPos = GridManager.getPlayerPosition();
            firstTime = false;
        }
        NewPos = GridManager.getPlayerPosition();
        OffSet.x = NewPos.x - OldPos.x;
        OffSet.z = NewPos.z - OldPos.z;
        OffSet.y = 0;
        TotalOffset = TotalOffset + OffSet;
        transform.position = transform.position + new Vector3(0.1f* TotalOffset.x, 0, 0.1f* TotalOffset.z);
        TotalOffset = new Vector3( TotalOffset.x*0.9f, 0, TotalOffset.z * 0.9f);
        OldPos = NewPos;
    }
    
}

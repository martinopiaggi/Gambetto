using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Room : MonoBehaviour
{
    [SerializeField] private RoomLayout _layout;

    [SerializeField] private Material light;
    [SerializeField] private Material dark;

    private int gridLength;
    private int gridWidth;

    private int[,] matrix; // The matrix to store the cubes

    [SerializeField] private GameObject cubePrefab; // Reference to the Cube prefab
    
    
    public void InitializeRoom(RoomLayout layout)
    {
        _layout = layout;
        gridLength = layout.GetRows().Count;
        gridWidth = layout.GetRows()[0].GetArray().Count;
        matrix = new int[gridLength, gridWidth];
        FillMatrixWithCubes();
    }


    void FillMatrixWithCubes()
    {
        for (int i = 0; i < gridLength; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                Vector3 position = new Vector3(i, -5, j);
                matrix[i, j] = 0;
                if (_layout.GetRows()[i].GetArray()[j] == -1)
                {
                    continue;
                }

                GameObject cubeInstance = Instantiate(cubePrefab, position, Quaternion.identity);
                cubeInstance.GetComponent<MeshRenderer>().material = (i + j) % 2 == 0 ? light : dark;
                cubeInstance.transform.parent = gameObject.transform;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Room : MonoBehaviour
{

    [SerializeField]
    private int gridLength = 8;
    [SerializeField]
    private int gridWidth = 8;
    [SerializeField]
    private GameObject cubePrefab; // Reference to the Cube prefab

    [SerializeField] private Material light;
    [SerializeField] private Material dark;
    
    private int[,] matrix; // The matrix to store the cubes
    
    void Start()
    {
        CreateMatrix();
        FillMatrixWithCubes();

    }
    
    void CreateMatrix()
    {
        matrix = new int[gridLength, gridWidth];   
    }
     
    void FillMatrixWithCubes()
    {
        for (int i = 0; i < gridLength; i++)
        {
            for (int j = 0; j < gridWidth ;j++)
            { 
                Vector3 position= new Vector3(i,0,j);
                matrix[i,j]=0;
                
                GameObject cubeInstance=Instantiate(cubePrefab,position,Quaternion.identity);
                cubeInstance.GetComponent<MeshRenderer>().material = (i+j)%2==0 ? light : dark;
            } 
        }  
    }

    
}

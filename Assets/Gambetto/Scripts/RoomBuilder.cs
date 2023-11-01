using UnityEngine;

namespace Gambetto.Scripts
{
    public class RoomBuilder : MonoBehaviour
    {
        [SerializeField] private RoomLayout _layout;

        [SerializeField] private Material light;
        [SerializeField] private Material dark;
    
    
        private int colorStart = 0;
        private int gridLength;
        private int gridWidth;
    

        private int[,] matrix; // The matrix to store the cubes

        [SerializeField] private GameObject cubePrefab; // Reference to the Cube prefab
    
    
        public void setColorStart(int value)
        {
            colorStart = value;
        }
    
        public void InitializeRoom(RoomLayout layout)
        {
            _layout = layout;
            gridLength = layout.GetSizeRow();
            gridWidth = layout.GetSizeColumn();
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
                    if (_layout.GetRows()[i].GetColumns()[j] == -1)
                    {
                        continue;
                    }

                    GameObject cubeInstance = Instantiate(cubePrefab, position, Quaternion.identity);
                    cubeInstance.GetComponent<MeshRenderer>().material = (i + j + colorStart) % 2 == 0 ? light : dark;
                    cubeInstance.transform.parent = gameObject.transform;
                }
            }
        }
    }
}
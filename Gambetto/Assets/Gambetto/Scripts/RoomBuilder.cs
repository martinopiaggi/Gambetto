using System;
using UnityEngine;

namespace Gambetto.Scripts
{
    public class RoomBuilder : MonoBehaviour
    {
        [SerializeField]
        private RoomLayout _layout;

        [SerializeField]
        private Material light;

        [SerializeField]
        private Material dark;

        [SerializeField]
        private bool isBuilt = false;

        private int colorStart = 0;

        [SerializeField]
        private GameObject cubePrefab; // Reference to the Cube prefab

        private void Update()
        {
            //todo: does this need to be here?
            if (isBuilt)
                return;
            InitializeRoom(_layout);
        }

        public void setColorStart(int value)
        {
            colorStart = value;
        }

        public void InitializeRoom(RoomLayout layout)
        {
            _layout = layout;
            FillMatrixWithCubes();
            isBuilt = true;
        }

        void FillMatrixWithCubes()
        {
            for (int i = 0; i < _layout.GetSizeRow(); i++)
            {
                for (int j = 0; j < _layout.GetSizeColumn(); j++)
                {
                    Vector3 position = new Vector3(i, cubePrefab.transform.position.y, j);
                    if (_layout.Squares[i, j].Value == RoomLayout.MatrixValue.Empty)
                    {
                        continue;
                    }
                    var cubeInstance = Instantiate(cubePrefab, gameObject.transform, true);
                    cubeInstance.transform.localPosition = position;
                    cubeInstance.transform.rotation = Quaternion.identity;
                    cubeInstance.GetComponent<MeshRenderer>().material =
                        (i + j + colorStart) % 2 == 0 ? light : dark;
                }
            }
        }
    }
}

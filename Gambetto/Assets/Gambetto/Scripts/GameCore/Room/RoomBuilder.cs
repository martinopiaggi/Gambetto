using UnityEngine;
using UnityEngine.Serialization;

namespace Gambetto.Scripts.GameCore.Room
{
    public class RoomBuilder : MonoBehaviour
    {
        [FormerlySerializedAs("_layout")]
        [SerializeField]
        private RoomLayout layout;

        [SerializeField]
        private new Material light;

        [SerializeField]
        private Material dark;

        [SerializeField]
        private bool isBuilt;

        private int colorStart;

        [SerializeField]
        private GameObject cubePrefab; // Reference to the Cube prefab

        private void Update()
        {
            //todo: does this need to be here?
            if (isBuilt)
                return;
            InitializeRoom(layout);
        }

        public void SetColorStart(int value)
        {
            colorStart = value;
        }

        public void InitializeRoom(RoomLayout l)
        {
            this.layout = l;
            FillMatrixWithCubes();
            isBuilt = true;
        }

        void FillMatrixWithCubes()
        {
            for (int i = 0; i < layout.GetSizeRow(); i++)
            {
                for (int j = 0; j < layout.GetSizeColumn(); j++)
                {
                    if (
                        (layout.Squares[i, j].Value == RoomLayout.MatrixValue.Empty)
                        || layout.Squares[i, j].Value == RoomLayout.MatrixValue.Exit
                    )
                    {
                        continue;
                    }
                    var position = new Vector3(i, cubePrefab.transform.position.y, j);
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

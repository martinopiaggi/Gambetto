using Gambetto.Scripts.GameCore.Grid;
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

        private int colorStart;

        [SerializeField]
        private GameObject cubePrefab; // Reference to the Cube prefab

        public void SetColorStart(int value)
        {
            colorStart = value;
        }

        public void InitializeRoom(RoomLayout l)
        {
            this.layout = l;
            FillMatrixWithCubes();
        }

        void FillMatrixWithCubes()
        {
            var endOfLevelEffect = CubesRuntimeManager.instance;
            for (int i = 0; i < layout.GetSizeRow(); i++)
            {
                for (int j = 0; j < layout.GetSizeColumn(); j++)
                {
                    if ((layout.Squares[i, j].Value == RoomLayout.MatrixValue.Empty)
                    // || layout.Squares[i, j].Value == RoomLayout.MatrixValue.Exit
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

                    // Add the cube to the list of cubes for the end of level effect only if it's not the exit
                    if (layout.Squares[i, j].Value == RoomLayout.MatrixValue.Exit)
                    {
                        continue;
                    }
                    endOfLevelEffect.AddCube(cubeInstance);
                }
            }
        }

        public void SetScheme(ColorScheme colorScheme)
        {
            light = colorScheme.lightMaterial;
            dark = colorScheme.darkMaterial;
        }
    }
}

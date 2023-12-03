using UnityEngine;

namespace Gambetto.Scripts
{
    public class CameraFollowing : MonoBehaviour
    {
        public GridManager gridManager;
        private Vector3 oldPos,
            newPos;
        private Vector3 offSet,
            totalOffset;

        private bool firstTime = true;

        // Start is called before the first frame update
        void Start()
        {
            offSet = new Vector3();
            totalOffset = new Vector3();
        }

        // Update is called once per frame
        void Update()
        {
            if (firstTime)
            {
                oldPos = gridManager.GetPlayerPosition();
                firstTime = false;
            }

            newPos = gridManager.GetPlayerPosition();
            offSet.x = newPos.x - oldPos.x;
            offSet.z = newPos.z - oldPos.z;
            offSet.y = 0;
            totalOffset = totalOffset + offSet;
            transform.position += new Vector3(0.1f * totalOffset.x, 0, 0.1f * totalOffset.z);
            totalOffset = new Vector3(totalOffset.x * 0.9f, 0, totalOffset.z * 0.9f);
            oldPos = newPos;
        }
    }
}

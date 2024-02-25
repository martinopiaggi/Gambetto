using System.Collections.Generic;
using Gambetto.Scripts.GameCore.Grid;
using UnityEngine;

namespace Gambetto.Scripts
{
    public class CameraFollowing : MonoBehaviour
    {
        public GridManager gridManager;

        /**
         * Modality of the camera, 0 simple, 1 midpoint
         */
        public bool modality;

        /**
         * parameter that regulate the smoothing in the camera movement
         */
        [Range(0, 1)]
        public float smoothFactor = 0.9f;

        /**
         * Distance of the camera from the grid (we were using 46)
         */
        public float distance;

        private const float Sin45 = 0.70771f;

        //variable to support the smoothing movement of the camera
        private Vector3 _oldPos,
            _newPos;
        private Vector3 _offSet,
            _totalOffset;

        private bool _firstTime = true;

        // variables to support the new type of camera

        /**
         * List of the centers of the rooms
         */
        private List<Vector3> _roomsCenter;

        /**
         * Variable containing current room index
         */
        private int _currentRoom;

        /**
         * Cell of the player
         */
        private Cell _playerPosition;

        /**
         * Center of the current room
         */
        private Vector3 _roomCenter;

        /**
         * Point between the center of the room and the player
         */
        private Vector3 _midPoint;

        /**
         * Variable that contains the vector of current player position
         */
        private Vector3 _playerPositionCoordinates;

        /**
         * Variable containing the desired camera position
         */
        private Vector3 _cameraPosition;

        // Start is called before the first frame update
        void Start()
        {
            _offSet = new Vector3();
            _totalOffset = new Vector3();
        }

        void Update()
        {
            if (modality)
            {
                MidPoint();
            }
            else
            {
                SimpleCamera();
            }
        }
        
        /**
         * Function that implement simple camera following
         */
        void SimpleCamera()
        {
            if (_firstTime)
            {
                // instructions that compute the camera position according to the distance
                _roomsCenter = gridManager.GetRoomsCenter();
                _roomCenter = _roomsCenter[0];
                float disTemp = distance * Sin45;
                disTemp = disTemp * Sin45;
                _cameraPosition.x = _roomCenter.x - disTemp;
                _cameraPosition.z = _roomCenter.z - disTemp;
                disTemp = distance * Sin45;
                _cameraPosition.y = disTemp;

                _oldPos = gridManager.GetPlayerPosition().GetGlobalCoordinates();
                _firstTime = false;
                transform.position = _cameraPosition;
            }

            _newPos = gridManager.GetPlayerPosition().GetGlobalCoordinates();
            _offSet.x = _newPos.x - _oldPos.x;
            _offSet.z = _newPos.z - _oldPos.z;
            _offSet.y = 0;
            _totalOffset = _totalOffset + _offSet;
            transform.position += new Vector3((1 - smoothFactor) * _totalOffset.x, 0, (1 - smoothFactor) * _totalOffset.z);
            _totalOffset = new Vector3(_totalOffset.x * smoothFactor, 0, _totalOffset.z * smoothFactor);
            
            // if the amount of offset become too small I just set it to 0 to avoid trembling of the camera
            //if (_totalOffset.x < 0.00001 && _totalOffset.x > -0.00001)  _totalOffset.x = 0;
            //if (_totalOffset.z < 0.00001 && _totalOffset.x > -0.00001)  _totalOffset.z = 0;
            
            _oldPos = _newPos;
        }

        void MidPoint()
        {
            _playerPosition = gridManager.GetPlayerPosition();
            _roomsCenter = gridManager.GetRoomsCenter();
            _currentRoom = _playerPosition.GetRoomNumber();

            _playerPositionCoordinates = _playerPosition.GetGlobalCoordinates();
            _playerPositionCoordinates.y = 0.0f;
            _roomCenter = _roomsCenter[_currentRoom];
            _roomCenter.y = 0.0f;

            _midPoint = _playerPositionCoordinates + _roomCenter;
            _midPoint.x /= 2.0f;
            _midPoint.y = 0.0f;
            _midPoint.z /= 2.0f;

            float disTemp = distance * Sin45;
            disTemp = disTemp * Sin45;
            _cameraPosition.x = _midPoint.x - disTemp;
            _cameraPosition.z = _midPoint.z - disTemp;
            disTemp = distance * Sin45;
            _cameraPosition.y = disTemp;

            if (_firstTime)
            {
                transform.position = _cameraPosition;
                _oldPos = _cameraPosition;
                _firstTime = false;
            }

            //Part of the code to implement a smooth camera movement
            _newPos = _cameraPosition;
            _offSet.x = _newPos.x - _oldPos.x;
            _offSet.z = _newPos.z - _oldPos.z;
            _offSet.y = 0;
            _totalOffset = _totalOffset + _offSet;
            transform.position += new Vector3(
                (1 - smoothFactor) * _totalOffset.x,
                0,
                (1 - smoothFactor) * _totalOffset.z
            );
            _totalOffset = new Vector3(_totalOffset.x * smoothFactor, 0, _totalOffset.z * smoothFactor);
            
            // if the amount of offset become too small I just set it to 0 to avoid trembling of the camera
            //if (_totalOffset.x < 0.00001 && _totalOffset.x > -0.00001)  _totalOffset.x = 0;
            //if (_totalOffset.z < 0.00001 && _totalOffset.x > -0.00001)  _totalOffset.z = 0;

            _oldPos = _newPos;
        }
    }
}

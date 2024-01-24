using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

// static class to hold the cubes that are in the end of level effect
namespace Gambetto.Scripts.GameCore.Grid
{
    public class CubesRuntimeManager : MonoBehaviour
    {
        [SerializeField]
        private Camera _cam;
        private bool _fired = false;

        //list
        [SerializeField]
        private List<GameObject> _cubes = new List<GameObject>();
        private readonly List<GameObject> _powerUps = new List<GameObject>();
        private Vector3 _exitCoords;

        // singleton
        public static CubesRuntimeManager instance;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(instance);
            }
            instance = this;
        }

        
        
        public void FireEffect(GameObject target)
        {
            _fired = true;
            StartCoroutine(EndOfLevelEffectCoroutine());
            _camOriginalSize = _cam.orthographicSize;
            StartCoroutine(CinematicZoomCoroutine(target, 1f));
        }

        private float _camOriginalSize;
        private Vector3 _camPosition;

        private IEnumerator CinematicZoomCoroutine(GameObject target, float duration)
        {
            var camOriginalSize = _cam.orthographicSize;

            var targetPosition = target.transform.position;
            var camFinalSize = 4.5f;

            float elapsedTime = 0;

            // Calculate the new position for the camera
            var finalCamPos = targetPosition - _cam.transform.forward * 10f;
            var moveTime = duration * 2f;
            target.GetComponent<Rigidbody>().isKinematic = true;
            while (elapsedTime < moveTime)
            {
                elapsedTime += Time.deltaTime;
                _cam.transform.position = Vector3.MoveTowards(
                    _cam.transform.position,
                    finalCamPos,
                    elapsedTime / moveTime
                );
                _cam.orthographicSize = Mathf.Lerp(
                    camOriginalSize,
                    camFinalSize,
                    elapsedTime / moveTime
                );

                target.transform.position = Vector3.MoveTowards(
                    targetPosition,
                    targetPosition + Vector3.up,
                    elapsedTime / moveTime
                );

                yield return null;
            }
        }
        
        private List<GameObject> doors = new List<GameObject>();
        
        public void AddDoorCoords(Vector3 coords)
        {
            foreach (var cube in _cubes.Where(cube => (cube.transform.position.x == coords.x && cube.transform.position.z == coords.z)))
            {
                Debug.Log("found door");
                doors.Add(cube);
                StartCoroutine(MoveDoorCoroutine(cube, false,true));
            }
        }
        
        //method used to open and closed all doors 
        public void ToggleAllDoors(bool isOpen,bool skipAnimation = false)
        {
            foreach (var door in doors)
            {
                StartCoroutine(MoveDoorCoroutine(door, isOpen,skipAnimation));
            }
        }
        

        private IEnumerator MoveDoorCoroutine(GameObject door, bool isOpen, bool skipAnimation = false)
        {
            float doorMoveDistance = 7f; // Total distance to move the door
            float doorMoveSpeed = 10f + 4f *  Random.value; // Speed at which the door moves
            float elapsedTime = 0f; // Time elapsed since the start of the animation
            Vector3 direction = isOpen ? Vector3.up : Vector3.down; // Direction of the door's movement
            float totalMovement = 0f; // Total movement accumulated

            // Calculate the target position based on the opening state
            Vector3 originalPosition = door.transform.position;
            Vector3 targetPosition = originalPosition + direction * doorMoveDistance;

            while (totalMovement < doorMoveDistance && !skipAnimation)
            {
                // Calculate movement for this frame and update the total movement
                float frameMovement = doorMoveSpeed * Time.deltaTime;
                totalMovement += frameMovement;
                door.transform.position += direction * frameMovement;
                
                if (totalMovement > doorMoveDistance)
                {
                    door.transform.position = targetPosition;
                    break;
                }
                
                elapsedTime += Time.deltaTime;
                yield return new WaitForSeconds(0.01f);
            }
            Debug.Log(" door moved");
            door.transform.position = targetPosition; //force position
        }
        
        private IEnumerator<WaitForSeconds> EndOfLevelEffectCoroutine()
        {
            // hide all powerups
            _powerUps.ForEach((p) => p.SetActive(false));
            // Assign a delay to each cube based on its order
            float baseDelay = 0.05f; // Base delay between each cube's movement
            Dictionary<GameObject, float> cubeDelays = new Dictionary<GameObject, float>();
            float minTime = float.MinValue;
            for (int i = 0; i < _cubes.Count; i++)
            {
                var delay = Dist(_cubes[i]) * baseDelay;
                if (minTime > delay)
                    minTime = delay;
                cubeDelays[_cubes[i]] = delay;
            }

            // Start moving all cubes, considering their delay

            float animTime = 0;
            while (animTime < 5f) //maximum time
            {
                foreach (var cube in _cubes)
                {
                    // Check if the cube's delay time has passed
                    if (animTime >= cubeDelays[cube])
                    {
                        // Move each cube if it hasn't reached the target position yet
                        if (cube.transform.position.y > -10)
                        {
                            cube.transform.position += Vector3.down * Time.deltaTime * 15;
                        }
                    }
                }

                // Update elapsed time and wait a bit before the next update
                animTime += Time.deltaTime;
                yield return new WaitForSeconds(0.01f);
            }
        }

        public void ResetEndOfLevelEffect()
        {
            if (_fired)
            {
                ResetCubes();
                _powerUps.ForEach((p) => p.SetActive(true));
                ResetCamera();
                _fired = false;
            }
        }

        private void ResetCubes()
        {
            StopAllCoroutines(); //stopping the running coroutine in case
            foreach (var cube in _cubes)
            {
                var p = cube.transform.position;
                cube.transform.position = new Vector3(p.x, -0.05f, p.z);
            }
        }

        private void ResetCamera()
        {
            _cam.orthographicSize = _camOriginalSize;
        }

        //compute distance
        private float Dist(GameObject cube)
        {
            return Vector3.Distance(cube.transform.position, _exitCoords);
        }

        public void AddCube(GameObject cube)
        {
            _cubes.Add(cube);
        }

        public void AddTile(GameObject powerUp)
        {
            _powerUps.Add(powerUp);
        }

        public void AddExitCoords(Vector3 coords)
        {
            _exitCoords = coords;
        }
    }
}

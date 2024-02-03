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
            foreach (
                var cube in _cubes.Where(
                    cube =>
                        (
                            cube.transform.position.x == coords.x
                            && cube.transform.position.z == coords.z
                        )
                )
            )
            {
                doors.Add(cube);
                StartCoroutine(ToggleCubeHeight(cube, false, true));
            }
        }

        //method used to open and closed all doors
        public void ToggleAllDoors(bool moveUp, bool skipAnimation = false)
        {
            foreach (var door in doors)
            {
                StartCoroutine(ToggleCubeHeight(door, moveUp, skipAnimation));
            }
        }

        public void DetonateNeighborhood(List<Cell> neighborhood, bool skipAnimation = false)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Instance.bombExplosion);
            var crater = RetrieveNeighborhood(neighborhood);
            _detonatedCubes.AddRange(crater); //adding to the list of detonated cubes
            foreach (var ripCell in crater)
            {
                StartCoroutine(ToggleCubeHeight(ripCell, false, skipAnimation));
            }
        }

        public void PulsingNeighborhood(List<Cell> neighborhood)
        {
            var crater = RetrieveNeighborhood(neighborhood);
            foreach (var ripCell in crater)
            {
                //adding emessive to the color
                ripCell.GetComponentInChildren<MeshRenderer>().material.EnableKeyword("_EMISSION");
                ripCell
                    .GetComponentInChildren<MeshRenderer>()
                    .material
                    .SetColor("_EmissionColor", Color.red*5f);
            }
            //launch coroutine to cooldown the color
            StartCoroutine(CooldownNeighborhood(crater));
        }

        private IEnumerator CooldownNeighborhood(List<GameObject> neighborhood)
        {
            // Cache materials and initial emission colors
            var materials = new List<(Material, Color)>();
            foreach (var ripCell in neighborhood)
            {
                var renderer = ripCell.GetComponentInChildren<MeshRenderer>();
                var mat = renderer.material; // Consider using 'sharedMaterial' if materials are shared and changes are non-persistent
                var emissionColor = mat.GetColor("_EmissionColor");
                materials.Add((mat, emissionColor));
            }

            // Define cooldown parameters
            var cooldownDuration = 1f; // Total duration in seconds to complete the cooldown
            var elapsedTime = 0f;

            // Cooldown loop
            while (elapsedTime < cooldownDuration)
            {
                foreach (var (material, initialColor) in materials)
                {
                    // Calculate the new emission color using Lerp for a smooth transition
                    Color newEmissionColor = Color.Lerp(
                        initialColor,
                        Color.black,
                        elapsedTime / cooldownDuration
                    );
                    material.SetColor("_EmissionColor", newEmissionColor);
                }

                // Increment the elapsed time and wait
                elapsedTime += 0.01f; // Increment based on your wait time below
                yield return new WaitForSeconds(0.01f);
            }
        }

        private List<GameObject> RetrieveNeighborhood(List<Cell> neighborhood)
        {
            List<GameObject> neighborhoodObjects = new List<GameObject>();
            foreach (var cell in neighborhood)
            {
                var xCell = cell.GetGlobalCoordinates().x;
                var zCell = cell.GetGlobalCoordinates().z;

                foreach (var cube in _cubes)
                {
                    var cubePos = cube.transform.position;
                    if (cubePos.x != xCell || cubePos.z != zCell)
                        continue;

                    neighborhoodObjects.Add(cube);
                }
            }
            return neighborhoodObjects;
        }

        private List<GameObject> _detonatedCubes = new List<GameObject>();

        public void ResetDetonatedCubes()
        {
            foreach (var detonatedCube in _detonatedCubes)
            {
                StartCoroutine(ToggleCubeHeight(detonatedCube, true, true));
            }
            _detonatedCubes = new List<GameObject>();
        }

        private IEnumerator ToggleCubeHeight(
            GameObject cube,
            bool moveUp,
            bool skipAnimation = false
        )
        {
            const float cubeMoveDistance = 7f; // Total distance to move the door
            var cubeMoveSpeed = 10f + 4f * Random.value; // Speed at which the door moves
            var direction = moveUp ? Vector3.up : Vector3.down; // Direction of the door's movement
            var totalMovement = 0f; // Total movement accumulated

            // Calculate the target position based on the opening state
            Vector3 originalPosition = cube.transform.position;
            Vector3 targetPosition = originalPosition + direction * cubeMoveDistance;

            //Weak try to fix BUG purposes
            if (targetPosition.y > 1f)
            {
                Debug.Log("target position is too high");
                Debug.Log("BUG AVOIDED, recheck the code");
                targetPosition.y = -0.05f;
            }

            while (totalMovement < cubeMoveDistance && !skipAnimation)
            {
                // Calculate movement for this frame and update the total movement
                float frameMovement = cubeMoveSpeed * Time.deltaTime;
                totalMovement += frameMovement;
                cube.transform.position += direction * frameMovement;

                if (totalMovement > cubeMoveDistance)
                {
                    cube.transform.position = targetPosition;
                    break;
                }

                yield return new WaitForSeconds(0.01f);
            }
            cube.transform.position = targetPosition; //force position
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

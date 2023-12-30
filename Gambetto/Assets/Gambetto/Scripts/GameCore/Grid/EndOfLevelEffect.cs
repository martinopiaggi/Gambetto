using System;
using System.Collections.Generic;
using UnityEngine;

// static class to hold the cubes that are in the end of level effect
namespace Gambetto.Scripts.GameCore.Grid
{
    public class EndOfLevelEffect : MonoBehaviour
    {

        //list 
        private List<GameObject> _cubes = new List<GameObject>();
        private Vector3 _exitCoords;
        
        
        public bool ok = false;
        
        // singleton
        public static EndOfLevelEffect instance;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(instance);
            }        
            instance = this;
        }

        //update is called every frame
        private void Update()
        {
            if (ok)
            {
                ok = false;
                //coroutine
                StartCoroutine(EndOfLevelEffectCoroutine());
            }
        }
    
        private IEnumerator<WaitForSeconds> EndOfLevelEffectCoroutine()
        {            
            
            // Assign a delay to each cube based on its order
            float baseDelay = 0.05f; // Base delay between each cube's movement
            Dictionary<GameObject, float> cubeDelays = new Dictionary<GameObject, float>();
            float minTime = float.MinValue;
            for (int i = 0; i < _cubes.Count; i++)
            {
                var delay = dist(_cubes[i]) * baseDelay;
                if (minTime > delay) minTime = delay;
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
                Debug.Log(animTime);
            }
        }


        //compute distance
        private float dist(GameObject cube)
        {
            return Vector3.Distance(cube.transform.position, _exitCoords);
        }
        
        public void AddCube(GameObject cube)
        {
            _cubes.Add(cube);
        }
        
        public void AddExitCoords(Vector3 coords)
        {
          _exitCoords = coords;  
        }
    }
}

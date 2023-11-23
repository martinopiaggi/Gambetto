using System;
using System.Collections;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;
using Utils;

namespace Gambetto.Scripts
{
    public class ClockEventArgs : EventArgs
    {
        public int CurrentTick { get; set; }
    }
    public class GameClock : MonoBehaviour
    {
        public static GameClock Instance { get; private set; }


        private const float DefaultClockPeriod = 2.3f;
        private bool _isRunning;
        private float _clockPeriod = DefaultClockPeriod; // clock period in seconds
        private Thread _clockThread;

        // Defines event delegate (signature of the method that will be called by the event)
        public delegate void ClockTickEventHandler(object sender, ClockEventArgs args);

        public event ClockTickEventHandler ClockTick;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public float ClockPeriod => _clockPeriod;

        /// <summary>
        /// If the clock thread isn't running, it starts it.
        /// </summary>
        /// <param name="clockPeriod"> Clock period in seconds. Default is<see cref="DefaultClockPeriod"/>.</param>
        public void StartClock(float clockPeriod = DefaultClockPeriod)
        {
            if (_isRunning)
            {
                Debug.LogWarning("Trying to start clock while it's already running.");
                return;
            }

            _clockPeriod = clockPeriod;
            _isRunning = true;
            StartCoroutine(ClockRoutine());
        }
        
        private IEnumerator ClockRoutine()
        {
            var i = 0;
            while (_isRunning)
            {
                OnClockTick(new ClockEventArgs() { CurrentTick = i });
                i++;
                var text = "currentTick = " + i;
                if (Debugger.Instance != null)
                    Debugger.Instance.Show(text, Color.white, Debugger.Position.UpperRight,printConsole: false);
                yield return new WaitForSeconds(_clockPeriod);
            }
        }
        
        
        /// <summary>
        /// Stops the clock thread.
        /// </summary>
        public void StopClock()
        {
            _isRunning = false;
        }

        /// <summary>
        /// Changes the clock period.
        /// </summary>
        /// <param name="clockPeriod"> Clock period in seconds.</param>
        public void ChangeClockPeriod(float clockPeriod)
        {
            _clockPeriod = clockPeriod;
        }

        

        private void OnClockTick(ClockEventArgs e)
        {
            ClockTick?.Invoke(null, e);
        }
    }
    
}
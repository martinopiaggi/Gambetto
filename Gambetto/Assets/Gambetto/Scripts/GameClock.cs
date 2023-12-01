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

        private const float DefaultClockPeriod = 5f;
        private bool _isRunning;
        private float _clockPeriod = DefaultClockPeriod; // clock period in seconds
        private Thread _clockThread;
        private int _currentTick;
        private Coroutine _clockCoroutine;

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
                Debug.LogWarning("clock already running");
                return;
            }

            try
            {
                //StopCoroutine(_clockCoroutine);
                StopAllCoroutines();
            }
            catch (Exception e)
            {
                Debug.Log(("No couroutine to stop"));
            }
            _currentTick = 0;
            _clockPeriod = clockPeriod;
            _isRunning = true;
            _clockCoroutine = StartCoroutine(ClockRoutine());
        }

        public void ForceClockTick()
        {
            Debug.Log("game clock forced to tick");
            StopAllCoroutines();
            _clockCoroutine = StartCoroutine(ClockRoutine());
        }

        /// <summary>
        /// Stops the clock thread.
        /// </summary>
        public void StopClock()
        {
            _currentTick = 0;
            _isRunning = false;
            StopAllCoroutines();
        }

        /// <summary>
        /// Pauses the clock thread.
        /// </summary>
        public void PauseClock()
        {
            _isRunning = false;
            StopAllCoroutines();
        }

        /// <summary>
        /// Resumes the clock thread.
        /// </summary>
        public void ResumeClock()
        {
            //TODO
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

        private IEnumerator ClockRoutine()
        {
            while (_isRunning)
            {
                MakeClockTick();
                yield return new WaitForSeconds(_clockPeriod);
            }
        }

        private void MakeClockTick()
        {
            OnClockTick(new ClockEventArgs() { CurrentTick = _currentTick });
            _currentTick++;
            if (Debugger.Instance != null)
                Debugger
                    .Instance
                    .Show(
                        "currentTick = " + _currentTick,
                        Color.white,
                        Debugger.Position.UpperRight,
                        printConsole: true
                    );
        }
    }
}

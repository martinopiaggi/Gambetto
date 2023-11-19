using System;
using System.Threading;
using UnityEngine;

namespace Gambetto.Scripts
{
    public static class GameClock
    {
        // Define event delegate (signature of the method that will be called by the event)
        public delegate void ClockTickEventHandler(object sender, EventArgs args);

        public static event ClockTickEventHandler ClockTick;

        private static bool _isRunning;
        private static int _clockPeriod = 1000; // clock period in milliseconds
        private static Thread _clockThread;

        private static void ClockRoutine()
        {
            while (_isRunning)
            {
                Thread.Sleep(_clockPeriod);
                OnClockTick(EventArgs.Empty);
            }
        }

        public static void StartClock()
        {
            if (_isRunning)
            {
                Debug.LogWarning("Trying to start clock while it is already running.");
                return;
            }

            _isRunning = true;
            _clockThread = new Thread(ClockRoutine);
            _clockThread.Start();
        }

        public static void StopClock()
        {
            _isRunning = false;
        }

        private static void OnClockTick(EventArgs e)
        {
            ClockTick?.Invoke(null, e);
        }
    }
}
using System;
using System.Threading;
using UnityEngine;

namespace Gambetto.Scripts
{
    
    public class ClockEventArgs : EventArgs
    {
        public int CurrentTick { get; set; }
    }
    
    /// <summary>
    /// Static class that implements a game clock.
    /// </summary>
    public static class GameClock
    {
        // Define event delegate (signature of the method that will be called by the event)
        public delegate void ClockTickEventHandler(object sender, ClockEventArgs args);

        public static event ClockTickEventHandler ClockTick;

        private static bool _isRunning;
        private static int _clockPeriod = 1000; // clock period in milliseconds
        private static Thread _clockThread;

        private static void ClockRoutine()
        {
            var i = 0;
            while (_isRunning)
            {
                Thread.Sleep(_clockPeriod);
                OnClockTick(new ClockEventArgs() {CurrentTick = i});
                i++;
            }
        }

        public static void StartClock()
        {
            if (_isRunning)
            {
                Debug.LogWarning("Trying to start clock while it's already running.");
                return;
            }

            _isRunning = true;
            // Could
            _clockThread = new Thread(ClockRoutine);
            _clockThread.Start();
        }

        public static void StopClock()
        {
            _isRunning = false;
        }

        private static void OnClockTick(ClockEventArgs e)
        {
            ClockTick?.Invoke(null, e);
        }
    }
}
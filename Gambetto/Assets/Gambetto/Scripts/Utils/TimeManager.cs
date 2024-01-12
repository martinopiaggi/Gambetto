using UnityEngine;

namespace Gambetto.Scripts.Utils
{
    /// <summary>
    /// Utility class to manage time scale
    /// </summary>
    public static class TimeManager
    {
        private static float _timeScale = 1f;

        public static float inputTimeInterval = 0.15f;
        
        public static void StopTime()
        {
            Time.timeScale = 0f;
        }
        
        public static void ResumeTime()
        {
            Time.timeScale = _timeScale;
        }
        
        public static void SetTimeScale(float timeScale)
        {
            _timeScale = timeScale;
            Time.timeScale = _timeScale;
        }
    }
}
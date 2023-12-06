using Gambetto.Scripts.GameCore.Grid;
using Gambetto.Scripts.Utils;
using UnityEngine;

namespace Gambetto.Scripts.UI
{
    public class EndOfLevelMenu : MonoBehaviour
    {
        GridManager gridManager;
        private void Awake()
        {
            gridManager = FindObjectOfType<GridManager>();
        }

        public void Retry()
        {
            TimeManager.ResumeTime();
            gridManager.RestartLevel();
        }
    }
}

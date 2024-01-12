using Gambetto.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gambetto.Scripts.UI
{
    public class PauseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// When the mouse is over an item drop location. It won't be possible to move the pawn.
        /// </summary>
        public static bool mouseOverItemDropLocation;

        /// <summary>
        /// On awake, set the mouse over item drop location to false. This will is necessary to avoid the static variable being true after a scene reload.
        /// </summary>
        public void Awake()
        {
            mouseOverItemDropLocation = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            mouseOverItemDropLocation = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            mouseOverItemDropLocation = false;
        }

        public void PauseGame()
        {
            TimeManager.StopTime();
        }

        public void ResumeGame()
        {
            TimeManager.ResumeTime();
            mouseOverItemDropLocation = false;
        }

        public void BackToMainMenu()
        {
            TimeManager.ResumeTime();
            GameManager.Instance.sceneTransition.CrossFade(1);
        }
    }
}

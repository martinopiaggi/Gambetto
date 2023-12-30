using System;
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
        public static bool MouseOverItemDropLocation;

        
        /// <summary>
        /// On awake, set the mouse over item drop location to false. This will is necessary to avoid the static variable being true after a scene reload.
        /// </summary>
        public void Awake()
        {
            MouseOverItemDropLocation = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            MouseOverItemDropLocation = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MouseOverItemDropLocation = false;
        }

        public void PauseGame()
        {
            TimeManager.StopTime();
        }

        public void ResumeGame()
        {
            TimeManager.ResumeTime();
            MouseOverItemDropLocation = false;
        }

        public void BackToMainMenu()
        {
            TimeManager.ResumeTime();
            GameManager.Instance.sceneTransition.CrossFade("Level selection");
        }
    }
}

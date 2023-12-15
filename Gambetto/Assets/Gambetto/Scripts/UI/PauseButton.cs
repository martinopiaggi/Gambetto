using Gambetto.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Gambetto.Scripts.UI
{
    public class PauseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [FormerlySerializedAs("MouseOverItemDropLocation")]
        public bool mouseOverItemDropLocation;

        public void OnPointerEnter(PointerEventData eventData)
        {
            mouseOverItemDropLocation = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            mouseOverItemDropLocation = false;
        }

        public void OpenSettings()
        {
            TimeManager.StopTime();
        }

        public void ResumeGame()
        {
            TimeManager.ResumeTime();
        }

        public void BackToMainMenu()
        {
            TimeManager.ResumeTime();
            GameManager.Instance.sceneTransition.CrossFade("Level selection");
        }
    }
}

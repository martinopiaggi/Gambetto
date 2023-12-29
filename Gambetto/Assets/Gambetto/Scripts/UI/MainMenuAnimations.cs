using System.Collections;
using UnityEngine;

namespace Gambetto.Scripts.UI
{
    public class MainMenuPlatformAnimation : MonoBehaviour
    {
        public Vector3 _finalPosition;

        public GameObject buttons;
        public GameObject title;

        // Start is called before the first frame update
        private void Awake()
        {
            _finalPosition = transform.position - new Vector3(0, 10f, 0);

            transform.Rotate(Vector3.up, -3f, Space.World);
        }

        public void AnimateMainMenu()
        {
            StartCoroutine(AnimationCoroutine());
        }

        private IEnumerator AnimationCoroutine()
        {
            var titleTransform = title.GetComponent<RectTransform>();
            var buttonsTransform = buttons.GetComponent<RectTransform>();
            var finalButtonsPosition = buttonsTransform.anchoredPosition - new Vector2(0, 100f);
            var finalTitlePosition = titleTransform.anchoredPosition + new Vector2(0, 100f);

            while (transform.position.y > _finalPosition.y)
            {
                //lerp pawn and platform to final position
                transform.position = Vector3.Lerp(
                    transform.position,
                    _finalPosition,
                    Time.deltaTime
                );
                transform.Rotate(Vector3.up, Time.deltaTime * 20f, Space.World);

                //lerp buttons and title to final position
                buttonsTransform.anchoredPosition = Vector2.Lerp(
                    buttonsTransform.anchoredPosition,
                    finalButtonsPosition,
                    Time.deltaTime
                );
                titleTransform.anchoredPosition = Vector2.Lerp(
                    titleTransform.anchoredPosition,
                    finalTitlePosition,
                    Time.deltaTime
                );

                yield return null;
            }
            enabled = false;
        }
    }
}

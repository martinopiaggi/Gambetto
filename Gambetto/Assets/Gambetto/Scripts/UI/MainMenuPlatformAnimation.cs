using UnityEngine;

namespace Gambetto.Scripts.UI
{
    public class MainMenuPlatformAnimation : MonoBehaviour
    {
        private Vector3 _finalPosition;
        private bool _rotationFinished;
        private bool _positionFinished;

        // Start is called before the first frame update
        private void Awake()
        {
            var transform1 = transform;
            var position = transform1.position;
            _finalPosition = position;
            position = new Vector3(position.x, position.y - 1f, position.z);
            transform1.position = position;
            transform.Rotate(Vector3.up, -3f, Space.World);
        }

        // Update is called once per frame
        private void Update()
        {
            //lerp to final position
            if (transform.position.y <= _finalPosition.y)
            {
                transform.position = Vector3.Lerp(
                    transform.position,
                    _finalPosition,
                    Time.deltaTime * 2f
                );
            }
            else
            {
                _positionFinished = true;
            }

            //rotate
            if (transform.rotation.y <= 0.0f)
            {
                transform.Rotate(Vector3.up, Time.deltaTime * 4f, Space.World);
            }
            else
            {
                _rotationFinished = true;
            }

            if (_positionFinished && _rotationFinished)
            {
                enabled = false;
            }
        }
    }
}

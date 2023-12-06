using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPlatformAnimation : MonoBehaviour
{
    private Vector3 _finalPosition;
    private bool _rotationFinished = false;
    private bool _positionFinished = false;

    // Start is called before the first frame update
    void Awake()
    {
        _finalPosition = transform.position;
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y - 1f,
            transform.position.z
        );
        transform.Rotate(Vector3.up, -3f, Space.World);
    }

    // Update is called once per frame
    void Update()
    {
        //lerp to final position
        if (transform.position.y <= _finalPosition.y)
        {
            transform.position = Vector3.Lerp(transform.position, _finalPosition, Time.deltaTime * 2f);
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
        
        
        if(_positionFinished && _rotationFinished)
        {
            enabled = false;
        }

    }
}

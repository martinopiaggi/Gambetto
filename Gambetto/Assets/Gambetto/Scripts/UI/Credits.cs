using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Gambetto.Scripts.UI
{
    public class Credits : MonoBehaviour
    {
        private Scrollbar scrollbar;
        private const float ScrollSpeed = 0.001f;
        private bool isAutoScrolling = true;

        private void Awake()
        {
            scrollbar = GetComponent<Scrollbar>();
            scrollbar.value = 1;
        }

        private void FixedUpdate()
        {
            if (scrollbar.value > 0 && isAutoScrolling)
                scrollbar.value -= ScrollSpeed;
            else
                isAutoScrolling = false;
        }
    }
}

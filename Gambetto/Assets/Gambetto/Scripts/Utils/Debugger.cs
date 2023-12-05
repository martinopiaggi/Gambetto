using Unity.VisualScripting;
using UnityEngine.Serialization;

namespace Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using TMPro;
    using UnityEngine.UI;

    public class Debugger : Singleton<Debugger>
    {
        [SerializeField]
        private bool isVisible = false;

        private Canvas _canvas;

        public enum Position
        {
            UpperLeft,
            UpperRight,
            LowerLeft,
            LowerRight,
            Center
        }

        [SerializeField] private TextMeshProUGUI upperLeft;
        [SerializeField] private TextMeshProUGUI upperRight;
        [SerializeField] private TextMeshProUGUI lowerLeft;
        [SerializeField] private TextMeshProUGUI lowerRight;
        [SerializeField] private TextMeshProUGUI center;

        private new void Awake()
        {
            base.Awake();
            upperLeft.text = "";
            upperRight.text = "";
            lowerLeft.text = "";
            lowerRight.text = "";
            center.text = "";
            _canvas = gameObject.GetComponent<Canvas>();
            _canvas.enabled = isVisible;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F3))
            {
                isVisible = !isVisible;
                _canvas.enabled = isVisible;
            }
        }

        public void Show(string text, Color color = default, Position position = Position.UpperLeft, bool printConsole = true)
        {
            if (printConsole) Debug.Log(text);
            
            color = color == default ? Color.white : color;
            
            switch (position)
            {
                case Position.UpperLeft:
                    upperLeft.text = text;
                    upperLeft.color = color;
                    break;
                case Position.UpperRight:
                    upperRight.text = text;
                    upperRight.color = color;
                    break;
                case Position.LowerLeft:
                    lowerLeft.text = text;
                    lowerLeft.color = color;
                    break;
                case Position.LowerRight:
                    lowerRight.text = text;
                    lowerRight.color = color;
                    break;
                case Position.Center:
                    center.text = text;
                    center.color = color;
                    break;
            }
        }
    }
}
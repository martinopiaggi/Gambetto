using Unity.VisualScripting;

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

        private void Start()
        {
            upperLeft.text = "";
            upperRight.text = "";
            lowerLeft.text = "";
            lowerRight.text = "";
            center.text = "";
        }

        public void Show(string text, Color? color, Position position = Position.UpperRight)
        {
            color ??= Color.white;
           
            switch (position)
            {
                case Position.UpperLeft:
                    upperLeft.text = text;
                    upperLeft.color = color.Value;
                    break;
                case Position.UpperRight:
                    upperRight.text = text;
                    upperRight.color = color.Value;
                    break;
                case Position.LowerLeft:
                    lowerLeft.text = text;
                    lowerLeft.color = color.Value;
                    break;
                case Position.LowerRight:
                    lowerRight.text =text;
                    lowerRight.color = color.Value;
                    break;
                case Position.Center:
                    center.text =text;
                    center.color = color.Value;
                    break;
            }
        }
  
    }

}
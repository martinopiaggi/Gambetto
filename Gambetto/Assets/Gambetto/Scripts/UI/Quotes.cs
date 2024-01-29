using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gambetto.Scripts.UI
{
    public class Quotes : MonoBehaviour
    {
        //create a dictionary containing name levels as key and the quote as value
        private Dictionary<string, string> levelQuotes =
            new()
            {
                { "tutortial", "lezzo" },
                { "KnightIntro", "sgocciola" },
                { "BishopIntro", "penzola" },
            };

        void Start()
        {
            //find the correct element in dictionary
            gameObject.GetComponent<TextMeshProUGUI>().text = levelQuotes[
                LevelSelector.instance.currentLevel
            ];
        }
    }
}

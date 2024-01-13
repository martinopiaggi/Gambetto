using System.Collections.Generic;
using Gambetto.Scripts.Utils;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace Gambetto.Scripts.UI
{
    public class LevelGroup : MonoBehaviour
    {
        [SerializeField]
        private GameObject buttonPrefab;

        [SerializeField]
        private List<string> levelNames;

        // On start instantiate all the buttons and append them as children of the current object
        private void Start()
        {
            var i = 0;
            foreach (var levelName in levelNames)
            {
                i++;
                var button = Instantiate(buttonPrefab, transform);
                button.name = levelName;
                button.GetComponent<LevelButton>().levelName = levelName;
                button.GetComponent<LevelButton>().nextLevels = levelNames;
                // get the text component of the child of the button
                button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text =
                    RomanNumeralGenerator.GenerateNumeral(i);
                var status = GameManager.Instance.GetLevelStatus(levelName);
                button.GetComponent<Button>().interactable = status;
            }
        }
    }
}

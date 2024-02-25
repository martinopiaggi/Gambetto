using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSimulator : MonoBehaviour
{
    public Button pauseButton;

    // Update is called once per frame
    void Update()
    {
        // Check if the user press ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Simulat the click of the pause button
            if (pauseButton.interactable && pauseButton.gameObject.activeInHierarchy)
            {
                pauseButton.onClick.Invoke();
            }
        }
    }
}

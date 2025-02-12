using UnityEngine;

public class UIToggleManager : MonoBehaviour
{
    public GameObject inventoryUI; // Reference to the main UI GameObject

    private bool isUIActive = false; // Tracks whether the UI is active

    void Update()
    {
        // Check if the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleUI();
        }
    }

    // Toggles the UI on/off
    private void ToggleUI()
    {
        isUIActive = !isUIActive; // Toggle the active state
        inventoryUI.SetActive(isUIActive); // Enable or disable the UI
    }
}

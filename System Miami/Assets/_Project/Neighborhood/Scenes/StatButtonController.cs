using UnityEngine;

public class StatsPanelController : MonoBehaviour
{
    public GameObject statsPanel; // Reference to the panel containing detailed stats
    private bool isExpanded = false; // Track whether the panel is expanded

    public void ToggleStatsPanel()
    {
        isExpanded = !isExpanded; // Toggle the state
        statsPanel.SetActive(isExpanded); // Show or hide the panel based on the state
    }
}

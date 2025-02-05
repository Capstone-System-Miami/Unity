using UnityEngine;

public class StatButtonController : MonoBehaviour
{
    public GameObject detailPanel; // The detailed info panel for this stat
    private bool isExpanded = false; // Tracks if the panel is expanded

    public void ToggleDetail()
    {
        isExpanded = !isExpanded; // Toggle state
        detailPanel.SetActive(isExpanded); // Show or hide the panel
    }
}

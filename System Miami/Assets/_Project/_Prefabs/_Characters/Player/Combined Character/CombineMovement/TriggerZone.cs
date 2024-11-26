using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public MovementModeManager modeManager; // Reference to the MovementModeManager

    public bool isNeighborhoodZone = false; // Set to true for Neighborhood zones

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure it's the player
        {
            if (isNeighborhoodZone)
            {
                modeManager.EnterNeighborhood(); // Updated method name
            }
            else
            {
                modeManager.EnterDungeon(); // Calls the dungeon method
            }
        }
    }
}

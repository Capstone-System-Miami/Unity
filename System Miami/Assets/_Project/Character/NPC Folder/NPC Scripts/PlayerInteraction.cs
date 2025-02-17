using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private NPC currentNpc;  // The current NPC the player is interacting with

    // Triggered when the player enters the NPC's trigger collider
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))  // Check if it's an NPC
        {
            currentNpc = other.GetComponent<NPC>();  // Assign the NPC to the currentNpc variable
        }
    }

    // Triggered when the player exits the NPC's trigger collider
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            currentNpc = null;  // Clear the current NPC when the player leaves the area
        }
    }

    // Update to check for interaction key press
    void Update()
    {
        if (currentNpc != null && Input.GetKeyDown(KeyCode.E)) // Press 'E' to interact
        {
            currentNpc.StartDialogue();
        }
    }
}

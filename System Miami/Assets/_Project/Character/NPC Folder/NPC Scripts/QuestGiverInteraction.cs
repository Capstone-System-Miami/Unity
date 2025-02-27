using UnityEngine;

public class QuestGiverInteraction : MonoBehaviour
{
    public QuestGiver questGiver; // Reference to the QuestGiver script
    public string interactionKey = "E"; // Key to press for interaction

    private bool isPlayerInRange = false; // Track if the player is within range

    // This method is called when another collider enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the player entered the trigger area
        {
            isPlayerInRange = true;
            Debug.Log("Press E to interact with the Quest Giver.");
        }
    }

    // This method is called when another collider exits the trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the player left the trigger area
        {
            isPlayerInRange = false;
            Debug.Log("You are too far to interact with the Quest Giver.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is within range and presses the interaction key (e.g., 'E')
        if (isPlayerInRange && Input.GetKeyDown(interactionKey))
        {
            questGiver.TalkToQuestGiver(); // Call the method to start the quest
            Debug.Log("Interacting with the Quest Giver.");
        }
    }
}

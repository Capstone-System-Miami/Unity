using UnityEngine;

public class NPC : MonoBehaviour
{
    public string npcName;
    public string[] dialogueLines;
    public Quest npcQuest;
    public GameObject questUI;
    public bool hasInteracted = false;

    private bool isPlayerNearby = false; // Track if the player is nearby for interaction

    // Detect when the player enters the NPC's trigger collider
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasInteracted)
        {
            isPlayerNearby = true;  // Player is nearby, can interact
            ShowInteractionPrompt();
        }
    }

    // Detect when the player exits the NPC's trigger collider
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false; // Player is no longer in range
            HideInteractionPrompt();
        }
    }

    // Show an interaction prompt (this can be a UI element, text, etc.)
    void ShowInteractionPrompt()
    {
        Debug.Log("Press 'E' to interact with " + npcName);
        // Optionally, you can activate a UI prompt here
    }

    // Hide the interaction prompt when the player leaves
    void HideInteractionPrompt()
    {
        Debug.Log("You are no longer near " + npcName);
        // Optionally, you can hide the UI prompt here
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E)) // Press 'E' to interact
        {
            StartDialogue();
        }
    }

    // Start the NPC's dialogue and give the quest
    public void StartDialogue()  
    {
        if (!hasInteracted)
        {
            hasInteracted = true;
            DisplayDialogue();
            GiveQuest();
        }
    }


    // Display dialogue 
    void DisplayDialogue()
    {
        foreach (var line in dialogueLines)
        {
            Debug.Log(line); 
        }
    }

    // Give the quest to the player
    void GiveQuest()
    {
        if (npcQuest != null)
        {
            questUI.SetActive(true);
            Debug.Log($"Quest given: {npcQuest.questTitle}");
            Debug.Log($"Description: {npcQuest.questDescription}");
        }
    }

    // When the quest is completed by the player
    public void CompleteQuest()
    {
        if (npcQuest.IsCompleted)
        {
            Debug.Log($"{npcName}: Quest Complete!");
            questUI.SetActive(false);
            npcQuest.ResetQuest(); // Reset the quest for future use
        }
    }
}

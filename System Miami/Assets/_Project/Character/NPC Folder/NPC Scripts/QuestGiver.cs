using UnityEngine;
using TMPro; 

[System.Serializable] 
public class Quest
{
    public string questName;
    public string[] questDescriptionLines; // Multiple lines of text for the quest description
    public string targetEnemyTag = "Goblin"; // Tag to track
    public int objectiveGoal = 5; // How many enemies to defeat
    public int rewardEXP;
    public int rewardCurrency;
}

public class QuestGiver : MonoBehaviour
{
    public Quest[] allPossibleQuests; // Array of possible quests
    private Quest selectedQuest; // The current quest assigned to the player

   
    public TMP_Text questNameText;
    public TMP_Text questDescriptionText;
    public TMP_Text progressText;
    public TMP_Text rewardText;
    public GameObject descriptionPanel;

    private int objectiveCount = 0; // How many enemies have been defeated
    private int currentDescriptionIndex = 0; // Index to track which line of text is shown
    private bool isQuestAccepted = false;
    private bool isQuestCompleted = false;

    void Start()
    {
        // Initially disable the quest UI
        if (questNameText != null) questNameText.gameObject.SetActive(false);
        if (questDescriptionText != null) questDescriptionText.gameObject.SetActive(false);
        if (progressText != null) progressText.gameObject.SetActive(false);
        if (rewardText != null) rewardText.gameObject.SetActive(false);
        if (descriptionPanel != null) descriptionPanel.SetActive(false);
    }

    // Call this method when the player interacts with the quest giver
    public void TalkToQuestGiver()
    {
        // Randomly select a quest from all possible quests
        selectedQuest = allPossibleQuests[Random.Range(0, allPossibleQuests.Length)];

        // Start the quest and reset progress
        isQuestAccepted = true;
        isQuestCompleted = false;
        objectiveCount = 0;
        currentDescriptionIndex = 0; // Start at the first line of description
        Debug.Log($"Quest Started: {selectedQuest.questName}\n{selectedQuest.questDescriptionLines[0]}");

        UpdateUI();
    }

    public void EnemyDefeated(GameObject enemy)
    {
        if (!isQuestAccepted || isQuestCompleted)
            return;

        if (enemy.CompareTag(selectedQuest.targetEnemyTag)) // Check if the defeated enemy has the correct tag
        {
            objectiveCount++;
            Debug.Log($"Progress: {objectiveCount}/{selectedQuest.objectiveGoal}");

            // Update UI progress
            UpdateProgressUI();

            if (objectiveCount >= selectedQuest.objectiveGoal)
            {
                CompleteQuest();
            }
        }
    }

    private void CompleteQuest()
    {
        if (!isQuestCompleted)
        {
            isQuestCompleted = true;
            Debug.Log($"Congratulations! You completed {selectedQuest.questName}. Reward: {selectedQuest.rewardEXP} EXP, {selectedQuest.rewardCurrency} Currency.");

            // Update UI to show rewards
            UpdateRewardUI();

            //  add logic to give rewards to the player
        }
    }

    // Helper method to update all UI elements at once
    private void UpdateUI()
    {
        if (questNameText != null) questNameText.gameObject.SetActive(true);
        if (questDescriptionText != null) questDescriptionText.gameObject.SetActive(true);
        if (progressText != null) progressText.gameObject.SetActive(true);
        if (rewardText != null) rewardText.gameObject.SetActive(true);
        if (descriptionPanel != null) descriptionPanel.SetActive(true);

        // Update the quest UI elements
        questNameText.text = selectedQuest.questName;
        questDescriptionText.text = selectedQuest.questDescriptionLines[currentDescriptionIndex];
        UpdateProgressUI();
    }

    // Helper method to update progress UI
    private void UpdateProgressUI()
    {
        if (progressText != null)
            progressText.text = $"Defeat {objectiveCount}/{selectedQuest.objectiveGoal} {selectedQuest.targetEnemyTag}s";
    }

    // Helper method to update reward UI
    private void UpdateRewardUI()
    {
        if (rewardText != null)
            rewardText.text = $"Reward: {selectedQuest.rewardEXP} EXP, {selectedQuest.rewardCurrency} Currency";
    }

    // Method to show the next line of the quest description
    public void ShowNextLine()
    {
        if (currentDescriptionIndex < selectedQuest.questDescriptionLines.Length - 1)
        {
            currentDescriptionIndex++;
            questDescriptionText.text = selectedQuest.questDescriptionLines[currentDescriptionIndex];
        }
        else
        {
            Debug.Log("You have read all the lines of the quest description.");
        }
    }

    void Update()
    {
        // Use either key press or button to progress
        if (Input.GetKeyDown(KeyCode.Space)) // Press Space to go to next line
        {
            ShowNextLine();
        }
    }
}

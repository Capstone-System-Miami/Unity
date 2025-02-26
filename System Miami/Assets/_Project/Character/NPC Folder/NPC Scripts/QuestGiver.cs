using UnityEngine;
using TMPro; // Required for TextMeshPro components
using System.Collections.Generic; // For using List<T>

[System.Serializable] // Make this class serializable for the Inspector
public class Quest
{
    public string questName;
    public string[] questDescriptionLines; // Multiple lines of text for the quest description
    public string targetEnemyTag = "Goblin"; // Tag to track
    public int objectiveGoal = 5; // How many enemies to defeat
    public int rewardEXP;
    public int rewardCurrency;

    public int objectiveCount = 0; // How many have been defeated
    public bool isQuestCompleted = false; // Whether the quest is completed
}

public class QuestGiver : MonoBehaviour
{
    public Quest[] allPossibleQuests; // Array of possible quests
    private List<Quest> activeQuests = new List<Quest>(); // List of active quests the player is working on

    // UI References
    public GameObject questEntryPrefab; // Prefab for quest entry UI (Assign in inspector)
    public Transform questListParent; // Parent object to hold all quest entries (assign the content of a ScrollView here)
    public GameObject descriptionPanel; // Panel for background behind text

    // References to TMP_Text for the UI elements
    public TMP_Text questNameText; // TextMeshPro component for the quest name
    public TMP_Text questDescriptionText; // TextMeshPro component for the quest description
    public TMP_Text progressText; // TextMeshPro component for quest progress
    public TMP_Text rewardText; // TextMeshPro component for quest reward

    private int currentDescriptionIndex = 0; // Index to track which line of text is shown

    void Start()
    {
        // Initially disable the quest UI
        if (questListParent != null) questListParent.gameObject.SetActive(false);
        if (descriptionPanel != null) descriptionPanel.SetActive(false);
        if (questNameText != null) questNameText.gameObject.SetActive(false);
        if (questDescriptionText != null) questDescriptionText.gameObject.SetActive(false);
        if (progressText != null) progressText.gameObject.SetActive(false);
        if (rewardText != null) rewardText.gameObject.SetActive(false);
    }

    // Call this method when the player interacts with the quest giver
    public void TalkToQuestGiver()
    {
        // Randomly select a quest from all possible quests
        Quest newQuest = allPossibleQuests[Random.Range(0, allPossibleQuests.Length)];

        // Add the new quest to active quests list if not already present
        if (!activeQuests.Contains(newQuest))
        {
            activeQuests.Add(newQuest);
            Debug.Log($"New quest added: {newQuest.questName}");
        }

        // Enable the quest UI and update the UI for all active quests
        UpdateUI();
    }

    public void EnemyDefeated(GameObject enemy)
    {
        // Check each active quest to see if it needs updating
        foreach (Quest quest in activeQuests)
        {
            if (quest.isQuestCompleted) continue;

            if (enemy.CompareTag(quest.targetEnemyTag)) // Check if the defeated enemy has the correct tag
            {
                quest.objectiveCount++;
                Debug.Log($"Progress: {quest.objectiveCount}/{quest.objectiveGoal} for {quest.questName}");

                // Update UI progress for this quest
                UpdateQuestProgressUI();

                if (quest.objectiveCount >= quest.objectiveGoal)
                {
                    CompleteQuest(quest);
                }
            }
        }
    }

    private void CompleteQuest(Quest quest)
    {
        if (!quest.isQuestCompleted)
        {
            quest.isQuestCompleted = true;
            Debug.Log($"Congratulations! You completed {quest.questName}. Reward: {quest.rewardEXP} EXP, {quest.rewardCurrency} Currency.");

            // Update UI to show rewards for this quest
            UpdateQuestRewardUI(quest);
        }
    }

    // Helper method to update all UI elements at once for multiple quests
    private void UpdateUI()
    {
        if (questListParent != null)
        {
            questListParent.gameObject.SetActive(true); // Enable the quest list UI container

            // Clear previous UI elements
            foreach (Transform child in questListParent)
            {
                Destroy(child.gameObject); // Destroy any existing quest entries
            }

            // Loop through all active quests and create a UI entry for each one
            foreach (Quest quest in activeQuests)
            {
                GameObject questEntry = Instantiate(questEntryPrefab, questListParent);
                QuestEntryUI questEntryUI = questEntry.GetComponent<QuestEntryUI>();

                if (questEntryUI != null)
                {
                    // Set the quest information
                    questEntryUI.SetQuestInfo(quest);
                }
            }
        }
    }

    // Helper method to update progress UI for each quest
    private void UpdateQuestProgressUI()
    {
        foreach (Transform child in questListParent)
        {
            QuestEntryUI questEntryUI = child.GetComponent<QuestEntryUI>();
            if (questEntryUI != null)
            {
                questEntryUI.UpdateProgress();
            }
        }
    }

    // Helper method to update reward UI for each quest
    private void UpdateQuestRewardUI(Quest quest)
    {
        foreach (Transform child in questListParent)
        {
            QuestEntryUI questEntryUI = child.GetComponent<QuestEntryUI>();
            if (questEntryUI != null && questEntryUI.quest == quest)
            {
                questEntryUI.UpdateReward();
            }
        }
    }
}

public class QuestEntryUI : MonoBehaviour
{
    // TMP_Text components for the quest name, description, progress, and reward
    public TMP_Text questNameText;
    public TMP_Text questDescriptionText;
    public TMP_Text progressText;
    public TMP_Text rewardText;

    public Quest quest;

    // Set quest information for UI
    public void SetQuestInfo(Quest quest)
    {
        this.quest = quest;
        questNameText.text = quest.questName;
        questDescriptionText.text = quest.questDescriptionLines[0]; // Set the first description line
        progressText.text = $"Progress: {quest.objectiveCount}/{quest.objectiveGoal}";
        rewardText.text = $"Reward: {quest.rewardEXP} EXP, {quest.rewardCurrency} Currency";
    }

    // Update progress for this quest
    public void UpdateProgress()
    {
        progressText.text = $"Progress: {quest.objectiveCount}/{quest.objectiveGoal}";
    }

    // Update reward for this quest
    public void UpdateReward()
    {
        rewardText.text = $"Reward: {quest.rewardEXP} EXP, {quest.rewardCurrency} Currency";
    }
}

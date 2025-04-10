using System;
using SystemMiami;
using SystemMiami.Management;
using SystemMiami.ui;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;



public class QuestGiver : MonoBehaviour
{
    public Quest assignedQuest; // Array of possible quests
    public Quest selectedQuest; // The current quest assigned to the player
    public string questNPCname;
    
   
    public GameObject questPanel;

    private int objectiveCount = 0; // How many enemies have been defeated
    private int currentDescriptionIndex = 0; // Index to track which line of text is shown
    private bool isQuestAccepted = false;
    private bool isQuestCompleted = false;

    void Start()
    {
        if (questPanel != null) questPanel.SetActive(false);
    }

    void Update()
    {
        if (questPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                questPanel.SetActive(false);
            }
        }
    }

    public void Initialize(NPCInfoSO npcInfoSo , string npcName, GameObject panelPrefab)
    {
        assignedQuest = npcInfoSo.GetQuest();
        questNPCname = npcName;
        questPanel = Instantiate(panelPrefab,transform);
        QuestPanel questPanelComponent = panelPrefab.GetComponent<QuestPanel>();
        questPanelComponent.Initialize(assignedQuest);
        questPanel.SetActive(false);
        Debug.Log($"{npcName} assigned quest: {assignedQuest.questName}");
   
    }
    
    // Call this method when the player interacts with the quest giver
    public void TalkToQuestGiver()
    {
       
        UI.MGR.StartDialogue(this,true,true,false,questNPCname,assignedQuest.questDialogue);
        UI.MGR.DialogueFinished += HandleDialogueFinished;

    }

    private void HandleDialogueFinished(object sender, EventArgs args)
    {
        OpenQuestWindow();
        UI.MGR.DialogueFinished -= HandleDialogueFinished;
    }

    public void OpenQuestWindow()
    {
        questPanel.SetActive(true);
        // Start the quest and reset progress
        //isQuestAccepted = true;
        //isQuestCompleted = false;
       // objectiveCount = 0;
       // UpdateUI();
       QuestTracker.MGR.AcceptQuest(assignedQuest);
    }
    
    

    /*public void EnemyDefeated(GameObject enemy)
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
    }*/

    

    

    
}

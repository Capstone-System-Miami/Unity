using System;
using SystemMiami;
using SystemMiami.Management;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public Quest assignedQuest; // Array of possible quests
    public string questNPCname;
    
    private int objectiveCount = 0; // How many enemies have been defeated
    private int currentDescriptionIndex = 0; // Index to track which line of text is shown
    private bool isQuestAccepted = false;
    private bool isQuestCompleted = false;

    public void Initialize(NPCInfoSO npcInfoSo , string npcName, GameObject panelPrefab)
    {
        assignedQuest = npcInfoSo.GetQuest();
        questNPCname = npcName;
        QuestPanel questPanelComponent = panelPrefab.GetComponent<QuestPanel>();
        questPanelComponent.Initialize(assignedQuest);
        // Debug.Log($"{npcName} assigned quest: {assignedQuest.questName}");
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
        QuestTracker.MGR.AcceptQuest(assignedQuest);
    }
   
}

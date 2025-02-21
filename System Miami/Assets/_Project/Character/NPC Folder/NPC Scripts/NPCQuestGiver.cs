// Author: Johnny Sosa


using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace SystemMiami
{
    public class NPCQuestGiver : MonoBehaviour
    {
        [Header("NPC Info")]
        [SerializeField] private string npcName;
        [SerializeField] private List<Quest> availableQuests;
        private Quest currentQuest;
        private bool hasGivenQuest = false;

        [Header("Events")]
        public UnityEvent<string> OnDialogueUpdate; // Update UI with dialogue
        public UnityEvent<Quest> OnQuestAssigned;   // Notify quest assignment
        public UnityEvent<Quest> OnQuestCompleted;  // Notify quest completion

        public void StartDialogue()
        {
            if (!hasGivenQuest)
            {
                AssignQuest();
            }
            else if (currentQuest != null && currentQuest.IsCompleted)
            {
                CompleteQuest();
            }
            else
            {
                OnDialogueUpdate?.Invoke("You still have a quest to complete!");
            }
        }

        private void AssignQuest()
        {
            if (availableQuests.Count > 0)
            {
                currentQuest = availableQuests[0];  // You can change this to a random selection if needed
                hasGivenQuest = true;
                OnQuestAssigned?.Invoke(currentQuest);
                Debug.Log($"{npcName} assigned a quest: {currentQuest.questTitle}");
            }
        }

        private void CompleteQuest()
        {
            if (currentQuest != null && currentQuest.IsCompleted)
            {
                currentQuest.GrantRewards(); // Give XP & Gold

                OnQuestCompleted?.Invoke(currentQuest);
                availableQuests.Remove(currentQuest);  // Remove the completed quest
                hasGivenQuest = false;  // Reset so the NPC can give a new quest
            }
        }
    }
}

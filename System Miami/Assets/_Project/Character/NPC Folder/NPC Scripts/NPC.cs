// Author: Johnny Sosa

using UnityEngine;
using UnityEngine.Events;

namespace SystemMiami
{
    public class NPC : MonoBehaviour
    {
        [Header("NPC Settings")]
        [SerializeField] private string npcName;
        [SerializeField] private string[] dialogueLines;
        [SerializeField] private Quest assignedQuest;

        private int dialogueIndex = 0;
        private bool hasGivenQuest = false;

        [Header("Events")]
        public UnityEvent<string> OnDialogueUpdate; // Event to update UI with dialogue
        public UnityEvent<Quest> OnQuestAssigned;   // Event to notify quest assignment

        // Called by InteractionTrigger when interacting
        public void StartDialogue()
        {
            if (dialogueLines.Length > 0 && dialogueIndex < dialogueLines.Length)
            {
                OnDialogueUpdate?.Invoke(dialogueLines[dialogueIndex]); // Update UI
                dialogueIndex++;
            }
            else
            {
                EndDialogue();
            }
        }

        private void EndDialogue()
        {
            if (!hasGivenQuest && assignedQuest != null)
            {
                AssignQuest();
            }
        }

        private void AssignQuest()
        {
            hasGivenQuest = true;
            OnQuestAssigned?.Invoke(assignedQuest);
            Debug.Log($"{npcName} has assigned a quest: {assignedQuest.questTitle}");
        }
    }
}

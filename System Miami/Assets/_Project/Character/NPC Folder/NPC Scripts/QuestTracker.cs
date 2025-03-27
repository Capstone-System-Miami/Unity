using System;
using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.InventorySystem;
using SystemMiami.Management;
using UnityEngine;

namespace SystemMiami
{
    public class QuestTracker : Singleton<QuestTracker>
    {
        public Quest activeQuest;
        [SerializeField] private PlayerLevel playerLevel;
        [SerializeField] private Inventory playerInventory;
        [SerializeField] private QuestPanel questPanel;
        public void AcceptQuest(Quest quest)
        {
            if (activeQuest != null)
            { 
                activeQuest.Reset();
            }
            activeQuest = quest;
            questPanel.Initialize(activeQuest);
            GAME.MGR.CombatantDeath += HandleCombatantDeath;
        }

        private void HandleCombatantDeath(Combatant obj)
        {
            Debug.Log($"Combatant died. Object layer: {obj.gameObject.layer}");
            Debug.Log($"Target enemy layer: {activeQuest.targetEnemyLayer.value}");
            if ((1 << obj.gameObject.layer & activeQuest.targetEnemyLayer.value) != 0)
            {
               UpdateQuest();
            }
        }


        public void UpdateQuest()
        {
            Debug.Log($"Updating Quest: {activeQuest.questName} QUEST");
            Debug.Log($"Current Enemies: {activeQuest.enemiesToGoal} QUEST");
            Debug.Log($"Objective Goal: {activeQuest.objectiveGoal} QUEST");
            activeQuest.AddQuestProgress();
            Debug.Log($"Updated Enemies: {activeQuest.enemiesToGoal} QUEST");
            UpdateUI();
            if (activeQuest.enemiesToGoal == activeQuest.objectiveGoal)
            {
                CompleteQuest();
            }
        }
        
        private void CompleteQuest()
        {
            activeQuest.CompleteQuest();
            questPanel.CompleteQuest();
            activeQuest.Reset();
            GiveQuestRewards();
            Debug.Log($"Congratulations! You completed {activeQuest.questName}. Reward: {activeQuest.rewardEXP} EXP, {activeQuest.rewardCurrency} Currency.");
            GAME.MGR.CombatantDeath -= HandleCombatantDeath;
        }

        public void GiveQuestRewards()
        {
            playerLevel.GainXP(activeQuest.rewardEXP);
            playerInventory.AddCredits(activeQuest.rewardCurrency);
        }

        // Helper method to update all UI elements at once
        private void UpdateUI()
        {
           questPanel.UpdateQuest();
        }

        // Helper method to update progress UI
        private void UpdateProgressUI()
        {
            //change actual quest prgress text
            // if (progressText != null)
            //     progressText.text = $"Defeat {objectiveCount}/{selectedQuest.objectiveGoal} {selectedQuest.targetEnemyTag}s";
        }

    }
}

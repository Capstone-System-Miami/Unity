using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SystemMiami
{
    [System.Serializable] 
    public class Quest
    {
        public string questName;
        public string questDescriptionLine; 
        [FormerlySerializedAs("targetEnemyTag")] public LayerMask targetEnemyLayer; // Tag to track
        public int enemiesToGoal;
        public int objectiveGoal; // How many enemies to defeat
        public int rewardEXP;
        public int rewardCurrency;
        public string[] questDialogue;
        public bool questCompleted;

        
        public Quest()
        {
            questName = "Quest Name";
            questDescriptionLine = "Quest Description";
            targetEnemyLayer = 4;
            objectiveGoal = 1000;
            rewardEXP = 0;
            rewardCurrency = 0;
            questDialogue = new string[]
            {
                "Quest Initiation Dialogue 1",
                "Quest Initiation Dialogue 2",
            };
            
        }

        public void AddQuestProgress()
        {
            Debug.Log($"Before increment: enemiesToGoal = {enemiesToGoal} QUEST");
            enemiesToGoal++;
            Debug.Log($"After increment: enemiesToGoal = {enemiesToGoal} QUEST");
        }

        public void CompleteQuest()
        {
            questCompleted = true;
        }

        public void Reset()
        {
            objectiveGoal = Random.Range(3, 12);
            rewardEXP = objectiveGoal * Random.Range(6, 15);
            rewardCurrency = objectiveGoal * Random.Range(8, 16);
            enemiesToGoal = 0;
            questCompleted = false;
        }
    }

    [System.Serializable]
    public class ShopData
    {
        public ItemType shopType;
        public string[] shopDialogue;

        public ShopData()
        {
            shopType = 0;
            shopDialogue = new string[]
            {
                "Shop Initiation Dialogue 1",
                "Shop Initiation Dialogue 2",
            };
        }
    }
}

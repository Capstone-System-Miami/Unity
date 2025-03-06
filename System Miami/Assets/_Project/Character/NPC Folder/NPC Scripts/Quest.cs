using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    [System.Serializable] 
    public class Quest
    {
        public string questName;
        public string questDescriptionLine; 
        public string targetEnemyTag ; // Tag to track
        public int objectiveGoal; // How many enemies to defeat
        public int rewardEXP;
        public int rewardCurrency;
        public string[] questDialogue;

        public Quest()
        {
            questName = "Quest Name";
            questDescriptionLine = "Quest Description";
            targetEnemyTag = "Enemy";
            objectiveGoal = 1000;
            rewardEXP = 0;
            rewardCurrency = 0;
            questDialogue = new string[]
            {
                "Quest Initiation Dialogue 1",
                "Quest Initiation Dialogue 2",
            };
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

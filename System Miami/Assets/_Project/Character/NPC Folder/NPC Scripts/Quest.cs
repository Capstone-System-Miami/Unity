using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    [System.Serializable] 
    public struct Quest
    {
        public string questName;
        public string questDescriptionLine; 
        public string targetEnemyTag ; // Tag to track
        public int objectiveGoal; // How many enemies to defeat
        public int rewardEXP;
        public int rewardCurrency;
        public string[] questDialogue;
    
    
    }
[System.Serializable]
    public struct ShopData
    {
        public ItemType shopType;
        public string[] shopDialogue;
    }
}

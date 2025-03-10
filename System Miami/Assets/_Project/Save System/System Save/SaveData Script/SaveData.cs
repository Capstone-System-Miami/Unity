using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    public class SaveData
    {
        public int playerLevel;
        public int playerCurrency;
        public int playerExperience;
        public string playerClassType;

        public List<string> playerMagicalAbilities = new List<string>();

        public List<string> playerPhysicalAbilities = new List<string>();

        public List<string> playerTools = new List<string>();
        public List<QuestData> activeQuests = new List<QuestData>();
    }
}

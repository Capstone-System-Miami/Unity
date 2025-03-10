using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    public class SaveData
    {
        public int playerLevel; // Level
        public int playerCredits; // Money
        public int playerExperience; // EXP
        public string playerClassType; //May need to change this so its more dynamic

        public List<string> magicalAbilities = new List<string>(); // magical
        public List<string> physicalAbilities = new List<string>(); // physical
        public List<string> playerTools = new List<string>(); // Shop tools
        public List<QuestData> activeQuests = new List<QuestData>(); // Quest


        public ExamplePlayer examplePlayer;
        public UnrelatedClass randomOtherClass;
    }
}

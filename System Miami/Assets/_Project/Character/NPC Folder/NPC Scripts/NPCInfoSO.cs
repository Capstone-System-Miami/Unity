using System.Collections.Generic;
using UnityEngine;


namespace SystemMiami
{
    [CreateAssetMenu(fileName = "NPCInfo", menuName = "Data/NPCInfo")]
    public class NPCInfoSO : ScriptableObject
    {
        public List<string>  possibleNames;
        public List<Sprite> possibleSprites;
        public List<Quest> possibleQuests = new();
        public List<ShopData> possibleShops;
        
        [SerializeField] private List<Quest> availableQuests;

        public void Initialize()  
        {
            foreach (Quest quest in possibleQuests)
            {
                quest.Reset();
            }
            availableQuests = new(possibleQuests);
        }
        
        public Quest GetQuest()
        {
            // If no available quests, reset from possible quests
            if (availableQuests == null || availableQuests.Count == 0)
            {
                availableQuests = new List<Quest>(possibleQuests);
            }

            // Select a random quest from available quests
            int index = Random.Range(0, availableQuests.Count);
            Quest selectedQuest = availableQuests[index];
    
            // Remove the selected quest from available quests
            availableQuests.RemoveAt(index);

            return selectedQuest;
        }
        
        public ShopData GetShop()
        {
            return possibleShops[Random.Range(0, possibleShops.Count)];
        }
    }
}

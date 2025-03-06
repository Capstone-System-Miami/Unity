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
        
        [SerializeField, ReadOnly] private List<Quest> availableQuests;

        public void Initialize()  
        {
            availableQuests = new(possibleQuests);
        }
        
        public Quest GetQuest()
        {
            bool noUniqueQuests =
                availableQuests != null
                && availableQuests.Count > 0;

            if (noUniqueQuests)
            {
                Debug.LogWarning($"DUPLICATE QUEST being assigned");
            }

            return noUniqueQuests
                ? availableQuests[Random.Range(0, availableQuests.Count)]
                : possibleQuests[Random.Range(0, possibleQuests.Count)];
        }
        
        public ShopData GetShop()
        {
            return possibleShops[Random.Range(0, possibleShops.Count)];
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace SystemMiami
{
    [CreateAssetMenu(fileName = "NPCInfo", menuName = "Data/NPCInfo")]
    public class NPCInfoSO : ScriptableObject
    {
        public List<string>  possibleNames ;
        public List<Sprite> possibleSprites ;
        public List<Quest> possibleQuests = new();
        public List<Quest> availableQuests;
        public List<ShopData> possibleShops;
        
        public void Initialize()  
        {
            availableQuests = new (possibleQuests);
        }
        
        public Quest GetQuest()
        {
            Quest quest = availableQuests[UnityEngine.Random.Range(0, availableQuests.Count)];
            availableQuests.Remove(quest);
            return quest;
        }
        
        public ShopData GetShop()
        {
            ShopData shop = possibleShops[UnityEngine.Random.Range(0, possibleShops.Count)];
            return shop;
        }
        
        
    }
}

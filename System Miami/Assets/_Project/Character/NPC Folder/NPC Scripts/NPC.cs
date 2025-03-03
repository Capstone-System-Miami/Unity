using System;
using System.Collections;
using System.Collections.Generic;
using SystemMiami.Shop;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace SystemMiami
{
    public class NPC : MonoBehaviour
    {
        public string npcName;
        public SpriteRenderer sprite;
        
       public NPCInfoSO npcInfoSo;

       public bool assigned;

       public GameObject questPanel;
       public GameObject shopPanel; 
       
       public NPCType myType;
       
       QuestGiver questGiver;
       ShopKeeper shopKeeper;

        private void Start()
        {
            
            
        }

        public void Initialize(NPCType npcType)
        {
            myType = npcType;
            npcName = npcInfoSo.possibleNames[Random.Range(0, npcInfoSo.possibleNames.Count)];
            sprite.sprite = npcInfoSo.possibleSprites[Random.Range(0, npcInfoSo.possibleSprites.Count)];
            gameObject.name = npcName;
            if (myType == NPCType.QuestGiver)
            {
                questGiver = gameObject.AddComponent<QuestGiver>();
                questGiver.Initialize(npcInfoSo, npcName, questPanel);
            }
            else if (myType == NPCType.ShopKeeper)
            {
                shopKeeper = gameObject.AddComponent<ShopKeeper>();
                shopKeeper.Initialize(npcInfoSo, npcName, shopPanel);
            }
            else
            {
                
            }
        }
        public void InteractWithPlayer()
        {
            if (myType == NPCType.QuestGiver)
            {
                questGiver.TalkToQuestGiver();
            }
            else if (myType == NPCType.ShopKeeper)
            {
                
              shopKeeper.TalkToShopKeeper();
            }
        }

        
        
    }
}

public enum NPCType
{
    QuestGiver,
    ShopKeeper,
    Dialogue
}
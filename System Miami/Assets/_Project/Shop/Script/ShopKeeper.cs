using System;
using System.Collections.Generic;
using System.Linq;
using SystemMiami.Management;
using TMPro;
using UnityEngine;

namespace SystemMiami.Shop
{
    public class ShopKeeper : MonoBehaviour
    {
        public int playerCredits;
        public TMP_Text coinUI;
        public List <ShopItemSlot> slots = new();
        public ShopData shop;
        private List<ItemData> ShopItems = new();
        
        public string myName;
        public GameObject shopPanel;
        [SerializeField] private ShopImage shopImage;


        void Start()
        {
            
            playerCredits = PlayerManager.MGR.CurrentCredits;
        }
        
        

        public void Initialize(NPCInfoSO npcInfo,string myName, GameObject shopPanel)
        {
            shop = npcInfo.GetShop();
            this.myName = myName;
            this.shopPanel = Instantiate(shopPanel);
            this.shopPanel.transform.SetParent(transform);
            this.shopPanel.SetActive(false);
            slots.AddRange(this.shopPanel.GetComponentsInChildren<ShopItemSlot>());
            shopImage = shopPanel.GetComponent<ShopImage>();
            shopImage.SetImage(GetComponent<SpriteRenderer>().sprite);


            // Debug.Log(shopPanel.GetComponentsInChildren<ShopItemSlot>());
            for (int i = 0; i < slots.Count; i++)
            {
               Add(Database.MGR.GetRandomDataOfType(shop.shopType));
            }
            FillSlots();
           
        }
        
        // Call this method when the player interacts with the quest giver
        public void TalkToShopKeeper()
        {
       
            UI.MGR.StartDialogue(this,true,true,false,myName,shop.shopDialogue);
            UI.MGR.DialogueFinished -= HandleDialogueFinished;
            UI.MGR.DialogueFinished += HandleDialogueFinished;

        }

      
        private void HandleDialogueFinished(object sender, EventArgs args)
        {
            OpenShopPanel();
            UI.MGR.DialogueFinished -= HandleDialogueFinished;
        }

        public void OpenShopPanel()
        {
            shopPanel.SetActive(true);
            
        }

       

        public void AddCoins()
        {
            playerCredits++;
        }

        public void CheckPurchaseable()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (!slots[i].gameObject.activeSelf)
                    continue;

                if (slots[i].ItemIsPurchaseable(playerCredits))
                    slots[i].EnableButton();
                else
                    slots[i].DisableButton();
            }
        }

       

        public void Add(ItemData item)
        {
            ShopItems.Add(item);
        }

        public void Add(List<ItemData> items)
        {
            ShopItems.AddRange(items);
        }

        public void FillSlots()
        {
            for (int i = 0; i < ShopItems.Count && i < slots.Count; i++)
            {
                slots[i].Fill(ShopItems[i]);
            }
        }

        public void LoadSlots()
        {
            for (int i = 0; i < ShopItems.Count && i < slots.Count; i++)
            {
                slots[i].Fill(ShopItems[i]);
            }
        }

       
    }
}

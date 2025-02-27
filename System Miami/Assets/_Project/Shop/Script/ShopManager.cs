using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SystemMiami.Shop
{
    public class ShopManager : MonoBehaviour
    {
        public int coins;
        public TMP_Text coinUI;
        public ShopItemSlot[] slots;
        public ShopItemSO[] ShopItemSOs;
        public EquipmentExampleSO[] EquipmentExampleSOs;

        private List<IShopItem> ShopItems = new();

        void Start()
        {
            LoadTestArrays();

            for (int i = 0; i < ShopItems.Count && i < slots.Length; i++)
                slots[i].gameObject.SetActive(true);

            FillSlots();
        }

        void Update()
        {
            CheckPurchaseable();
            coinUI.text = "Coins: " + coins;
        }

        public void AddCoins()
        {
            coins++;
        }

        public void CheckPurchaseable()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].gameObject.activeSelf)
                    continue;

                if (slots[i].ItemIsPurchaseable(coins))
                    slots[i].EnableButton();
                else
                    slots[i].DisableButton();
            }
        }

        public void PurchaseItem(int slotNumber)
        {
            if (slotNumber < 0 || slotNumber >= ShopItems.Count)
            {
                Debug.LogWarning("Invalid slot number.");
                return;
            }

            coins -= ShopItems[slotNumber].GetCost();
            Debug.Log("You bought: " + ShopItems[slotNumber].GetTitle());
        }

        public void Add(IShopItem item)
        {
            ShopItems.Add(item);
        }

        public void Add(List<IShopItem> items)
        {
            ShopItems.AddRange(items);
        }

        public void FillSlots()
        {
            for (int i = 0; i < ShopItems.Count && i < slots.Length; i++)
            {
                slots[i].Fill(ShopItems[i]);
            }
        }

        public void LoadSlots()
        {
            for (int i = 0; i < ShopItemSOs.Length && i < slots.Length; i++)
            {
                slots[i].Fill(ShopItemSOs[i]);
            }
        }

        private void LoadTestArrays()
        {
            foreach (var item in ShopItemSOs)
            {
                Add(item);
            }

            foreach (var item in EquipmentExampleSOs)
            {
                Add(item);
            }
        }
    }
}

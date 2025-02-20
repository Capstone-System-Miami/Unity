using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SystemMiami.Shop
{
    public class ShopManager : MonoBehaviour
    {
        /// <summary>
        /// TODO: this variable will eventually be removed, and the player's
        /// currency amount will be checked instead.
        /// </summary>
        public int coins;

        public TMP_Text coinUI;
        public ShopItemSlot[] slots;

        public ShopItemSO[] ShopItemSOs;
        public EquipmentExampleSO[] EquipmentExampleSOs;

        private List<IShopItem> ShopItems = new();

        void Start()
        {

            LoadTestArrays();

            for (int i = 0; i < ShopItems.Count; i++)
                slots[i].gameObject.SetActive(true);

            FillSlots();
        }

        void Update()
        {
            CheckPurchaseable();
            coinUI.text = "Coins: " + coins.ToString();
        }

        /// <summary>
        /// TODO: this method will eventually be removed, and the player's
        /// currency amount will be checked instead.
        /// </summary>
        public void AddCoins()
        {
            coins++;
        }

        public void CheckPurchaseable()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].gameObject.activeSelf == false)
                    continue;

                if (slots[i].ItemIsPurchaseable(coins))
                    slots[i].EnableButton();
                else
                    slots[i].DisableButton();
            }
        }

        public void PurchaseItem(int slotNumber)
        {
            coins = coins - ShopItems[slotNumber].GetCost();

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
            for (int i = 0; i < ShopItems.Count; i++)
            {
                slots[i].Fill(ShopItems[i]);
            }
        }

        /// <summary>
        /// A testing-only function to load test arrays into our
        /// list of IShopItems
        /// </summary>
        private void LoadTestArrays()
        {
            for(int i = 0; i < ShopItemSOs.Length; i++)
            {
                Add(ShopItemSOs[i]);
            }

            for (int i = 0; i < EquipmentExampleSOs.Length; i++)
            {
                Add(EquipmentExampleSOs[i]);
            }
        }
    }
}
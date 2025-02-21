using System;
using UnityEngine;
using System.Collections.Generic;

namespace SystemMiami.InventorySystem
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Inventory inventory;
        [SerializeField] private List<InventoryItemSlot> inventorySlots;

        private void OnEnable() 
        {
            inventory.OnInventoryChanged += RefreshUI;
        }

        private void Start()
        {
            RefreshUI();
        }

        public void RefreshUI()
        {
            //for now just add everything to one list and display them on one panel but TODO sort them out into different
            //panels and lists based on type
            
            List<int> allIDs = new List<int>();
            allIDs.AddRange(inventory.MagicalAbilityIDs);
            allIDs.AddRange(inventory.PhysicalAbilityIDs);
            allIDs.AddRange(inventory.ConsumableIDs);

            // clear all slots so they don't display old itemData.
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                inventorySlots[i].ClearSlot();
            }

            // Fill each slot with the corresponding item ID,
            //    until we run out of slots or IDs.
            int slotCount = inventorySlots.Count;
            int itemCount = allIDs.Count;
            int minCount = Mathf.Min(slotCount, itemCount);

            for (int i = 0; i < itemCount; i++)
            {
                int id = allIDs[i];
               // ItemData itemData = Database.Instance.GetData(id);
                inventorySlots[i].itemID = id;
                inventorySlots[i].UpdateSlot();
                
            }
        }

        private void OnDisable()
        {
            inventory.OnInventoryChanged -= RefreshUI;
        }
    }
}
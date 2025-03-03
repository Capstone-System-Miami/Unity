using System;
using UnityEngine;
using System.Collections.Generic;
using SystemMiami.Utilities;

namespace SystemMiami.InventorySystem
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private dbug log;

        [Header("References")]
        [SerializeField] private Inventory inventory;
        [SerializeField] private List<InventoryItemSlot> inventorySlots = new();

        private int SlotCount
        {
            get
            {
                return inventorySlots == null
                    ? 0
                    : inventorySlots.Count;
            }
        }

        private void OnEnable()
        {
            if (inventory == null)
            {
                log.error("eff"); return;
            }
            inventory.OnInventoryChanged += RefreshUI;
        }

        private void Start()
        {
            RefreshUI();
        }

        // TODO: Sort items out into different
        // panels and lists based on type.
        // For now just add everything to one list
        // and display them on one panel
        public void RefreshUI()
        {
            if (SlotCount == 0) { return; }

            ClearAllSlots();

            FillSlots(inventory.AllValidInventoryItems);
        }

        // Clear all slots so they don't display old itemData.
        private void ClearAllSlots()
        {
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                inventorySlots[i].ClearSlot();
            }
        }

        // Fill each slot with the corresponding item ID,
        // until we run out of slots or IDs.
        private void FillSlots(List<int> ids)
        {

            int minCount = Mathf.Min(SlotCount, ids.Count);

            for (int i = 0; i < minCount; i++)
            {
                // ItemData itemData = Database.Instance.GetData(id);
                if (!inventorySlots[i].TryFill(ids[i]))
                {
                    log.error(
                        $"{name} could not fill {inventorySlots[i]}. SKIPPING");
                    continue;
                }
            }
        }

        private void OnDisable()
        {
            inventory.OnInventoryChanged -= RefreshUI;
        }
    }
}
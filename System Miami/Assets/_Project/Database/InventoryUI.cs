using System;
using UnityEngine;
using System.Collections.Generic;
using SystemMiami.Utilities;
using SystemMiami.ui;

namespace SystemMiami.InventorySystem
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private dbug log;

        [Header("External References")]
        [SerializeField] private Inventory playerInventory;

        [Header("Internal References")]
        [SerializeField] private ItemGrid gridPhysical;
        [SerializeField] private ItemGrid gridMagical;
        [SerializeField] private ItemGrid gridConsumable;
        [SerializeField] private ItemGrid gridEquipment;

        [Header("Readonly")]
        [SerializeField, ReadOnly] private ItemGrid activeGrid;
        [SerializeField, ReadOnly] private List<InventoryItemSlot> inventorySlots = new();

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
            if (playerInventory == null)
            {
                log.error(
                    $"inventory was null during {name}'s {this}.OnEnable(), " +
                    $"so {name} did not subscribe to inventory's System.Action " +
                    $"OnInventoryChanged."); return;
            }
            playerInventory.OnInventoryChanged += RefreshUI;
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

            FillSlots(playerInventory.AllValidInventoryItems);
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
            playerInventory.OnInventoryChanged -= RefreshUI;
        }
    }
}
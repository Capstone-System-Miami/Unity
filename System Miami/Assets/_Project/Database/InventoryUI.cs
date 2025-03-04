using System;
using UnityEngine;
using System.Collections.Generic;
using SystemMiami.Utilities;
using SystemMiami.ui;
using System.Linq;
using UnityEngine.Tilemaps;
using UnityEngine.Assertions;

namespace SystemMiami.InventorySystem
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private dbug log;

        [Header("External References")]
        [SerializeField] private Inventory playerInventory;

        [Header("Internal References")]
        [SerializeField] private InventoryTab tabPhysical;
        [SerializeField] private InventoryTab tabMagical;
        [SerializeField] private InventoryTab tabConsumable;
        [SerializeField] private InventoryTab tabEquipment;

        [Header("Readonly")]
        [SerializeField, ReadOnly] private string activeTab;

        private List<InventoryTab> tabs = new();
        private SingleSelector<InventoryTab> tabSelector;

        private void Awake()
        {
            InitializeTabs();
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


            // TODO: Should this be reversed? I was sort of assuming the
            // inventory menu would be a place where the UI affects the
            // players inventory, and is safe from needing to update.
            // This way, we would just RefreshUI() on enable when the pause
            // menu comes up.  Then when the UI changes (player drags an item,
            // player trashes an item?, player moves an item to loadout)
            // an event from this script would fire?
            playerInventory.OnInventoryChanged += RefreshUI;
        }

        private void Start()
        {
            Assert.IsTrue(tabs != null);
            Assert.IsTrue(tabs.Count > 0);
            tabSelector = new(tabs);
            tabSelector.Reset();

            RefreshUI();
        }

        private void Update()
        {
            activeTab = tabSelector.CurrentSelection.ItemGrid.ItemType.ToString();
            foreach (InventoryTab tab in tabs)
            {
                log.warn($"checking {tab.ItemGrid.ItemType}");
                if (tab.Button.IsSelected)
                {
                    log.warn($"{tab.ItemGrid.ItemType} is Selected");
                    tabSelector.Select(tabs.IndexOf(tab), false);
                }
            }
        }

        // TODO: Sort items out into different
        // panels and lists based on type.
        // For now just add everything to one list
        // and display them on one panel
        public void RefreshUI()
        {
            tabPhysical.ItemGrid.ClearSlots();
            tabMagical.ItemGrid.ClearSlots();
            tabConsumable.ItemGrid.ClearSlots();
            tabEquipment.ItemGrid.ClearSlots();

            tabPhysical.ItemGrid.FillSlots(playerInventory.PhysicalAbilityIDs);
            tabMagical.ItemGrid.FillSlots(playerInventory.MagicalAbilityIDs);
            tabConsumable.ItemGrid.FillSlots(playerInventory.ConsumableIDs);
            //tabEquipment.ItemGrid.FillSlots(playerInventory.[fill this in whenever]);
            
        }

        private void InitializeTabs()
        {
            tabPhysical.Initialize(this, ItemType.PhysicalAbility);
            tabMagical.Initialize(this, ItemType.MagicalAbility);
            tabConsumable.Initialize(this, ItemType.Consumable);
            tabEquipment.Initialize(this, ItemType.EquipmentMod);

            tabs.Add(tabPhysical);
            tabs.Add(tabMagical);
            tabs.Add(tabConsumable);
            tabs.Add(tabEquipment);
        }

        private void OnDisable()
        {
            playerInventory.OnInventoryChanged -= RefreshUI;
        }
    }
}
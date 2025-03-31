using System.Collections.Generic;
using System.Linq;
using SystemMiami.ui;
using SystemMiami.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using SystemMiami.Management;
using System.ComponentModel;

namespace SystemMiami.InventorySystem
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private dbug log;

        [Header("External References")]
        [SerializeField] private Inventory playerInventory;

        [Header("Internal References")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private InventoryTabGroup tabs;

        
        [Header("Readonly")]
        [SerializeField, ReadOnly] private string activeTab;
        [SerializeField] public ItemGrid activeGrid
        {
            get
            {
                return activeGridInternal;
            }
            set
            {
                activeGridInternal = value;
                scrollRect.content = activeGrid.RT;
            }
        }

        //private SingleSelector<InventoryTab> tabSelector;
        //private List<InventoryTab> tabs = new();
        //private List<ISingleSelectable> selectableTabs = new();

        //private SingleSelector buttonSelector;
        //private List<SingleSelectButton> buttons = new();
        //private List<ISingleSelectable> selectableButtons = new();

        private ItemGrid activeGridInternal;

        public InventoryTabGroup Tabs { get { return tabs; } }

        public ScrollRect ScrollRect { get { return scrollRect; } }

        private void Awake()
        {
        }

        private void OnEnable()
        {
            if (!PlayerManager.MGR.TryGetComponent(out playerInventory))
            {
                string[] error = new string[]
                {
                    "InventoryUI couldn't find the Inventory component on Player"
                };
                UI.MGR.StartDialogue(this, true, true, false, "ERROR", error);
                log.error(
                    $"inventory was null during {name}'s {this}.OnEnable(), " +
                    $"so {name} did not subscribe to inventory's System.Action " +
                    $"OnInventoryChanged.");
                return;

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
            InitializeTabs();
            InitializeButtons();
            RefreshUI();
        }

        private void Update()
        {
            //// Set this to check in the inspector.
            //activeTab = $"{(tabSelector.CurrentSelection as InventoryTab).ValidItemType}";

            //// Loop through our buttons.
            //// If a button is selected and the matching tab is not,
            //// select the tab using the SingleSelector
            //for (int i = 0 ; i < buttons.Count; i++)
            //{
            //    if (buttons[i].IsSelected && !tabs[i].IsSelected)
            //    {
            //        log.print(
            //            $"{buttons[i].name} is Selected, " +
            //            $"and {tabs[i].ValidItemType} tab is not. Selecting...");

            //        tabSelector.Select(i);
            //    }
            //}
            //RefreshUI();
        }

        public void RefreshUI()
        {
            Tabs.TabPhysical.ItemGrid.ClearSlots();
            Tabs.TabMagical.ItemGrid.ClearSlots();
            Tabs.TabConsumable.ItemGrid.ClearSlots();
            Tabs.TabEquipment.ItemGrid.ClearSlots();

            Tabs.TabPhysical.ItemGrid.FillSlots(playerInventory.PhysicalAbilityIDs);
            Tabs.TabMagical.ItemGrid.FillSlots(playerInventory.MagicalAbilityIDs);
            Tabs.TabConsumable.ItemGrid.FillSlots(playerInventory.ConsumableIDs);

            /// TODO:
            //Tabs.TabEquipment.ItemGrid.FillSlots(playerInventory.[fill this in whenever]);
        }

        private void InitializeTabs()
        {
            // Initialize tabs set in the inspector with the infor they need


            //// Create a new list of tabs
            //tabs = new List<InventoryTab>()
            //{
            //    tabPhysical,
            //    tabMagical,
            //    tabConsumable,
            //    tabEquipment
            //};

            //Assert.IsTrue(tabs != null);
            //Assert.IsTrue(tabs.Count > 0);

            //// Cast list
            //selectableTabs = tabs.Cast<ISingleSelectable>().ToList();

            //Assert.IsNotNull(selectableTabs);
            //Assert.IsTrue(selectableTabs.Any());

            //// Create a new SingleSelector to manage tab selection
            //tabSelector = new SingleSelector(selectableTabs);
            //tabSelector.Reset();
        }

        private void InitializeButtons()
        {
            //Assert.IsNotNull(selectableTabs);
            //Assert.IsTrue(selectableTabs.Any());

            //// Get buttons from tab objs
            //buttons = tabs.Select(tab => tab.Button).ToList();

            //// Cast list
            //selectableButtons = buttons.Cast<ISingleSelectable>().ToList();

            //Assert.IsNotNull(selectableButtons);
            //Assert.IsTrue(selectableButtons.Any());

            //// Create a new SingleSelector to handle button selection
            //buttonSelector = new SingleSelector(selectableButtons);
            //buttonSelector.Reset();
        }

        private void OnDisable()
        {
            playerInventory.OnInventoryChanged -= RefreshUI;
        }
    }
}
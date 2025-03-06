using System.Collections.Generic;
using System.Linq;
using SystemMiami.ui;
using SystemMiami.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace SystemMiami.InventorySystem
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private dbug log;

        [Header("External References")]
        [SerializeField] private Inventory playerInventory;

        [Header("Internal References")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private InventoryTab tabPhysical;
        [SerializeField] private InventoryTab tabMagical;
        [SerializeField] private InventoryTab tabConsumable;
        [SerializeField] private InventoryTab tabEquipment;

        public InventoryTab TabPhysical { get {return tabPhysical;} }
        public InventoryTab TabMagical  { get {return tabMagical;} }
        public InventoryTab TabConsumable  { get {return tabConsumable;} }
        public InventoryTab TabEquipment  { get {return tabEquipment;} }
        
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

        private SingleSelector tabSelector;
        private List<InventoryTab> tabs = new();
        private List<ISingleSelectable> selectableTabs = new();

        private SingleSelector buttonSelector;
        private List<SingleSelectButton> buttons = new();
        private List<ISingleSelectable> selectableButtons = new();

        private ItemGrid activeGridInternal;

        public ScrollRect ScrollRect { get { return scrollRect; } }

        private void Awake()
        {
            InitializeTabs();
            InitializeButtons();
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
            RefreshUI();
        }

        private void Update()
        {
            // Set this to check in the inspector.
            activeTab = $"{(tabSelector.CurrentSelection as InventoryTab).ValidItemType}";

            // Loop through our buttons.
            // If a button is selected and the matching tab is not,
            // select the tab using the SingleSelector
            for (int i = 0 ; i < buttons.Count; i++)
            {
                if (buttons[i].IsSelected && !tabs[i].IsSelected)
                {
                    log.print(
                        $"{buttons[i].name} is Selected, " +
                        $"and {tabs[i].ValidItemType} tab is not. Selecting...");

                    tabSelector.Select(i);
                }
            }
        }

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
            // Initialize tabs set in the inspector with the infor they need
            tabPhysical.Initialize(this, ItemType.PhysicalAbility);
            tabMagical.Initialize(this, ItemType.MagicalAbility);
            tabConsumable.Initialize(this, ItemType.Consumable);
            tabEquipment.Initialize(this, ItemType.EquipmentMod);

            // Create a new list of tabs
            tabs = new List<InventoryTab>()
            {
                tabPhysical,
                tabMagical,
                tabConsumable,
                tabEquipment
            };

            Assert.IsTrue(tabs != null);
            Assert.IsTrue(tabs.Count > 0);

            // Cast list
            selectableTabs = tabs.Cast<ISingleSelectable>().ToList();

            Assert.IsNotNull(selectableTabs);
            Assert.IsTrue(selectableTabs.Any());

            // Create a new SingleSelector to manage tab selection
            tabSelector = new SingleSelector(selectableTabs);
            tabSelector.Reset();
        }

        private void InitializeButtons()
        {
            Assert.IsNotNull(selectableTabs);
            Assert.IsTrue(selectableTabs.Any());

            // Get buttons from tab objs
            buttons = tabs.Select(tab => tab.Button).ToList();

            // Cast list
            selectableButtons = buttons.Cast<ISingleSelectable>().ToList();

            Assert.IsNotNull(selectableButtons);
            Assert.IsTrue(selectableButtons.Any());

            // Create a new SingleSelector to handle button selection
            buttonSelector = new SingleSelector(selectableButtons);
            buttonSelector.Reset();
        }

        private void OnDisable()
        {
            playerInventory.OnInventoryChanged -= RefreshUI;
        }
    }
}
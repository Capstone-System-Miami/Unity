using SystemMiami.ui;
using SystemMiami.Utilities;
using UnityEngine;
using UnityEngine.UI;
using SystemMiami.Management;

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

        private ItemGrid activeGridInternal;

        public InventoryTabGroup Tabs { get { return tabs; } }

        public ScrollRect ScrollRect { get { return scrollRect; } }

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
            //
            playerInventory.OnInventoryChanged += RefreshUI;
        }

        private void Start()
        {
            RefreshUI();
        }

        private void Update()
        {
            // RefreshUI();
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
            Tabs.TabEquipment.ItemGrid.FillSlots(playerInventory.EquipmentModIDs);
        }

        private void OnDisable()
        {
            playerInventory.OnInventoryChanged -= RefreshUI;
        }
    }
}

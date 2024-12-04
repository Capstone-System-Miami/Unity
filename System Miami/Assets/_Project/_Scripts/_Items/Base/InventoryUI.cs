// Authors: Lee
using UnityEngine;

using SystemMiami.CombatSystem;
using Unity.VisualScripting;

namespace SystemMiami
{
    /// <summary>
    /// Manages the inventory UI and updates it based on the player's inventory.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] public GameObject inventoryUIPanel;    // The panel containing the inventory UI
        [SerializeField] private Transform itemsParent;          // The parent transform for inventory slots
        [SerializeField] private InventorySlotUI slotPrefab;     // Prefab for the inventory slot UI

        private Inventory _inventory;                            // Reference to the player's inventory
        private InventorySlotUI[] _slots;                        // Array of slot UI instances

        private void Start()
        {
            // Get the player's Combatant component and inventory
            Combatant playerCombatant = FindObjectOfType<PlayerController>().GetComponent<Combatant>();
            _inventory = playerCombatant.Inventory;

            // Create UI slots based on the inventory
            CreateInventorySlots();

            // Subscribe to the inventory changed event
            _inventory.InventoryChanged += UpdateUI;

            // Initial UI update
            UpdateUI();

            // Optionally hide the inventory UI at the start
            //if (inventoryUIPanel != null)
            //{
            //    inventoryUIPanel.SetActive(false);
            //}
        }

        /// <summary>
        /// Creates the inventory slot UI elements based on the player's inventory size.
        /// </summary>
        private void CreateInventorySlots()
        {
            int slotCount = _inventory.maxSlots;
            _slots = new InventorySlotUI[slotCount];
            Debug.Log("CreateInventorySlotws called");
            for (int i = 0; i < slotCount - 1; i++)
            {
                // Instantiate the slot UI prefab
                Vector3 newPos = new Vector3(itemsParent.position.x, itemsParent.position.y + 105 * i, 0);
                InventorySlotUI slotUI = Instantiate(slotPrefab,itemsParent);
                slotUI.transform.position = newPos;
                _slots[i] = slotUI;
                slotUI.slotIndex = i;

                // Assign the corresponding inventory slot
                if (i <= _inventory._inventorySlots.Count)
                {
                    slotUI.AddItem(_inventory._inventorySlots[i]);
                    Debug.Log($"{_inventory._inventorySlots[i]} added");
                }
                else
                {
                    slotUI.ClearSlot();
                }
            }
        }

        /// <summary>
        /// Updates the inventory UI to reflect the current state of the player's inventory.
        /// </summary>
        private void UpdateUI()
        {
            Debug.Log("InventoryUI: UpdateUI called");
            //for (int i = 0; i < _slots.Length; i++)
            //{
            //    if (i < _inventory._inventorySlots.Count)
            //    {
            //        InventorySlot slot = _inventory._inventorySlots[i];

            //        if (!slot.IsEmpty)
            //        {
            //            _slots[i].AddItem(slot);
            //        }
            //        else
            //        {
            //            _slots[i].ClearSlot();
            //        }
            //    }
            //    else
            //    {
            //        _slots[i].ClearSlot();
            //    }
            //}
            //Debug.Log("InventoryUI: UpdateUI completed");
        }

        /// <summary>
        /// Toggles the visibility of the inventory UI panel.
        /// </summary>
        public void ToggleInventoryUI()
        {
            if (inventoryUIPanel != null)
            {
                inventoryUIPanel.SetActive(!inventoryUIPanel.activeSelf);
            }
        }
    }
}

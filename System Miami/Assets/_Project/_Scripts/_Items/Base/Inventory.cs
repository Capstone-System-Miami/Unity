//Author: Lee
using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami
{
    public class Inventory : MonoBehaviour
    {
        [Header("Inventory Settings")] 
        public int maxSlots;

        [SerializeField] public List<InventorySlot> _inventorySlots = new List<InventorySlot>();

        //event to notify when the inventory changes
        public delegate void OnInventoryChanged();

        public event OnInventoryChanged InventoryChanged;
        // Property to access inventory slots
        
        private void Start()
        {
            //for (int i = 0; i < maxSlots; i++)
            //{
            //    _inventorySlots.Add(new InventorySlot());
            //}
        }

        /// <summary>
        /// Adds an item to the inventory.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="quantity">The quantity of the item.</param>
        /// <returns>True if the item was added successfully.</returns>
        public bool AddItem(Item item, int quantity = 1)
        {
            //check if item is stackable and already exists in inventory
            if (item.isStackable)
            {
                //find if a slot in the inventory already has that item
                InventorySlot slot = _inventorySlots.Find(s => s.Item == item);

                if (slot != null)
                {
                    //if there is an existing slot with the item add the quantity
                    slot.AddQuantity(quantity);
                    InventoryChanged?.Invoke();
                    return true;
                }
            }

            //if item isnt stackable find an empty slot
            InventorySlot emptySlot = _inventorySlots.Find(s => s.IsEmpty);
            if (emptySlot != null)
            {
                //add item to empty slot
                emptySlot.AddItem(item,quantity);
                
                InventoryChanged?.Invoke();
                return true;
            }
            // Inventory is full
            Debug.Log("Inventory is full!");
            return false;
        }

        /// <summary>
        /// Removes an item from the inventory.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <param name="quantity">The quantity to remove.</param>
        public void RemoveItem(Item item, int quantity = 1)
        {
            InventorySlot slot = _inventorySlots.Find(s => s.Item == item);
            if (slot != null)
            {
                // Reduce 
                slot.RemoveQuantity(quantity);
                if (slot.Quantity <= 0)
                {
                    // Clear the slot no items 
                    slot.ClearSlot();
                }
                InventoryChanged?.Invoke();
            }
        }

        /// <summary>
        /// Uses an item from a specific slot.
        /// </summary>
        /// <param name="slotIndex">The index of the slot.</param>
        /// <param name="combatant">The combatant using the item.</param>
        public void UseItem(int slotIndex, Combatant combatant)
        {
            if (slotIndex < 0 || slotIndex >= _inventorySlots.Count)
                { return; }
            InventorySlot slot = _inventorySlots[slotIndex];

            if (!slot.IsEmpty)
            {
                slot.Item.Use(combatant);
                //reduce
                RemoveItem(slot.Item, 1);
                
            }


            

        }

      

    }
}

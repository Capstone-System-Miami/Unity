//Author: Lee
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    /// <summary>
    /// Represents a single slot in the inventory.
    /// </summary>
    [System.Serializable]
    public class InventorySlot 
    {
        /// <summary>
        /// The item held in this inventory slot.
        /// </summary>
        public Item Item;

        /// <summary>
        /// The quantity of the item in this slot.
        /// </summary>
        public int Quantity;

        /// <summary>
        /// Indicates whether the slot is empty.
        /// </summary>
        public bool IsEmpty => Item == null;

        public void AddItem(Item item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }

        /// <summary>
        /// Increases the quantity of the item in the slot.
        /// </summary>
        /// <param name="quantity">The amount to increase the quantity by.</param>
        public void AddQuantity(int quantity)
        {
            Quantity += quantity;
        }

        /// <summary>
        /// Decreases the quantity of the item in the slot.
        /// </summary>
        /// <param name="quantity">The amount to decrease the quantity by.</param>
        public void RemoveQuantity(int quantity)
        {
            Quantity -= quantity;
            if (Quantity <= 0)
            {
                ClearSlot();
            }
        }

        /// <summary>
        /// Clears the slot, removing the item and resetting the quantity.
        /// </summary>
        public void ClearSlot()
        {
            Item = null;
            Quantity = 0;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    public class InventoryChangedEventArgs : EventArgs
    {
        public enum Operation { add, remove };

        public readonly ItemType itemType;
        public readonly List<int> itemsRemoved;
        public readonly List<int> itemsAdded;

        public InventoryChangedEventArgs(ItemType type, List<int> items, Operation operation)
        {
            this.itemType = type;

            itemsAdded = operation == Operation.add ? items : new();
            itemsRemoved = operation == Operation.remove ? items : new();
        }

        public InventoryChangedEventArgs(
            ItemType itemType,
            List<int> itemsAdded = null,
            List<int> itemsRemoved = null)
        {
            this.itemType = itemType;

            itemsAdded ??= new();
            itemsRemoved ??= new();

            this.itemsAdded = itemsAdded;
            this.itemsRemoved = itemsRemoved;
        }
    }
}

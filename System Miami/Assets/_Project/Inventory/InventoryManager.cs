using System.Collections.Generic;
using SystemMiami.Management;
using UnityEngine;

namespace SystemMiami.LeeInventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        public InventorySlot[] slots;

        protected override void Awake()
        {
            base.Awake();
        }

        public void Add(OutdatedOrDuplicates.Item item)
        {
            foreach (InventorySlot slot in slots)
            {
                if (slot.item == null)
                {
                    slot.AddItem(item);
                    return;
                }
            }

            Debug.Log("Inventory full!");
        }
    }
}
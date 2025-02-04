using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami.LeeInventory
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager instance;

        public InventorySlot[] slots;

        private void Awake()
        {
            instance = this;
        }

        public void Add(wtf.Item item)
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
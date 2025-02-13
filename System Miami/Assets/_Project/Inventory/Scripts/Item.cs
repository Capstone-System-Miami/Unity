using System;
using UnityEngine;

namespace SystemMiami.LeeInventory
{
    [Serializable]
    public class Item
    {
        public OutdatedOrDuplicates.ItemData itemData;
        public int stackSize;
    
        public Item(OutdatedOrDuplicates.ItemData item)
        {
            itemData = item;
            AddToStack();
        }
    
        public void AddToStack()
        {
            stackSize++;
        }
    
        public void RemoveFromStack()
        {
            stackSize--;
        }
    }
}

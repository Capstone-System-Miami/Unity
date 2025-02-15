using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    public class InventoryItem
    {
        public int id;
        public DataType type;
        public int quantity; 
        public bool stackable;
    }
}

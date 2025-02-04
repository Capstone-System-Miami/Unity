using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami.LeeInventory
{
    public class InventorySlot : MonoBehaviour
    {
        public Image icon; // Icon for the item
        public Button removeButton; // Optional: Button to remove the item
    
        public wtf.Item item { get; private set; } // The item currently in this slot
    
        public void AddItem(wtf.Item newItem)
        {
            item = newItem;
            icon.sprite = item.icon;
            icon.enabled = true;
        }
    
        public void ClearSlot()
        {
            item = null;
            icon.sprite = null;
            icon.enabled = false;
        }
    
        public void UseItem()
        {
            if (item != null)
            {
                item.Use();
            }
        }
    }
}

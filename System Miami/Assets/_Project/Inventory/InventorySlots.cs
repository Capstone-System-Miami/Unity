using UnityEngine;

namespace SystemMiami.LeeInventory
{
    public class InventorySlot : MonoBehaviour
    {
        public OutdatedOrDuplicates.Item item; // Reference to the item in the slot

        public void AddItem(OutdatedOrDuplicates.Item newItem)
        {
            item = newItem; // Assign the new item to the slot
            // Update the visual representation, e.g., set an icon
            GetComponent<UnityEngine.UI.Image>().sprite = newItem.icon;
            GetComponent<UnityEngine.UI.Image>().color = Color.white; // Make the slot visible
        }

        public void ClearSlot()
        {
            item = null; // Remove the item from the slot
            GetComponent<UnityEngine.UI.Image>().sprite = null;
            GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 0); // Hide the slot
        }
    }
}

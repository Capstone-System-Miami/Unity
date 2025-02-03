using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public InventorySlot[] slots;

    private void Awake()
    {
        instance = this;
    }

    public void Add(Item item)
    {
        foreach (var slot in slots)
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

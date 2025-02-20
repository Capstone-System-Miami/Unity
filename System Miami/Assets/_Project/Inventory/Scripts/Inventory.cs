using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SystemMiami.Management;
using SystemMiami.LeeInventory;

namespace SystemMiami
{
    public class Inventory : Singleton<Inventory>
    {
        //public List<Item> quickslot = new List<Item>();
        //private Dictionary<ItemData, Item> quickslotDictionary = new Dictionary<ItemData, Item>();

        //private int maxInventorySlots = 8;
        //public List<Item> fullInventory = new List<Item>();
        //private Dictionary<ItemData, Item> inventoryDictionary = new Dictionary<ItemData, Item>();

        //public GameObject[] allInventories;

        //private void Start()
        //{
        //    //allInventories = FindObjectsByType<InventoryButtonUI>(FindObjectsSortMode.InstanceID);
        //}
        //public void Add(ItemData itemData)
        //{
        //    if (quickslot.Count < maxInventorySlots)
        //    {
        //        if (quickslotDictionary.TryGetValue(itemData, out Item item))
        //        {
        //            item.AddToStack();
        //        }
        //        else
        //        {
        //            Item newItem = new Item(itemData);
        //            quickslot.Add(newItem);
        //            quickslotDictionary.Add(itemData, newItem);
        //        }
        //    }
        //    else if (quickslot.Count >= maxInventorySlots)
        //    {
        //        if (inventoryDictionary.TryGetValue(itemData, out Item item))
        //        {
        //            item.AddToStack();
        //        }
        //        else
        //        {
        //            Item newItem = new Item(itemData);
        //            fullInventory.Add(newItem);
        //            inventoryDictionary.Add(itemData, newItem);
        //        }
        //    }

        //    foreach (GameObject button in allInventories)
        //    {
        //        if (button.GetComponent<InventoryButtonUI>() != null)
        //        {
        //            button.GetComponent<InventoryButtonUI>().UpdateInventory();
        //        }
        //        else
        //        {
        //            button.GetComponent<QuickslotButtonUI>().UpdateQuickslotInventory();
        //        }
        //    }

        //}

        //public void Remove(ItemData itemData)
        //{
        //    if (quickslotDictionary.TryGetValue(itemData, out Item item))
        //    {
        //        item.RemoveFromStack();
        //        if (item.stackSize == 0)
        //        {
        //            quickslot.Remove(item);
        //            quickslotDictionary.Remove(itemData);

        //        }
        //    }
        //    if (inventoryDictionary.TryGetValue(itemData, out Item chestItem))
        //    {
        //        chestItem.RemoveFromStack();
        //        if (chestItem.stackSize == 0)
        //        {
        //            fullInventory.Remove(item);
        //            inventoryDictionary.Remove(itemData);

        //        }
        //    }
        //    foreach (GameObject button in allInventories)
        //    {
        //        if (button.GetComponent<InventoryButtonUI>() != null)
        //        {
        //            button.GetComponent<InventoryButtonUI>().UpdateInventory();
        //        }
        //        else if (button.GetComponent<QuickslotButtonUI>() != null)
        //        {
        //            button.GetComponent<QuickslotButtonUI>().UpdateQuickslotInventory();
        //        }
        //    }

        //}
    }
}
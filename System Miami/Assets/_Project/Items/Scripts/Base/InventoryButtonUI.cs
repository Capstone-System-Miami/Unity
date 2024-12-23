using SystemMiami.CombatSystem;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButtonUI : MonoBehaviour
{
    public Item item;
    public int index;
    public Image Icon;
    [SerializeField] Combatant player;
    private void Start()
    {
        UpdateInventory();
        int ID = gameObject.GetInstanceID();
        Debug.LogWarning($"{ID}");
    }

    public void UpdateInventory()
    {
        if (Inventory.instance.quickslot[index].itemData != null)
        {
            item = Inventory.instance.quickslot[index];
            Icon.sprite = item.itemData.icon;
            item.itemData.Init(player);
        }
        Debug.Log("Update Inventory was called");

    }

    public void Clicked()
    {
        item.itemData.Use();
        Debug.Log("Item Clicked");
    }
}



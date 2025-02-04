using SystemMiami;
using SystemMiami.CombatSystem;
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami.LeeInventory
{
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
            if (Inventory.MGR.quickslot[index].itemData != null)
            {
                item = Inventory.MGR.quickslot[index];
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
}
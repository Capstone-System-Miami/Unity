using SystemMiami.CombatSystem;
using UnityEngine;
using UnityEngine.UI;

    public class QuickslotButtonUI : MonoBehaviour
    {
        public Item item;
        public int index;
        public Image Icon;
        [SerializeField] Combatant player;
        public bool locked;
        private void Start()
        {
            UpdateQuickslotInventory();
            int ID = gameObject.GetInstanceID();
            Debug.LogWarning($"{ID}");
           locked = false;
        }

        public void UpdateQuickslotInventory()
        {
            if (locked) return;
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



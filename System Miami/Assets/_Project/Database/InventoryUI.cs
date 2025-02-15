using UnityEngine;
using System.Collections.Generic;

namespace SystemMiami
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private Inventory playerInventory;
        [SerializeField] private InventoryItemButton itemButtonPrefab;
        [SerializeField] private Transform contentParent; // A layout group or similar

        private readonly List<InventoryItemButton> spawnedButtons = new();

        private void Start()
        {
            PopulateInventory();
        }

        /// <summary>
        /// Builds the inventory UI from the player inventory IDs.
        /// </summary>
        public void PopulateInventory()
        {
            // Clear old buttons
            foreach (var btn in spawnedButtons)
            {
                Destroy(btn.gameObject);
            }
            spawnedButtons.Clear();

            // Create UI buttons for Abilities
            foreach (int abilityID in playerInventory.AbilityIDs)
            {
                var newButton = Instantiate(itemButtonPrefab, contentParent);
                newButton.SetItem(abilityID, DataType.Ability);
                spawnedButtons.Add(newButton);
            }

            // Create UI buttons for Consumables
            foreach (int consumableID in playerInventory.ConsumableIDs)
            {
                var newButton = Instantiate(itemButtonPrefab, contentParent);
                newButton.SetItem(consumableID, DataType.Consumable);
                spawnedButtons.Add(newButton);
            }
        }
    }
}
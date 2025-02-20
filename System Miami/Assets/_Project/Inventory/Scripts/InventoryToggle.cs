using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    public class InventoryToggle : MonoBehaviour
    {
        public GameObject inventoryUI; // Assign your inventory UI GameObject in the Inspector
        private bool isOpen = false;

        void Start()
        {
            inventoryUI.SetActive(false); // Ensure the inventory starts hidden
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleInventory();
            }
        }

        void ToggleInventory()
        {
            isOpen = !isOpen;
            inventoryUI.SetActive(isOpen);
        }
    }

}
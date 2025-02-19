using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami
{
    public class InventoryItemSlot : MonoBehaviour
    {
        public int itemID = 0;
        [SerializeField] ItemType itemType;
        
        public Image icon;
        public Text name;
        public Text description;

        private void Awake()
        {
            // Automatically assigning the references based on child hierarchy
            icon = GetComponentInChildren<Image>();
            var textComponents = GetComponentsInChildren<Text>();
            if (textComponents.Length >= 2)
            {
                name = textComponents[0]; // Assume the first Text is for name
                description = textComponents[1]; // Assume the second Text is for description
                // Adjust order above based on actual hierarchy
            }
            else
            {
                Debug.LogError("Not enough Text components found in InventoryItemSlot.");
            }
        }


        public void UpdateSlot() 
        {
            ItemData itemData = Database.MGR.GetRandomDataOfType(itemType);
            icon.sprite = itemData.Icon;
            name.text = itemData.Name;
            description.text = itemData.Description;
            
        }
        
        public void ClearSlot()
        {
            itemID = -1;
            icon.sprite = null;
            name.text = "";
            description.text = "";
        }
    }
}

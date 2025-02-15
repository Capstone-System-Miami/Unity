using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SystemMiami
{
    public class InventoryItemButton : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI itemNameText;
       

        private int itemID;
        private DataType dataType;

        /// <summary>
        /// Called by InventoryUI to initialize button with ID.
        /// </summary>
        public void SetItem(int id, DataType type)
        {
            itemID = id;
            dataType = type;

            // Get the data from the Database
            Data data = Database.Instance.GetData(itemID, dataType);
            if (!data.Equals(default))
            {
                iconImage.sprite = data.Icon;
                itemNameText.text = data.AbilityName;
                
            }
            else
            {
                // if invalid
                iconImage.sprite = null;
                itemNameText.text = "Invalid Item";
            }
        }

        /// <summary>
        /// Called from the button OnClick() event to put this item into a quickslot //temp need to change to double click or click and drag
        /// </summary>
        public void OnClick()
        {
            
            var quickSlotUI = FindObjectOfType<QuickSlotUI>();
            if (quickSlotUI != null)
            {
                quickSlotUI.EquipItemToSlot(itemID);
            }
            else
            {
                Debug.LogWarning("No QuickSlotUI found");
            }
        }
    }
}
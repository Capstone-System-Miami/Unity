using SystemMiami.InventorySystem;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SystemMiami.Shop
{
    public class ShopItemSlot : MonoBehaviour
    {
        public TMP_Text titleTxt;
        public TMP_Text descriptionTxt;
        public TMP_Text costTxt;
        public Button button;
        public Inventory playerInventory;
        private ItemData item;

        private void Start()
        {
            button.onClick.AddListener(addItemToInventory);
            playerInventory = PlayerManager.MGR.GetComponent<Inventory>();
        }
        public void Fill(ItemData item)
        {
            
            this.item = item;
            titleTxt.text = item.Name;
            descriptionTxt.text = item.Description;
            costTxt.text = item.Price.ToString();
        }

        public bool ItemIsPurchaseable(int playerCurrency)
        {
            return playerCurrency >= item.Price;
        }

        public void EnableButton()
        {
            button.interactable = true;
        }

        public void addItemToInventory()
        {
            if (PlayerManager.MGR.CurrentCredits >= item.Price)
            {
                playerInventory.AddToInventory(item.ID);
            }
        }

        public void DisableButton()
        {
            button.interactable = false;
        }
    }
}

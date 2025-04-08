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
        public Image itemImage;
        public Button buyButton;
        public Button seeItemButton;
        public Inventory playerInventory;
        private ItemData item;

        private void Start()
        {
            
            seeItemButton.onClick.AddListener(ShowItemInfo);
            playerInventory = PlayerManager.MGR.GetComponent<Inventory>();
            
        }
        public void Fill(ItemData item)
        {
            
            this.item = item;
            itemImage.sprite = item.Icon;

        }

        public void ShowItemInfo()
        {
            titleTxt.text = item.Name;
            descriptionTxt.text = item.Description;
            costTxt.text = item.Price.ToString();
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => addItemToInventory());
        }

        public bool ItemIsPurchaseable(int playerCurrency)
        {
            return playerCurrency >= item.Price;
        }

        public void EnableButton()
        {
            buyButton.interactable = true;
        }

        public void addItemToInventory()
        {
            if (PlayerManager.MGR.CurrentCredits >= item.Price)
            {
                playerInventory.AddToInventory(item.ID);
                Debug.Log("Player has bought " + item.Name);
            }
        }

        public void DisableButton()
        {
            buyButton.interactable = false;
        }
    }
}

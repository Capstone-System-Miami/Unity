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

        private IShopItem item;

        public void Fill(IShopItem item)
        {
            this.item = item;
            titleTxt.text = item.GetTitle();
            descriptionTxt.text = item.GetDescription();
            costTxt.text = item.GetCost().ToString();
        }

        public bool ItemIsPurchaseable(int playerCurrency)
        {
            if (item == null)
            {
                Debug.LogWarning("Item is null.");
                return false;
            }

            return playerCurrency >= item.GetCost();
        }

        public void EnableButton()
        {
            button.interactable = true;
        }

        public void DisableButton()
        {
            button.interactable = false;
        }
    }
}

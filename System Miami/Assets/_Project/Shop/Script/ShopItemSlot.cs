using System.Collections;
using System.Collections.Generic;
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
            return playerCurrency >= item.GetCost();
        }

        /// <summary>
        /// TODO: Maybe destroys the item, moves the item, etc.
        /// </summary>
        public void PurchaseItem()
        {

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
using SystemMiami.Management;
using SystemMiami.ui;
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami
{
    public class PopUpHandler : Singleton<PopUpHandler>
    {
        public RectTransform Popup;
        public Text ItemName;
        public Text DescriptionText;

        public ItemData CurrentItemData;
        public Vector2 offset;

        // UNITY
        private void Start()
        {
            Popup.SetAsLastSibling();
            Popup.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Popup.gameObject.activeSelf)
            {
                SetPopupPosition();
            }
        }

        // PUBLIC
        public void OpenPopup(ItemData itemData, ActionQuickslot slot)
        {
            OpenPopup(itemData, slot.RT);
        }

        public void OpenPopup(ItemData itemData, InventoryItemSlot slot)
        {
            OpenPopup(itemData, slot.RT);
        }

        public void OpenPopup(ItemData itemData, RectTransform rt)
        {
            CurrentItemData = itemData;

            // Turn on the popup
            Popup.gameObject.SetActive(true);

            // Set parent to the slot
            Popup.SetParent(rt, false);

            // Set position to the position of the slot plus an offset of 1.5
            // times the height of the slot.
            // TODO: check if this needs a specific anchor config
            // (either the popup, the parent rectTransform, or both)
            Popup.anchoredPosition = Vector2.zero + new Vector2(0, rt.sizeDelta.y * 1.5f);

            // Set the text in the popup
            BindText();
        }

        public void ClosePopup()
        {
            // Set parent to this manager
            Popup.SetParent(transform, false);

            // Turn off the popup
            Popup.gameObject.SetActive(false);
        }

        // PRIVATE
        private void SetPopupPosition()
        {
            //Vector2 Mouseposition = Input.mousePosition;
            //Popup.position = Mouseposition /*+ offset */+ (new Vector2(Popup.sizeDelta.x, Popup.sizeDelta.y) / 2f);
        }

        private void BindText()
        {
            DescriptionText.text = CurrentItemData.Description;
            ItemName.text = CurrentItemData.Name;
        }

    }
}

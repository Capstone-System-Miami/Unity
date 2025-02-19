using SystemMiami.Management;
using SystemMiami.ui;
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami
{
    public class PopUpHandler : Singleton<PopUpHandler>
    {
        public RectTransform Popup;
        public ItemData AssignedCombatAction;
        public Text DescriptionText;
        public Vector2 offset;
        public Text ItemName;

       // private bool OnShown => Popup.gameObject.activeSelf;

        private void Update()
        {
            if (Popup.gameObject.activeSelf)
            {
                SetPopupPosition();
            }
        }

        public void SetPopupAblility(ItemData itemData, ActionQuickslot slot)
        {
            AssignedCombatAction = itemData;

            // Turn on the popup
            Popup.gameObject.SetActive(true);

            // Set parent to the slot
            Popup.SetParent(slot.transform, false);

            // Set position to the position of the slot plus an offset of 1.5
            // times the height of the slot.
            Popup.anchoredPosition = Vector2.zero + new Vector2(0, slot.RT.sizeDelta.y * 1.5f);

            // Set the text in the popup
            BindText();
        }


        public void SetPopupAblility()
        {
            // Set parent to this manager
            Popup.SetParent(transform, false);

            // Turn off the popup
            Popup.gameObject.SetActive(false);
        }

        private void SetPopupPosition()
        {
            //Vector2 Mouseposition = Input.mousePosition;
            //Popup.position = Mouseposition /*+ offset */+ (new Vector2(Popup.sizeDelta.x, Popup.sizeDelta.y) / 2f);
        }

        private void Start()
        {
            Popup.SetAsLastSibling();
            Popup.gameObject.SetActive(false);
        }

        private void BindText()
        {
            DescriptionText.text = AssignedCombatAction.Description;
            ItemName.text = AssignedCombatAction.Name;
        }

    }
}

using SystemMiami.Management;
using SystemMiami.Utilities;
using UnityEngine;
using UnityEngine.UI;
using SystemMiami.CombatRefactor;

namespace SystemMiami.ui
{
    public class PopUpHandler : Singleton<PopUpHandler>
    {
        [SerializeField] private dbug log;

        public RectTransform Popup;
        public Text ItemName;
        public Text DescriptionText;
        public Text CooldownIndicator;
        [TextArea] public string enabledStr;
        [TextArea] public string cooldownStr;

        public ItemData CurrentItemData;
        public Vector2 offset;

        private Canvas currentCanvas;

        // UNITY
        private void Start()
        {
            Popup.SetAsLastSibling();
            Popup.gameObject.SetActive(false);
            SetCurrentCanvas();
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
            OpenPopup(itemData, slot.RT, true);
        }

        public void OpenPopup(CombatAction combatAction, ActionQuickslot slot)
        {
            ItemData itemData = Database.MGR.GetDataWithJustID(combatAction.ID);

            if (combatAction is NewAbility ability)
            {
                CooldownIndicator.text = ability.IsOnCooldown
                    ? cooldownStr.Replace("<>", ability.CooldownRemaining.ToString())
                    : enabledStr;
            }

            OpenPopup(itemData, slot.RT, true);
        }

        public void OpenPopup(ItemData itemData, InventoryItemSlot slot)
        {
            OpenPopup(itemData, slot.RT, false);
        }

        public void OpenPopup(ItemData itemData, RectTransform rt, bool parentToSlot)
        {
            CurrentItemData = itemData;

            // Turn on the popup
            Popup.gameObject.SetActive(true);

            if (parentToSlot)
            {
                // Set parent to the slot
                Popup.SetParent(rt, false);

                // Set position to the position of the slot plus an offset of 1.5
                // times the height of the slot.
                // TODO: check if this needs a specific anchor config
                // (either the popup, the parent rectTransform, or both)
                // TODO: if it does, try to handle the math for every case.
                Popup.anchoredPosition = Vector2.zero + new Vector2(0, rt.sizeDelta.y * 1.5f);
            }
            else
            {
                Vector2 slotposScreen = (Vector2)Camera.main.WorldToScreenPoint(rt.position);

                Canvas slotCanvas;
                TryGetCanvasInParents(rt, out slotCanvas);

                // TODO: This is really bad I have no idea why 50 works and I'm a little scared about it
                Vector2 myPosScreen = new(slotposScreen.x, slotposScreen.y + (rt.rect.height * slotCanvas.scaleFactor) * 50);

                Popup.position = (Vector2)Camera.main.ScreenToWorldPoint(myPosScreen);
                    //(Vector2)Camera.main.ScreenToWorldPoint(
                    //    slotposScreen
                    //    + new Vector2(0, (rt.rect.height * currentCanvas.scaleFactor * 2.5f))
                    //);
            }

            // Set the text in the popup
            BindText();
        }        

        public void ClosePopup()
        {
            SetCurrentCanvas();

            // Set parent to the canvas we found
            Popup.SetParent(currentCanvas.transform, false);

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


        private void SetCurrentCanvas()
        {
            // Get the first canvas in a
            // recursive search of the popup's parents.
            if (Popup != null && TryGetCanvasInParents(Popup.transform, out Canvas existingPopupCanvas))
            {
                currentCanvas = existingPopupCanvas;
            }
            else if (TryGetCanvasInParents(transform, out Canvas thisParentCanvas))
            {
                currentCanvas = thisParentCanvas;
            }
            else
            {
                log.error(
                    $"UI ERROR. Neither {name} nor {Popup.name} is on a canvas. " +
                    $"Make sure {Popup.name} remains on a canvas at all times. " +
                    $"Check the hierarchy in the scene, and also " +
                    $"the {this} script");
                return;
            }
        }

        private bool TryGetCanvasInParents(Transform child, out Canvas canvas)
        {
            Transform searchTarget = child;

            int maxDepth = 100;
            int depth = 0;

            while ( (++depth < maxDepth) && (searchTarget != null) )
            {
                // Update search target
                searchTarget = searchTarget.parent;

                if (searchTarget.TryGetComponent(out canvas))
                {
                    return true;
                }
            }

            canvas = null;
            return false;
        }
    }
}

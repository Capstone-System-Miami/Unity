using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SystemMiami.CombatSystem;
using UnityEngine.EventSystems;
using SystemMiami.Utilities;

namespace SystemMiami
{
    [RequireComponent(typeof(RectTransform))]
    public class InventoryItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private dbug log;

        [Header("Settings")]
        [SerializeField] private ItemType slotType;

        [SerializeField] private HighlightColorSet enabledColors;
        [SerializeField] private HighlightColorSet disabledColors;

        
        [Header("Internal Refs")]
        [SerializeField] private SpriteBox spriteBox;

        private ItemData itemData;

        private Image fallback;
        private bool usingFallback = false;
        private bool fallbackFound;

        public RectTransform RT { get; private set; }

        [field: SerializeField, ReadOnly] bool IsEnabled;
        [field: SerializeField, ReadOnly] private string currentItem;

        private void Awake()
        {
            RT = GetComponent<RectTransform>();
            
            if (spriteBox == null && !TryGetComponent(out spriteBox))
            {
                foreach (Transform child in transform)
                {
                    if (child.TryGetComponent(out spriteBox))
                    {
                        return;
                    }
                    else if (!fallbackFound && child.TryGetComponent(out fallback))
                    {
                        fallbackFound = true;
                    }
                }

                if (spriteBox == null && fallbackFound)
                {
                    usingFallback = true;
                    log.error(
                        $"USING FALLBACK. Didn't find a SpriteBox. " +
                        $"Using {fallback.name}, but if you can, try to use " +
                        $"a SpriteBox, or a prefab that already has one.");
                }
                else if (!fallbackFound)
                {
                    log.error(
                        $"Couldn't find a SpriteBox or an Image " +
                        $"anywhere on {name}");
                }
            }
        }

        public bool TryFill(int itemID)
        {
            itemData = Database.MGR.GetDataWithJustID(itemID);
            Refresh();
            return !itemData.failbit;
        }

        public bool TryFill(ItemData data)
        {
            itemData = data;
            Refresh();
            return !itemData.failbit;
        }

        public void ClearSlot()
        {
            itemData = ItemData.FailedData;

            if (!usingFallback)
            {
                spriteBox.SetBackground(null, Color.grey);
                spriteBox.SetForeground(null, Color.white);
            }
            else
            {
                fallback.sprite = null;
            }

            currentItem = "None";
        }

        private void Refresh()
        {
            if (itemData.failbit) { return; }

            if (!usingFallback)
            {
                spriteBox.SetForeground(itemData.Icon);
            }
            else
            {
                fallback.sprite = itemData.Icon;
            }

            currentItem = itemData.Name;
        }

        public void EnableInteraction()
        {
            IsEnabled = true;
        }

        public void DisableInteraction()
        {
            IsEnabled = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            PopUpHandler.MGR.OpenPopup(itemData, this);


            if (!usingFallback)
            {
                Color toSet = IsEnabled
                    ? enabledColors.Highlighted
                    : disabledColors.Highlighted;

                // could set a highlight color
                spriteBox.SetBackground(toSet);
                spriteBox.SetForeground(itemData.Icon);
            }
            else
            {
                fallback.sprite = itemData.Icon;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Color toSet = IsEnabled
                    ? enabledColors.Unhighlighted
                    : disabledColors.Unhighlighted;

            // could set a highlight color
            spriteBox.SetBackground(toSet);

            PopUpHandler.MGR.ClosePopup();
        }
    }
}

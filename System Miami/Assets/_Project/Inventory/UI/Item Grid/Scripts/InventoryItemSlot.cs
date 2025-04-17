using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SystemMiami.Utilities;

namespace SystemMiami.ui
{
    [RequireComponent(typeof(RectTransform))]
    public class InventoryItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private dbug log;

        [Header("Settings")]
        [SerializeField] private ItemType slotType;

        [SerializeField] private HighlightColorSet enabledColors;
        [SerializeField] private HighlightColorSet disabledColors;

        [Header("Internal Refs")]
        [SerializeField] private SpriteBox spriteBox;

        [SerializeField] private ItemData itemData;

        private Image fallback;
        private bool usingFallback = false;
        private bool fallbackFound;

        public RectTransform RT { get; private set; }

        [field: SerializeField, ReadOnly] bool IsEnabled;
        [field: SerializeField, ReadOnly] private string currentItemStr;

        private float clickTime;
        private float clickDelay = 0.2f;
        private int clicks;
        public bool doubleClick;

        [field: SerializeField, ReadOnly] public int ItemCount { get; private set; }

        public event Action<InventoryItemSlot> slotDoubleClicked;

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

        public bool TryFill(ItemData data)
        {
            return TryFill(data.ID);
        }

        public bool TryFill(int itemID)
        {
            if (itemData.ID == itemID)
            {
                // TODO: Uncomment the rest of this
                // if we can implement UI for ItemCount.
                //
                // if (!itemData.IsStackable)
                // {
                return false;
                // }
                // else
                // {
                //     ItemCount++;
                // }
            }

            itemData = Database.MGR.GetDataWithJustID(itemID);
            Refresh();
            return !itemData.failbit;
        }

        public ItemData ClearSlot()
        {
            ItemData itemCleared = itemData;
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

            currentItemStr = "None";
            return itemCleared;
        }

        private void Refresh()
        {
            if (itemData.failbit) { log.error("Failed to get item data"); return; }

            if (!usingFallback)
            {
                spriteBox.SetForeground(itemData.Icon);
            }
            else
            {
                fallback.sprite = itemData.Icon;
            }

            currentItemStr = itemData.Name;
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
            if (itemData.failbit) { return; }

            PopUpHandler.MGR.OpenPopup(itemData, this);

            if (!usingFallback)
            {
                Color toSet = IsEnabled
                    ? enabledColors.Highlighted
                    : disabledColors.Highlighted;

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
            PopUpHandler.MGR?.ClosePopup();

            if (itemData.failbit) { return; }

            Color toSet = IsEnabled
                    ? enabledColors.Unhighlighted
                    : disabledColors.Unhighlighted;

            // could set a highlight color
            spriteBox.SetBackground(toSet);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            clicks++;
            if (clicks == 1) clickTime = Time.time;

            if (clicks > 1 && Time.time - clickTime < clickDelay)
            {
                clicks = 0;
                clickTime = 0;
                doubleClick = true;
                slotDoubleClicked?.Invoke(this);
                Debug.Log("Double Click: " + this.GetComponent<RectTransform>().name);
            }
            else if (clicks > 2 || Time.time - clickTime > 1)
            {
                clicks = 0;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (doubleClick)
            {
                PopUpHandler.MGR?.ClosePopup();
            }
        }
    }
}

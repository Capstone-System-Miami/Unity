using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SystemMiami
{
    public class ToggleButton : BetterButton, ISelectable, IToggleable
    {
        [SerializeField] private UnityEvent AdditionalOnButtonSelected;
        [SerializeField] private UnityEvent AdditionalOnButtonDeselected;

        [field: SerializeField, ReadOnly] public virtual bool IsSelected { get; protected set; }

        public virtual void Select()
        {
            SelectableSprite.Select();
            AdditionalOnButtonSelected?.Invoke();
            IsSelected = true;
        }

        public virtual void Deselect()
        {
            SelectableSprite.Deselect();
            AdditionalOnButtonDeselected?.Invoke();
            IsSelected = false;
        }

        public virtual void Toggle()
        {
            if (IsSelected)
            {
                Deselect();
            }
            else
            {
                Select();
            }
        }

        protected override void OnGoodClickDown(PointerEventData eventData)
        {
            Toggle();
        }

        protected override void OnGoodClickUp(PointerEventData eventData)
        {
        }

        protected override void OnBadClickDown(PointerEventData eventData)
        {
        }

        protected override void OnBadClickUp(PointerEventData eventData)
        {
        }
    }
}

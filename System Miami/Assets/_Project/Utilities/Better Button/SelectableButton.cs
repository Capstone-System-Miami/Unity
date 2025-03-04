using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SystemMiami
{
    public class SelectableButton : BetterButton, ISelectable
    {
        [SerializeField] private UnityEvent AdditionalOnButtonSelected;
        [SerializeField] private UnityEvent AdditionalOnButtonDeselected;

        [field: SerializeField, ReadOnly] public virtual bool IsSelected { get; protected set; }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            Toggle();
        }

        public virtual void Select()
        {
            selectableSprite.Select();
            IsSelected = true;
        }

        public virtual void Deselect()
        {
            selectableSprite.Deselect();
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
    }
}

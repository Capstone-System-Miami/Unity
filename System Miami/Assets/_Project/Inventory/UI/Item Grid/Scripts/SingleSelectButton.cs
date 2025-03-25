using System;
using SystemMiami.ui;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SystemMiami
{
    public class SingleSelectButton : BetterButton, ISingleSelectable
    {
        [SerializeField] private UnityEvent AdditionalOnButtonSelected;
        [SerializeField] private UnityEvent AdditionalOnButtonDeselected;

        [field: SerializeField, ReadOnly] public virtual bool IsSelected { get; protected set; }

        private Action<int> notifySelectionGroup;

        int ISingleSelectable.SelectionIndex { get; set; }

        public void Init(Action<int> notifySelectionGroup)
        {
            this.notifySelectionGroup = notifySelectionGroup;
        }

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

        protected override void OnGoodClickDown(PointerEventData eventData)
        {
            Assert.IsNotNull(notifySelectionGroup);

            notifySelectionGroup((this as ISingleSelectable).SelectionIndex);
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

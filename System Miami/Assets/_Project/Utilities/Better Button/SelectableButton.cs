using UnityEngine;
using System;
using UnityEngine.Events;

namespace SystemMiami
{
    public class SelectableButton : BetterButton, ISelectable
    {
        [SerializeField] private UnityEvent ButtonSelectedEvent;
        [SerializeField] private UnityEvent ButtonDeselectedEvent;

        protected override Action ClickStrategy => IsSelected
            ? OnButtonDeselected
            : OnButtonSelected;

        public bool IsSelected { get; protected set; }

        protected virtual void OnButtonSelected()
        {
            ButtonSelectedEvent.Invoke();
        }

        protected virtual void OnButtonDeselected()
        {
            ButtonDeselectedEvent.Invoke();
        }

        public void Select()
        {
            OnButtonSelected();
            IsSelected = true;
        }

        public void Deselect()
        {
            OnButtonDeselected();
            IsSelected = false;
        }
    }
}

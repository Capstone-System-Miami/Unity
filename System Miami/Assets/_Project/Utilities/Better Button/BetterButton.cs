using SystemMiami.InventorySystem;
using UnityEngine;
using UnityEngine.UI;
using SystemMiami.ui;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Threading;

namespace SystemMiami
{
    [RequireComponent(
        typeof(SelectableSprite),
        typeof(EventTrigger) )]
    public abstract class BetterButton : MonoBehaviour
    {
        protected SelectableSprite selectableSprite;

        protected abstract Action ClickStrategy { get; }

        private void Awake()
        {
            selectableSprite = GetComponent<SelectableSprite>();
        }

        public void Click()
        {
            ClickStrategy?.Invoke();
        }
    }
}

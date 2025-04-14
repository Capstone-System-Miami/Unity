using System;
using UnityEngine;

namespace SystemMiami.ui
{
    public class EndCombatWindow : Window
    {
        public override Type GenericType => GetType();

        IWindowable windowableObject;
        WindowData windowData;

        public override void Initialize(IWindowable windowableObject)
        {
            this.windowableObject = windowableObject;
            this.windowData = windowableObject.WindowData;
        }

        public override void Open()
        {
            return;
        }

        public override void Close()
        {
            return;
        }

        public override void Show()
        {
            return;
        }

        public override void Hide()
        {
            return;
        }
    }
}

// Authors: Layla
using System;
using SystemMiami.ui;

namespace SystemMiami.Management
{
    public class UI : Singleton<UI>
    {
        public Action<AbilitySlot> SlotClicked;

        protected override void Awake()
        {
            base.Awake();
        }
    }
}

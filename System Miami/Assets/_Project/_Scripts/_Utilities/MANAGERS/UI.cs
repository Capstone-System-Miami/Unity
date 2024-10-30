using System;
using SystemMiami.UI;

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

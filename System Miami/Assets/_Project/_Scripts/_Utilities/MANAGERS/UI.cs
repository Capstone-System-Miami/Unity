using System;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.AbilitySystem;
using UnityEngine;
using UnityEngine.Events;
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

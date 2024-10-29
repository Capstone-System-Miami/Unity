using System;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using UnityEngine;
using UnityEngine.Events;

namespace SystemMiami.Management
{
    public class UI : Singleton<UI>
    {
        public UnityEvent<int> EquipAbility;
        public UnityEvent UnequipAbility;

        protected override void Awake()
        {
            base.Awake();
        }
    }
}

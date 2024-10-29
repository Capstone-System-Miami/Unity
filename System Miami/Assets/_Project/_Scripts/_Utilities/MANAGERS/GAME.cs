using System;
using SystemMiami.AbilitySystem;
using SystemMiami.CombatSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace SystemMiami.Management
{
    public class GAME : Singleton<GAME>
    {
        public Action<Combatant> CombatantDeath;

        protected override void Awake()
        {
            base.Awake();
        }
    }
}

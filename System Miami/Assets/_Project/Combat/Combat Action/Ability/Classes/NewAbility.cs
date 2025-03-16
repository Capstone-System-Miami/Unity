using System.Collections;
using System.Linq;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;
using System;

namespace SystemMiami.CombatRefactor
{
    public abstract class NewAbility : CombatAction
    {
        public readonly float ResourceCost;
        public readonly int CooldownTurns;

        private Action<float> looseResource;
        private int cooldownRemaining;

        public bool IsOnCooldown
        {
            get
            {
                return cooldownRemaining > 0;
            }
        }

        protected NewAbility(NewAbilitySO preset, Combatant user, Action<float> looseResource)
            : base(
                  preset.Icon,
                  preset.itemData.ID,
                  preset.Actions.ToList(),
                  preset.OverrideController,
                  user,preset.VFXPrefab)
        {
            this.ResourceCost = preset.ResourceCost;
            this.CooldownTurns = preset.CooldownTurns;
            this.looseResource = looseResource;
        }

        public void ReduceCooldown()
        {
            if (cooldownRemaining > 0)
            {
                cooldownRemaining--;
            }
        }

        protected override void PreExecution()
        {
            base.PreExecution();
            looseResource?.Invoke(ResourceCost);
        }

        protected override void PostExecution()
        {
            startCooldown();
        }
        private void startCooldown()
        {
            cooldownRemaining = CooldownTurns;
        }
    }
}

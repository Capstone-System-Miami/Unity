using System.Collections;
using System.Linq;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class NewAbility : CombatAction
    {
        public readonly float ResourceCost;
        public readonly int CooldownTurns;

        private Resource targetResource;
        private int cooldownRemaining;

        public bool IsOnCooldown
        {
            get
            {
                return cooldownRemaining > 0;
            }
        }

        protected NewAbility(NewAbilitySO preset, Combatant user, Resource targetResource)
            : base(
                  preset.Icon, preset.itemData.ID,
                  preset.Actions.ToList(),
                  preset.OverrideController,
                  user)
        {
            this.ResourceCost = preset.ResourceCost;
            this.CooldownTurns = preset.CooldownTurns;
            this.targetResource = targetResource;
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
            targetResource.Lose(ResourceCost);
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

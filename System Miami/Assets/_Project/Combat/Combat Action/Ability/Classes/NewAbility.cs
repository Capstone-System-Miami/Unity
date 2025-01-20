using System.Collections;
using SystemMiami.CombatSystem;

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
                  preset.Icon,
                  preset.Actions,
                  preset.OverrideController,
                  user)
        {
            this.ResourceCost = preset.ResourceCost;
            this.CooldownTurns = preset.CooldownTurns;
            this.targetResource = targetResource;
        }

        // TODO:
        // (layla question)
        // Does this need to be an IEnumerator?
        // It feels like it could just be a System.Action
        // or other delegate type
        public override IEnumerator Use()
        {
            targetResource.Lose(ResourceCost);
            yield return null;

            performActions();

            startCooldown();
        }

        public void ReduceCooldown()
        {
            if (cooldownRemaining > 0)
            {
                cooldownRemaining--;
            }
        }

        private void startCooldown()
        {
            cooldownRemaining = CooldownTurns;
        }
    }
}

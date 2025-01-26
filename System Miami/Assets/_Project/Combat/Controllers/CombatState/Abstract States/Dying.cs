using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class Dying : CombatantState
    {
        protected Dying(Combatant combatant)
            : base(combatant, Phase.None) { }

        public override void OnEnter()
        {
            base.OnEnter();

            // start animation?
        }

        public override void MakeDecision()
        {
            if (Proceed())
            {
                GoToDead();
            }
        }

        protected abstract bool Proceed();

        protected abstract void GoToDead();
    }
}

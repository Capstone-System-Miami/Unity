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
            if (DeadRequested())
            {
                SwitchState(factory.Dead());
            }
        }

        protected abstract bool DeadRequested();
    }
}

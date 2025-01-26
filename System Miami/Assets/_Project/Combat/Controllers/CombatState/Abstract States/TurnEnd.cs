using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class TurnEnd : CombatantState
    {
        public TurnEnd(Combatant combatant)
            : base(combatant, Phase.None) { }

        public override void OnEnter()
        {
            Debug.Log($"{machine.name}Calling end of turn");
        }

        public override void MakeDecision()
        {
            if (Proceed())
            {
                GoToIdle();
                return;
            }
        }

        public override void OnExit()
        {
            combatant.IsMyTurn = false;
        }

        // Decision
        protected abstract bool Proceed();

        // Outcome
        protected abstract void GoToIdle();
    }
}

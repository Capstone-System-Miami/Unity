using System.Collections;
using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public abstract class ActionExecution : CombatantState
    {
        public ActionExecution(Combatant combatant)
            : base(combatant, Phase.Action) { }

        public override void OnEnter()
        {
            if (combatant.Abilities.AbilityExecutionIsValid
                (out IEnumerator abilityProcess))
            {
                machine.StartCoroutine(abilityProcess);
            }
        }

        public override void MakeDecision()
        {
            GoToEndTurn();
            return;
        }

        // Only outcome
        protected abstract void GoToEndTurn();
    }
}

using System.Collections;
using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public class ActionExecution : CombatantState
    {
        CombatAction combatAction;

        public ActionExecution(Combatant combatant, CombatAction combatAction)
            : base(combatant, Phase.Action)
        {
            this.combatAction = combatAction;
        }

        public override void OnEnter()
        {
            if (combatant.Abilities.AbilityExecutionIsValid
                (out IEnumerator abilityProcess))
            {
                combatant.StartCoroutine(abilityProcess);
            }
        }

        public override void MakeDecision()
        {
            SwitchState(factory.TurnEnd());
            return;
        }

    }
}

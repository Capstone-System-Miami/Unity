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
            base.OnEnter();
            combatAction.BeginExecution();

            InputPrompts =
                "Executing Action.";
        }

        public override void MakeDecision()
        {
            SwitchState(factory.TurnEnd());
            return;
        }

        public override void OnExit()
        {
            base.OnExit();

            combatAction.Unequip();
        }
    }
}

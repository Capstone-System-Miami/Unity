using System.Collections;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;

namespace SystemMiami.CombatRefactor
{
    public class ActionExecution : CombatantState
    {
        CombatAction combatAction;

        Conditions proceedConditions = new();

        public ActionExecution(Combatant combatant, CombatAction combatAction)
            : base(combatant, Phase.Action)
        {
            this.combatAction = combatAction;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            combatAction.Equip();
            combatAction.BeginExecution();
            proceedConditions.Add( () => combatAction.ExecutionStarted);
            proceedConditions.Add( () => combatAction.ExecutionFinished);

            InputPrompts =
                "Executing Action.";
        }

        public override void MakeDecision()
        {
            if (!proceedConditions.AllMet()) { return; }

            SwitchState(factory.TurnEnd());
        }

        public override void OnExit()
        {
            base.OnExit();

            combatAction.Unequip();
        }
    }
}

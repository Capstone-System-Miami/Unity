using System.Collections;
using SystemMiami.CombatSystem;
using SystemMiami.Management;
using SystemMiami.Utilities;
using UnityEngine;

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

            proceedConditions.Add( () => combatAction.ExecutionStarted);
            proceedConditions.Add( () => combatAction.ExecutionFinished);

            combatAction.Equip();
            combatAction.LockTargets();
            combatAction.BeginExecution();

            InputPrompts =
                "Executing Action.";

            UI.MGR.ClearInputPrompt();
        }

        public override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                combatAction.Reportback();
            }
        }

        public override void MakeDecision()
        {
            if (!proceedConditions.AllMet()) { return; }

            SwitchState(factory.TurnEnd());
        }

        public override void OnExit()
        {
            base.OnExit();

            combatAction.CleanUp();
        }
    }
}

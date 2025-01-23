using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class PlayerActionSelection : ActionSelection
    {
        public PlayerActionSelection(CombatantStateMachine machine)
            : base(machine) { }

        public override void cMakeDecision()
        {
            if (ActionSelected())
            {
                machine.SwitchState(new PlayerActionTileSelections(machine));
            }
        }

        protected override bool ActionSelected()
        {
            throw new System.NotImplementedException();
        }
    }
}

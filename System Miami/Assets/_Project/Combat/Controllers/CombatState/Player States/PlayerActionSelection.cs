using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class PlayerActionSelection : ActionSelection
    {
        public PlayerActionSelection(Combatant combatant)
            : base(combatant) { }

        public override void cMakeDecision()
        {
            if (ActionSelected())
            {
                machine.SwitchState(new PlayerActionTileSelections(combatant));
            }
        }

        protected override bool ActionSelected()
        {
            throw new System.NotImplementedException();
        }
    }
}

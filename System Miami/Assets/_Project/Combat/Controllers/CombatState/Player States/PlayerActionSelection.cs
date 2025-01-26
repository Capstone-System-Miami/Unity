using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class PlayerActionSelection : ActionSelection
    {
        public PlayerActionSelection(Combatant combatant)
            : base(combatant) { }

        protected override bool ActionSelected()
        {
            // Event from UI?
            return false;
        }

        protected override bool SkipPhase()
        {
            return Input.GetKeyDown(KeyCode.Q);
        }

        protected override void GoToActionEquipped()
        {
            machine.SetState(new PlayerActionEquipped(combatant));
        }

        protected override void GoToEndTurn()
        {
            machine.SetState(new PlayerTurnEnd(combatant));
        }
    }
}

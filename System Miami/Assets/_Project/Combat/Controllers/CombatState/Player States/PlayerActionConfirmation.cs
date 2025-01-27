using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class PlayerActionConfirmation : ActionConfirmation
    {
        public PlayerActionConfirmation(Combatant combatant)
            : base(combatant) { }

        protected override bool CancelSelection()
        {
            // Player right clicks?
            return false;
        }

        protected override bool ConfirmSelection()
        {
            // Player presses enter?
            return false;
        }
    }
}

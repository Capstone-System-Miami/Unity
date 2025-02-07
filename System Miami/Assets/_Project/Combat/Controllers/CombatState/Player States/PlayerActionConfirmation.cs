using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class PlayerActionConfirmation : ActionConfirmation
    {
        public PlayerActionConfirmation(Combatant combatant, CombatAction combatAction)
            : base(combatant, combatAction) { }

        protected override bool CancelConfirmation()
        {
            // Player right clicks
            return Input.GetMouseButtonDown(1);
        }

        protected override bool ConfirmSelection()
        {
            // Player presses enter
            return Input.GetKeyDown(KeyCode.Return);
        }
    }
}

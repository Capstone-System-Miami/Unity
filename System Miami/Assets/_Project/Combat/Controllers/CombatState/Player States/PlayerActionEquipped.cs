using SystemMiami.CombatRefactor;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami
{
    public class PlayerActionEquipped : ActionEquipped
    {
        public PlayerActionEquipped(Combatant combatant) : base(combatant) { }

        protected override bool SelectTile()
        {
            // Player clicks? Event from UI buttons?
            return false;
        }

        protected override bool Unequip()
        {
            // Player right clicks?
            return false;
        }

        protected override void GoToActionSelection()
        {
            machine.SetState(new PlayerActionSelection(combatant));
        }

        protected override void GoToActionTileConfirmation()
        {
            machine.SetState(new PlayerActionConfirmation(combatant));
        }

    }
}

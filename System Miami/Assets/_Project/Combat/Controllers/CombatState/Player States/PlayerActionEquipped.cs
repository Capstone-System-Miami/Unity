using SystemMiami.CombatRefactor;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami
{
    public class PlayerActionEquipped : ActionEquipped
    {
        public PlayerActionEquipped(Combatant combatant, CombatAction combatAction)
            : base(combatant, combatAction) { }

        protected override bool SelectTileRequested()
        {
            // Player clicks? Event from UI buttons?
            return false;
        }

        protected override bool UnequipRequested()
        {
            // Player right clicks?
            return Input.GetMouseButtonDown(1);
        }
    }
}

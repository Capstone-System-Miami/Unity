using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class EnemyActionEquipped : ActionEquipped
    {
        public EnemyActionEquipped(Combatant combatant, CombatAction combatAction)
            : base (combatant, combatAction) { }

        protected override bool SelectTileRequested()
        {
            // Once enemy has checked enough tiles?
            return false;
        }

        protected override bool UnequipRequested()
        {
            // Never happens?
            // Maybe they cycle through their
            // weapons to check if any have hit the player?
            return false;
        }
    }
}

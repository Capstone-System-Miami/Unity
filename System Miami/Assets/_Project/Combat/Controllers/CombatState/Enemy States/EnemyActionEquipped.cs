using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class EnemyActionEquipped : ActionEquipped
    {
        public EnemyActionEquipped(Combatant combatant)
            : base (combatant) { }

        protected override bool SelectTile()
        {
            // Once enemy has checked enough tiles?
            return false;
        }

        protected override bool Unequip()
        {
            // Never happens?
            // Maybe they cycle through their
            // weapons to check if any have hit the player?
            return false;
        }
    }
}

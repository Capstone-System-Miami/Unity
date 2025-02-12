using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    /// <summary>
    /// This state is effectively bypassed. The delay happens
    /// within EnemyActionConfirmation, which can decide to cancel
    /// and come back here to try a new target set with a new
    /// focus tile.
    /// </summary>
    public class EnemyActionEquipped : ActionEquipped
    {
        public EnemyActionEquipped(Combatant combatant, CombatAction combatAction)
            : base (combatant, combatAction) { }

        /// <inheritdoc/>
        /// <summary>
        /// TODO: This might come into play later.
        /// Maybe if they don't find the player in their targets,
        /// they could equip another CombatAction?
        /// </summary>
        /// <returns></returns>
        protected override bool ReEquipRequested()
        {
            return false;
        }

        protected override bool SelectTileRequested()
        {
            return true;
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

using System.Collections.Generic;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class EnemyMovementConfirmation : MovementConfirmation
    {
        public EnemyMovementConfirmation(
            Combatant combatant,
            MovementPath path)
                : base(combatant, path) { }

        /// <summary>
        /// Will return FALSE every time
        /// because the enemy makes its
        /// movement selection based on
        /// (a) randomness
        /// (b) player location within range,
        /// but this decision happens in the
        /// Selection state
        /// </summary>
        protected override bool CancelSelection()
        {
            return false;
        }

        /// <summary>
        /// Will return TRUE every time
        /// because the enemy makes its
        /// movement selection based on
        /// (a) randomness
        /// (b) player location within range,
        /// but this decision happens in the
        /// Selection state
        /// </summary>
        protected override bool ConfirmSelection()
        {
            return true;
        }

        protected override void GoToMovementExecution()
        {
            machine.SetState(new EnemyMovementExecution(combatant, path));
        }

        protected override void GoToMovementTileSelection()
        {
            machine.SetState(new EnemyMovementTileSelection(combatant));
        }
    }
}

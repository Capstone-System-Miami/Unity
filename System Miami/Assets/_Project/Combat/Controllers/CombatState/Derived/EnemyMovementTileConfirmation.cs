using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class EnemyMovementTileConfirmation : MovementTileConfirmation
    {
        public EnemyMovementTileConfirmation(
            CombatantStateMachine machine,
            List<OverlayTile> currentPath,
            List<OverlayTile> arrowPath
            )
                : base (
                      machine,
                      currentPath,
                      arrowPath)
            { }

        public override void cMakeDecision()
        {
            if (CancelSelection())
            {
                machine.SwitchState(new EnemyMovementTileSelection(machine));
                return;
            }

            if (ConfirmSelection())
            {
                machine.SwitchState(new EnemyMovementExecution(machine));
                return;
            }
        }

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
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class PlayerMovementTileConfirmation : MovementTileConfirmation
    {
        public PlayerMovementTileConfirmation(
            CombatantStateMachine machine,
            List<OverlayTile> currentPath,
            List<OverlayTile> arrowPath
            )
                : base(
                      machine,
                      currentPath,
                      arrowPath
                      )
            { }

        public override void cMakeDecision()
        {
            if (CancelSelection())
            {
                machine.SwitchState(new PlayerMovementTileSelection(machine));
                return;
            }

            if (ConfirmSelection())
            {
                machine.SwitchState(new PlayerMovementExecution(machine));
                return;
            }
        }

        protected override bool CancelSelection()
        {
            return Input.GetMouseButtonDown(1);
        }

        protected override bool ConfirmSelection()
        {
            return Input.GetKeyDown(KeyCode.Return);
        }
    }
}

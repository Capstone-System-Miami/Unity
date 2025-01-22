using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class PlayerMovementTileSelection : MovementTileSelection
    {
        public PlayerMovementTileSelection(CombatantStateMachine machine)
                : base(machine) { }

        public override void cMakeDecision()
        {
            if (SelectTile())
            {                
                machine.SwitchState(
                    new PlayerMovementTileConfirmation(
                        machine,
                        truncatedPath,
                        arrowPath
                        )
                    );
                return;
            }

            if (SkipPhase())
            {
                machine.SwitchState(
                    new PlayerActionSelection(
                        machine
                        )
                    );
                return;
            }
        }

        protected override OverlayTile GetFocusedTile()
        {
            throw new System.NotImplementedException();
        }

        protected override bool SelectTile()
        {
            return Input.GetMouseButtonDown(0);
        }

        protected override bool SkipPhase()
        {
            return Input.GetKeyDown(KeyCode.Tab);
        }
    }
}

using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class EnemyMovementTileSelection : MovementTileSelection
    {
        public EnemyMovementTileSelection(CombatantStateMachine machine)
            : base(machine) { }

        public override void cMakeDecision()
        {
            if (SelectTile())
            {
                machine.SwitchState(
                    new EnemyMovementTileConfirmation(
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
                    new EnemyActionSelection(
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
            return true;
        }

        protected override bool SkipPhase()
        {
            // TODO:
            // If the phase has to be skipped
            // because of some status effect,
            // this is where that would happen.
            return false;
        }
    }
}

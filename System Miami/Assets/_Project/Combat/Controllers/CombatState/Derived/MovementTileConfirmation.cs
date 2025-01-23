using System.Collections.Generic;

namespace SystemMiami.CombatRefactor
{
    public abstract class MovementTileConfirmation : CombatantState
    {
        List<OverlayTile> currentPath;
        List<OverlayTile> arrowPath;

        protected MovementTileConfirmation(
            CombatantStateMachine machine,
            List<OverlayTile> currentPath,
            List<OverlayTile> arrowPath)
            : base(machine, Phase.Movement)
        {
            this.currentPath = currentPath;
            this.arrowPath = arrowPath;
        }

        public override void aOnEnter()
        {
            machine.combatant.DestinationTile = machine.combatant.FocusTile;
            DrawArrows.MGR.DrawPath(arrowPath);
        }


        public override void bUpdate()
        {
        }

        public override abstract void cMakeDecision();

        public override void eOnExit()
        {
            machine.CurrentPath = currentPath;
        }

        protected abstract bool ConfirmSelection();
        protected abstract bool CancelSelection();
    }
}

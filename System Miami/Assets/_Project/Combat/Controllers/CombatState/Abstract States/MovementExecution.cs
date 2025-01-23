using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class MovementExecution : CombatantState
    {
        protected const float PLACEMENT_RANGE = 0.0001f;

        protected OverlayTile targetTile;

        protected float distanceToTarget;

        protected MovementExecution(CombatantStateMachine context)
            : base(context, Phase.Movement) { }

        public override void aOnEnter()
        {
        }

        public override void bUpdate()
        {
            if (destinationReached())
            {
                return;
            }

            targetTile = machine.CurrentPath[0];

            distanceToTarget = Vector2.Distance(
                machine.combatant.transform.position,
                targetTile.transform.position
                );

            stepTowardsTarget();

            // If character is close enough to a new tile
            if (inPlacementRangeOf(targetTile))
            {
                placeCombatantOn(targetTile);
                machine.CurrentPath.RemoveAt(0);
            }
        }

        public override abstract void cMakeDecision();

        public override void eOnExit()
        {
            machine.combatant.FocusTile?.UnHighlight(); // on phase transit
        }

        /// <summary>
        /// Returns false if there is
        /// no current destination tile.
        /// Returns true if there is no current path,
        /// or if the combatant's current tile is the same
        /// as the current destination tile.
        /// </summary>
        protected bool destinationReached()
        {
            return machine.combatant.CurrentTile == machine.combatant.DestinationTile;
        }

        /// <summary>
        /// Clears destination tile,
        /// current path, and movement bool.
        /// </summary>
        protected void clearMovement()
        {
            machine.combatant.DestinationTile = machine.combatant.CurrentTile;
            machine.CurrentPath = null;
        }

        private void stepTowardsTarget()
        {
            float stepDistance = machine.movementSpeed * Time.deltaTime;

            Vector2 positionAfterStep = Vector2.MoveTowards(
                machine.combatant.transform.position,
                targetTile.transform.position,
                stepDistance);

            machine.combatant.transform.position = positionAfterStep;
        }

        private bool inPlacementRangeOf(OverlayTile tile)
        {
            return distanceToTarget < PLACEMENT_RANGE;
        }

        private void placeCombatantOn(OverlayTile newTile)
        {
            // Directional info based on the current tile
            // and the one we're moving to.
            DirectionContext newDir = new DirectionContext(
                (Vector2Int)machine.combatant.CurrentTile.GridLocation,
                (Vector2Int)newTile.GridLocation
                );

            // Let the combatant know that we are moving along path
            machine.combatant.PathTileChanged(newDir);

            newTile.PlaceCombatant(machine.combatant);
        }


    }
}

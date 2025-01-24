using System.Collections.Generic;
using System.Linq;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class MovementExecution : CombatantState
    {
        protected MovementPath path;
        protected List<OverlayTile> movementPath = new();
        protected OverlayTile currentCombatantTile;
        protected OverlayTile currentFocusTile;
        protected OverlayTile destinationTile;

        protected float distanceToTarget;

        protected MovementExecution(
            Combatant combatant,
            MovementPath limitedPath)
                : base(
                    combatant,
                    Phase.Movement
                )
        {
            this.path = limitedPath;
        }

        public override void aOnEnter()
        {
            movementPath = path.ForMovement;
        }

        public override void bUpdate()
        {
            if (!movementPath.Any()) { return; }

            currentCombatantTile = combatant.CurrentTile;
            currentFocusTile = movementPath[0];

            combatant.StepTowards(currentFocusTile);

            if (combatant.InPlacementRangeOf(currentFocusTile))
            {
                combatant.SnapTo(currentFocusTile);
                currentFocusTile.PlaceCombatant(combatant);
                movementPath.RemoveAt(0);
            }
        }

        public override abstract void cMakeDecision();

        public override void eOnExit()
        {
            DrawArrows.MGR.RemoveArrows();
        }
    }
}

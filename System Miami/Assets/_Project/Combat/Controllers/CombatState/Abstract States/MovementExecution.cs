using System.Collections.Generic;
using System.Linq;
using SystemMiami.CombatSystem;

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
            MovementPath movementPath)
                : base(
                    combatant,
                    Phase.Movement
                )
        {
            this.path = movementPath;
        }

        public override void OnEnter()
        {
            movementPath = path.ForMovement;
        }

        public override void Update()
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

        public override void MakeDecision()
        {
            if (movementPath.Any()) { return; }

            GoToActionSelection();

            if (MoveAgain())
            {
                GoToMovementTileSelection();
                return;
            }
            else
            {
                GoToActionSelection();
                return;
            }
        }

        public override void OnExit()
        {
            DrawArrows.MGR.RemoveArrows();
        }

        // Decision
        protected abstract bool MoveAgain();

        // Outcomes
        protected abstract void GoToMovementTileSelection();
        protected abstract void GoToActionSelection();
        protected abstract void GoToTurnEnd();
    }
}

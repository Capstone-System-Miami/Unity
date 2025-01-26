using System.Linq;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    // waits for input
    public abstract class MovementTileSelection : CombatantState
    {
        int currentSpeedStat;

        OverlayTile occupiedTile;
        OverlayTile currentFocusTile;
        OverlayTile currentDestinationTile;

        // Pathing
        protected MovementPath movementPath;

        protected MovementTileSelection(Combatant combatant)
            : base(combatant, Phase.Movement) { }

        public override void OnEnter()
        {
            currentSpeedStat = (int)combatant.Speed.Get();

            occupiedTile = combatant.CurrentTile;
        }


        public override void Update()
        {
            // Find a new possible focus tile
            // by the means described
            // by int the derived classes.
            if (!TryGetNewFocus(out OverlayTile newFocus))
            {
                // Focus was not new.
                // Nothing to update.
                return;
            }

            // Update currentTile & tile hover
            currentFocusTile?.EndHover(combatant);
            currentFocusTile = newFocus;
            currentFocusTile?.BeginHover(combatant);

            DirectionContext newDirection = GetNewDirection();

            // Update animator based on direction.
            combatant.UpdateAnimDirection(newDirection.ScreenDirection);

            // Generate a path
            movementPath = new(
                occupiedTile,
                currentFocusTile,
                currentSpeedStat
                );

            if (movementPath.IsEmpty) { return; }

            movementPath.Draw();
        }

        public override void MakeDecision()
        {
            if (SkipPhase())
            {
                GoToActionSelection();
                return;
            }

            if (movementPath.IsEmpty) { return; }

            if (SelectTile())
            {
                GoToTileConfirmation();
                return;
            }
        }


        // Decision
        protected abstract bool SkipPhase();
        protected abstract bool SelectTile();


        // Decision outcomes
        protected abstract void GoToActionSelection();
        protected abstract void GoToTileConfirmation();

        // Focus
        protected bool TryGetNewFocus(out OverlayTile newFocus)
        {
            newFocus = GetNewFocus();

            return newFocus != currentFocusTile;
        }

        /// <summary>
        /// Whatever tile the combatant
        /// is currently focusing on.
        /// The methods for determining this
        /// must be defined in derived classes.
        /// </summary>
        protected abstract OverlayTile GetNewFocus();

        protected DirectionContext GetNewDirection()
        {
            Vector2Int occupiedPos = (Vector2Int)occupiedTile.GridLocation;

            // If the character isn't focusing on anything,
            // Then use the position 1 tile in "front" of them,
            // **relative to where the already are**,
            // not forward relative to the board.
            Vector2Int focusPos = (currentFocusTile != null) ?
                (Vector2Int)currentFocusTile.GridLocation
                : combatant.CurrentDirectionContext.ForwardA;

            return new DirectionContext(occupiedPos, focusPos);
        }
    }
}

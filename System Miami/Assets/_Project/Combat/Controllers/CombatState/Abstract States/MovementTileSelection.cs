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

        OverlayTile currentPlayerTile;
        OverlayTile currentFocusTile;
        OverlayTile currentDestinationTile;

        // Pathing
        protected MovementPath limitedPath;

        protected MovementTileSelection(Combatant combatant)
            : base(combatant, Phase.Movement) { }

        public override void aOnEnter()
        {
            currentSpeedStat = (int)combatant.Speed.Get();

            currentPlayerTile = combatant.CurrentTile;
        }


        public override void bUpdate()
        {
            // Find a new possible focus tile
            // by the means described
            // by int the derived classes.
            if (!TryGetNewFocus(out OverlayTile newFocus))
            {
                // Focus was not new.
                return;
            }

            // Update currentTile & tile hover
            currentFocusTile?.EndHover(combatant);
            currentFocusTile = newFocus;
            currentFocusTile?.BeginHover(combatant);

            if (!TryGetNewDirection(out DirectionContext newDir))
            {
                // Focus was null
                return;
            }

            // Update animator based on direction.
            combatant.UpdateAnimDirection(newDir.ScreenDirection);

            // Generate a path
            limitedPath = new(
                currentPlayerTile,
                currentFocusTile,
                currentSpeedStat
            );

            if (limitedPath.IsEmpty) { return; }

            limitedPath.Draw();
        }

        public override void cMakeDecision()
        {
            if (SkipPhase())
            {
                GoToActionSelection();
                return;
            }

            if (!limitedPath.ForMovement.Any()) { return; }

            if (SelectTile())
            {
                GoToTileConfirmation();
                return;
            }
        }

        public override void eOnExit()
        {
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

        protected bool TryGetNewDirection(out DirectionContext newDir)
        {
            if (currentFocusTile == null)
            {
                newDir = new DirectionContext();
                return false;
            }

            newDir = new DirectionContext(
                (Vector2Int)currentPlayerTile.GridLocation,
                (Vector2Int)currentFocusTile.GridLocation
            );

            return true;
        }
    }
}

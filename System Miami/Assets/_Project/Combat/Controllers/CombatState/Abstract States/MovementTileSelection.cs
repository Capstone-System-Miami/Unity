using System.Collections.Generic;
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
        protected MovementPath path;

        protected MovementTileSelection(Combatant combatant)
            : base(combatant, Phase.Movement) { }

        public override void OnEnter()
        {
            base.OnEnter();
            currentSpeedStat = (int)combatant.Speed.Get();

            occupiedTile = combatant.CurrentTile;
        }


        public override void Update()
        {
            occupiedTile = combatant.CurrentTile;
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
            combatant.CurrentDirectionContext = newDirection;

            // Update animator based on direction.
            combatant.UpdateAnimDirection(newDirection.ScreenDirection);


            path?.Unhighlight();
            // Generate a path
            path = new(
                occupiedTile,
                currentFocusTile,
                currentSpeedStat
                );

            if (path.IsEmpty) { return; }

            path.DrawArrows();

            path.HighlightValidMoves(Color.yellow);
            path.HighlightInvalidMoves(Color.red);
        }

        public override void MakeDecision()
        {
            if (SkipPhase())
            {
                GoToActionSelection();
                return;
            }

            if (path == null) { return; }
            if (path.IsEmpty) { return; }

            if (SelectTile())
            {
                GoToTileConfirmation();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            path.UnDrawAll();
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
            newFocus = GetNewFocus() ?? GetDefaultFocus();

            return newFocus != currentFocusTile;
        }

        /// <summary>
        /// Whatever tile the combatant
        /// is currently focusing on.
        /// The methods for determining this
        /// must be defined in derived classes.
        /// </summary>
        protected abstract OverlayTile GetNewFocus();

        protected OverlayTile GetDefaultFocus()
        {
            OverlayTile result;

            Vector2Int forwardPos
                = combatant.CurrentDirectionContext.ForwardA;

            if (!MapManager.MGR.map.TryGetValue(forwardPos, out result))
            {
                Debug.LogError(
                    $"FATAL | {combatant.name}'s {this}" +
                    $"FOUND NO TILE TO FOCUS ON."
                    );
            }

            return result;
        }

        protected DirectionContext GetNewDirection()
        {
            Vector2Int occupiedPos = (Vector2Int)combatant.CurrentTile.GridLocation;

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

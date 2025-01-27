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

        // Tiles
        OverlayTile occupiedTile;
        OverlayTile focusTile;

        // Pathing
        protected MovementPath path;

        // Transitioning
        protected Conditions turnEndConditions = new();
        protected Conditions skipMovementConditions = new();
        protected Conditions confirmPathConditions = new();

        protected MovementTileSelection(Combatant combatant)
            : base(combatant, Phase.Movement) { }

        public override void OnEnter()
        {
            base.OnEnter();
            currentSpeedStat = (int)combatant.Speed.Get();

            occupiedTile = combatant.CurrentTile;

            confirmPathConditions.Add(() => path != null);
            confirmPathConditions.Add(() => !path.IsEmpty);
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
            focusTile?.EndHover(combatant);
            focusTile = newFocus;
            focusTile?.BeginHover(combatant);

            combatant.CurrentDirectionContext = GetNewDirection();

            // Update animator based on direction.
            combatant.UpdateAnimDirection(
                combatant.CurrentDirectionContext.ScreenDirection);

            // If there is already a
            // path set, unhighlight it.
            path?.Unhighlight();

            // Generate a path based on 
            path = new(
                occupiedTile,
                focusTile,
                currentSpeedStat
                );

            if (path.IsEmpty) { return; }

            path.DrawArrows();

            path.HighlightValidMoves(Color.yellow);
            path.HighlightInvalidMoves(Color.red);
        }

        public override void MakeDecision()
        {
            if (TurnEndRequested())
            {
                if (!turnEndConditions.Met()) { return; }

                SwitchState(factory.TurnEnd());
                return;
            }

            if (SkipMovementRequested())
            {
                if (!skipMovementConditions.Met()) { return; }

                SwitchState(factory.ActionSelection());
                return;
            }

            if (ConfirmPathRequested())
            {
                if (!confirmPathConditions.Met()) { return; }

                SwitchState(factory.MovementConfirmation(path));
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            path.UnDrawAll();
        }


        // Decision
        protected abstract bool TurnEndRequested();
        protected abstract bool SkipMovementRequested();
        protected abstract bool ConfirmPathRequested();


        // Focus
        protected bool TryGetNewFocus(out OverlayTile newFocus)
        {
            newFocus = GetNewFocus() ?? combatant.GetDefaultFocus();

            return newFocus != focusTile;
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
            Vector2Int occupiedPos = (Vector2Int)combatant.CurrentTile.GridLocation;

            // If the character isn't focusing on anything,
            // Then use the position 1 tile in "front" of them,
            // **relative to where the already are**,
            // not forward relative to the board.
            Vector2Int focusPos = (focusTile != null) ?
                (Vector2Int)focusTile.GridLocation
                : combatant.CurrentDirectionContext.ForwardA;

            return new DirectionContext(occupiedPos, focusPos);
        }
    }
}

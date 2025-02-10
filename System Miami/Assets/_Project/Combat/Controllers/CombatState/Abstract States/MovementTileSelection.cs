using System.Collections.Generic;
using System.Linq;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace SystemMiami.CombatRefactor
{
    // waits for input
    public abstract class MovementTileSelection : CombatantState
    {
        int currentSpeedStat;

        // Pathing
        protected MovementPath path;

        protected Conditions confirmPathConditions = new();

        protected MovementTileSelection(Combatant combatant)
            : base(combatant, Phase.Movement) { }

        public override void OnEnter()
        {
            base.OnEnter();

            /// Store current speed for use in pathing.
            currentSpeedStat = (int)combatant.Speed.Get();

            /// Set conditions for being able to confirm the path.
            confirmPathConditions.Add(() => path != null);
            confirmPathConditions.Add(() => path.ContainsValidMoves);

            /// Subscribe to FocusTile changed events.
            combatant.FocusTileChanged += HandleFocusTileChanged;

            InputPrompts =
                "Hover over a tile to preview movement.\n" +
                "Click to lock in your path.\n" +
                "(You will still be able to change your mind).\n";
        }

        public override void Update()
        {
            combatant.UpdateFocus();
            combatant.UpdateAnimDirection();
        }

        public override void MakeDecision()
        {
            if (TurnEndRequested())
            {
                SwitchState(factory.TurnEnd());

                return;
            }

            if (SkipMovementRequested())
            {
                Debug.LogWarning($"Skip movement requested", combatant);
                SwitchState(factory.ActionSelection());
                return;
            }

            if (ConfirmPathRequested())
            {
                if (!confirmPathConditions.AllMet()) { return; }

                SwitchState(factory.MovementConfirmation(path));
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            path?.UnDrawAll();

            combatant.FocusTileChanged -= HandleFocusTileChanged;
        }


        // Decision
        protected abstract bool TurnEndRequested();
        protected abstract bool SkipMovementRequested();
        protected abstract bool ConfirmPathRequested();

        protected virtual void HandleFocusTileChanged(
            object sender,
            FocusTileChangedEventArgs args)
        {
            //args.previousTile?.EndHover(combatant);
            //args.newTile?.BeginHover(combatant);

            // If there is already a
            // path set, unhighlight it.
            path?.Unhighlight();

            // Generate a path based on 
            path = new(
                MapManager.MGR.map[args.directionContext.TilePositionA],
                MapManager.MGR.map[args.directionContext.TilePositionB],
                currentSpeedStat
                );


            if (!path.ContainsValidMoves) { return; }

            path.DrawArrows();

            path.HighlightValidMoves(Color.yellow);
            path.HighlightInvalidMoves(Color.red);
        }
    }
}

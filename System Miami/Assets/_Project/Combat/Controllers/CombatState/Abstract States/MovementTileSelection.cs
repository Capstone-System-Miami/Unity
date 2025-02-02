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

        // Transitioning
        protected Conditions turnEndConditions = new();
        protected Conditions turnEndRequested = new();

        protected Conditions skipMovementConditions = new();
        protected Conditions confirmPathConditions = new();

        protected MovementTileSelection(Combatant combatant)
            : base(combatant, Phase.Movement) { }

        public override void OnEnter()
        {
            base.OnEnter();
            currentSpeedStat = (int)combatant.Speed.Get();

            confirmPathConditions.Add(() => path != null);
            confirmPathConditions.Add(() => !path.IsEmpty);

            combatant.FocusTileChanged += HandleFocusTileChanged;
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


            if (path.IsEmpty) { return; }

            path.DrawArrows();

            path.HighlightValidMoves(Color.yellow);
            path.HighlightInvalidMoves(Color.red);
        }
    }
}

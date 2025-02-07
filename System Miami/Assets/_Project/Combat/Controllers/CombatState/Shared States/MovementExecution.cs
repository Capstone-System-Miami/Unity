using System.Collections.Generic;
using System.Linq;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class MovementExecution : CombatantState
    {
        protected Conditions moveAgainConditions = new();
        protected Conditions actionSelectionConditions = new();

        protected MovementPath path;
        protected List<OverlayTile> pathToConsume = new();

        public MovementExecution(
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
            base.OnEnter();

            /// Display whatever part of the path we want to here
            path.HighlightValidMoves(Color.green);
            path.DrawArrows();

            /// Copy path so we can remove elements as we go.
            pathToConsume = new(path.ForMovement);

            /// Set conditions for moving again when the
            /// path is used up.
            moveAgainConditions.Add( () => !pathToConsume.Any() );
            moveAgainConditions.Add( () => combatant.Speed.Get() > 0 );

            /// Set conditions for changing to action
            /// selection state
            actionSelectionConditions.Add( () => !pathToConsume.Any() );

            InputPrompts = 
                "Executing Movement.\n";
        }

        public override void Update()
        {
            if (!pathToConsume.Any())
                { Debug.Log($"{combatant.name} no path"); return; }

            combatant.FocusTile = pathToConsume[0];

            combatant.StepTowards(combatant.FocusTile);

            if (combatant.InPlacementRangeOf(combatant.FocusTile))
            {
                if (!MapManager.MGR.TryPlaceOnTile(combatant, combatant.FocusTile))
                {
                    Debug.LogError(
                        $"{combatant.gameObject} " +
                        $"was not able to be placed on" +
                        $"{combatant.FocusTile.gameObject}.");
                    return;
                }

                pathToConsume.RemoveAt(0);
                combatant.Speed.Lose(1);

                Debug.Log(
                    $"{combatant} moved along path. " +
                    $"new speed: {combatant.Speed.Get()}"
                    );
            }
        }

        public override void MakeDecision()
        {
            if (pathToConsume.Any()) { return; }

            if (moveAgainConditions.AllMet())
            {
                SwitchState(factory.MovementTileSelection());
                return;
            }
            else if (actionSelectionConditions.AllMet())
            {
                SwitchState(factory.ActionSelection());
                return;
            }
        }

        public override void OnExit()
        {
            path.UnDrawAll();
        }
    }
}

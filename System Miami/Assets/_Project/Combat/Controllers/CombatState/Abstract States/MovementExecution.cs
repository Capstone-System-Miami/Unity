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
        protected OverlayTile occupiedTile;
        protected OverlayTile focusTile;
        protected OverlayTile destinationTile;

        protected float distanceToTarget;

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
            pathToConsume = new(path.ForMovement);

            moveAgainConditions.Add( () => !pathToConsume.Any() );
            moveAgainConditions.Add( () => combatant.Speed.Get() > 0 );
        }

        public override void Update()
        {
            if (!pathToConsume.Any())
                { Debug.Log($"{combatant.name} no path"); return; }

            occupiedTile = combatant.CurrentTile;
            focusTile = pathToConsume[0];

            combatant.CurrentDirectionContext = new(
                (Vector2Int)occupiedTile.GridLocation,
                (Vector2Int)focusTile.GridLocation
                );

            combatant.StepTowards(focusTile);

            if (combatant.InPlacementRangeOf(focusTile))
            {
                combatant.SnapTo(focusTile);
                focusTile.PlaceCombatant(combatant);

                pathToConsume.RemoveAt(0);

                combatant.Speed.Lose(1);
                Debug.Log(
                    $"{combatant} new tile snap," +
                    $"new speed: {combatant.Speed.Get()}"
                    );
            }
        }

        public override void MakeDecision()
        {
            if (pathToConsume.Any()) { return; }

            if (moveAgainConditions.Met())
            {
                SwitchState(factory.MovementTileSelection());
                return;
            }
            else if (actionSelectionConditions.Met())
            {
                SwitchState(factory.ActionSelection());
                return;
            }
            else
            {
                SwitchState(factory.TurnEnd());
            }
        }

        public override void OnExit()
        {
            path.UnDrawAll();
        }
    }
}

using System.Collections.Generic;
using SystemMiami.CombatSystem;
using System.Linq;
using SystemMiami.Management;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class ForcedMovementExecution : CombatantState
    {
        protected MovementPath path;
        protected List<OverlayTile> pathToConsume = new();

        public ForcedMovementExecution(
            Combatant combatant,
            MovementPath movementPath)
                : base(
                    combatant,
                    Phase.None
                )
        {
            this.path = movementPath;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            /// Display whatever part of the path we want to here
            path.HighlightValidMoves(Color.red);
            path.DrawArrows();

            /// Copy path so we can remove elements as we go.
            pathToConsume = new(path.ForMovement);
        }

        public override void Update()
        {
            if (!pathToConsume.Any()) { return; }

            combatant.StepTowards(pathToConsume[0]);

            if (combatant.InPlacementRangeOf(pathToConsume[0]))
            {
                if (!MapManager.MGR.TryPlaceOnTile(combatant, pathToConsume[0]))
                {
                    Debug.LogError(
                        $"{combatant.gameObject} " +
                        $"was not able to be placed on" +
                        $"{pathToConsume[0].gameObject}.");
                    return;
                }

                pathToConsume.RemoveAt(0);

                Debug.Log(
                    $"{combatant} moved along path. " +
                    $"new speed: {combatant.Speed.Get()}");
            }
        }

        public override void MakeDecision()
        {
            if (pathToConsume.Any()) { return; }

            SwitchState(combatant.Factory.Idle());
        }

        public override void OnExit()
        {
            path.UnDrawAll();

            /// Set the animator back to idle.
            combatant.Animator.runtimeAnimatorController
                = combatant.AnimControllerIdle;
        }
    }
}

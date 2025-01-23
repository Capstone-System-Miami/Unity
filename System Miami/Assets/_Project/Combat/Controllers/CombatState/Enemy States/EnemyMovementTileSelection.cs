using System.Collections.Generic;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class EnemyMovementTileSelection : MovementTileSelection
    {
        int detectionRadius= 3;

        public EnemyMovementTileSelection(CombatantStateMachine machine)
            : base(machine) { }

        // Decision
        protected override bool SelectTile()
        {
            return true;
        }

        protected override bool SkipPhase()
        {
            // TODO:
            // If the phase has to be skipped
            // because of some status effect,
            // this is where that would happen.
            return false;
        }

        // Decision outcomes
        protected override void GoToActionSelection()
        {
            machine.SwitchState(
                new EnemyActionSelection(
                    machine
                    )
                );
            return;
        }

        protected override void GoToTileConfirmation()
        {
            machine.SwitchState(
                new EnemyMovementTileConfirmation(
                    machine,
                    newPath,
                    arrowPath
                    )
                );
            return;
        }


        protected override OverlayTile GetNewFocus()
        {
            Combatant targetPlayer = TurnManager.MGR.playerCharacter;

            if (IsInDetectionRange(targetPlayer))
            {
                Debug.Log($"Player found in {name}'s range");
                return targetPlayer.CurrentTile;
            }
            else
            {
                Debug.Log($"Player not found in {name}'s range." +
                    $"Getting random tile");
                return MapManager.MGR.GetRandomUnblockedTile();
            }
        }

        private bool IsInDetectionRange(Combatant target)
        {
            List<OverlayTile> path = getPathTo(target.CurrentTile);

            if (path.Count <= detectionRadius)
            {
                return true;
            }

            return false;
        }
    }
}

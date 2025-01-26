using System.Collections.Generic;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class EnemyMovementTileSelection : MovementTileSelection
    {
        int detectionRadius= 3;

        public EnemyMovementTileSelection(Combatant combatant)
            : base(combatant) { }

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
            machine.SetState(new EnemyActionSelection(combatant));
        }

        protected override void GoToTileConfirmation()
        {
            machine.SetState(new EnemyActionConfimation(combatant));
        }


        protected override OverlayTile GetNewFocus()
        {
            Combatant targetPlayer = TurnManager.MGR.playerCharacter;

            if (IsInDetectionRange(targetPlayer))
            {
                Debug.Log($"Player found in {combatant.name}'s range");
                return targetPlayer.CurrentTile;
            }
            else
            {
                Debug.Log($"Player not found in {combatant.name}'s range." +
                    $"Getting random tile");
                return MapManager.MGR.GetRandomUnblockedTile();
            }
        }

        private bool IsInDetectionRange(Combatant target)
        {
            MovementPath fullPathToPlayer = new(
                combatant.CurrentTile,
                target.CurrentTile
                );

            if (fullPathToPlayer.ForMovement.Count <= detectionRadius)
            {
                return true;
            }

            return false;
        }
    }
}

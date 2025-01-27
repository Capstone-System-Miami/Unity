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
        protected override bool TurnEndRequested()
        {
            return false;
        }

        protected override bool SkipMovementRequested()
        {
            return false;
        }

        protected override bool ConfirmPathRequested()
        {
            return true;
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

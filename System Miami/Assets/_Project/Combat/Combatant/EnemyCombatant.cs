using UnityEngine;

// Authors: Layla Hoey, Lee St Louis
namespace SystemMiami.CombatSystem
{
    public class EnemyCombatant : Combatant
    {
        [SerializeField] private int detectionRadius = 3;

        public override OverlayTile GetNewFocus()
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

        public bool IsInDetectionRange(Combatant target)
        {
            MovementPath fullPathToPlayer = new(
                CurrentTile,
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

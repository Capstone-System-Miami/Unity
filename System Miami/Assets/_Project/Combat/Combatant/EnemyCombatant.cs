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
                return targetPlayer.PositionTile;
            }
            else
            {
                Debug.Log($"Player not found in {name}'s range." +
                    $"Getting random tile");
                return MapManager.MGR.GetRandomValidTile();
            }
        }

        public bool IsInDetectionRange(Combatant target)
        {
            MovementPath fullPathToPlayer = new(
                PositionTile,
                target.PositionTile
                );

            if (fullPathToPlayer.ForMovement.Count <= detectionRadius)
            {
                return true;
            }

            return false;
        }
    }
}

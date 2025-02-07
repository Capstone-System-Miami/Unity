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
            MovementPath pathToPlayerData = new(
                PositionTile,
                target.PositionTile
                );

            return pathToPlayerData.ForMovement.Count <= detectionRadius;
        }
    }
}

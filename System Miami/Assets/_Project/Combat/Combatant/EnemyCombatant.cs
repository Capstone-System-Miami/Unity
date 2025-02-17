using System.Collections.Generic;
using SystemMiami.CombatRefactor;
using UnityEngine;

// Authors: Layla Hoey, Lee St Louis
namespace SystemMiami.CombatSystem
{
    public class EnemyCombatant : Combatant
    {
        [SerializeField] private int detectionRadius = 3;

        [HideInInspector] public bool PlayerInRange;

        [Header("CombatAction Presets")]
        [SerializeField] private List<NewAbilitySO> physicalSOs;
        [SerializeField] private List<NewAbilitySO> magicalSOs;
        [SerializeField] private List<ConsumableSO> consumableSOs;
       
        public override OverlayTile GetNewFocus()
        {
            Combatant targetPlayer = TurnManager.MGR.playerCharacter;
            PlayerInRange = IsInDetectionRange(targetPlayer);

            if (PlayerInRange)
            {
                Debug.Log($"Player found in {name}'s range", this);
                return targetPlayer.PositionTile;
            }
            else
            {
                Debug.Log($"Player not found in {name}'s range." +
                    $"Getting random tile", this);
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

        protected override void InitLoadout()
        {
            Loadout = new(_inventory, this);
        }
    }
}

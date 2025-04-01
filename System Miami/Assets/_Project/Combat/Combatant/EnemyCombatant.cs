using System.Collections.Generic;
using System.ComponentModel.Design;
using SystemMiami.Animation;
using SystemMiami.CombatRefactor;
using SystemMiami.Dungeons;
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


        protected override void Start()
        {
            base.Start();
            int playerLevel = PlayerManager.MGR.GetPlayerLevel();
            DifficultyLevel difficultyLevel = MapManager.MGR.Dungeon.DifficultyLevel;
            GetComponent<EnemiesLevel>().Initialize(difficultyLevel, playerLevel);
        }
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
                target.PositionTile,
                false
            );

            return pathToPlayerData.ForMovement.Count <= detectionRadius;
        }

        protected override void InitLoadout()
        {
            Loadout = new(_inventory, this);
        }
    }
}

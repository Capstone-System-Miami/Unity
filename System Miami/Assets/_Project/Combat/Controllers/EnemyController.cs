// Authors: Layla, Lee
using System.Collections;
using System.Collections.Generic;
using SystemMiami.AbilitySystem;
using SystemMiami.CombatSystem;
using SystemMiami.Enums;
using UnityEngine;

namespace SystemMiami
{
    public class EnemyController : CombatantController
    {
        [SerializeField] private int detectionRadius = 2;

        #region Flags
        private bool FLAG_EndTurn;
        private bool FLAG_NextPhase;
        private bool FLAG_BeginMovement;

        private bool FLAG_Unequip;
        private bool FLAG_Equip;
        private bool FLAG_LockTargets;
        private bool FLAG_UseAbility;

        #endregion


        #region Turn Management
        // ======================================
        public override void StartTurn()
        {
            FocusedTile = null;

            base.StartTurn();

            StartCoroutine(TurnBehavior());
        }

        private IEnumerator TurnBehavior()
        {
            yield return new WaitForSeconds(.5f);

            // Movement
            FLAG_BeginMovement = true;

            yield return new WaitUntil(() => DestinationReached());
            yield return new WaitForSeconds(.5f);

            FLAG_NextPhase = true;
            yield return null;

            // Equip Ability
            // TODO use selectAbility() once that logic is figured out.
            TypeToEquip = AbilityType.PHYSICAL;
            IndexToEquip = 0;
            Debug.Log($"{name} starting equip cor.");
            yield return StartCoroutine(equipAbility());
            yield return null;

            // Can see player, check every direction rapidly.
            if (IsInDetectionRange(TurnManager.MGR.playerCharacter))
            {
                FocusedTile = TurnManager.MGR.playerCharacter.CurrentTile;
                yield return null;
                FocusedTileChanged?.Invoke(FocusedTile);

                for (int i = 0; i < System.Enum.GetValues(typeof(TileDir)).Length; i++)
                {
                    FocusedTileChanged?.Invoke(FocusedTile);

                    yield return new WaitForSeconds(.25f);

                    // If the player is found in Ability's targets,
                    // lock the ability and hold for a moment, then execute.
                    if (combatant.Abilities.SelectedAbility.PlayerFoundInTargets)
                    {
                        yield return StartCoroutine(lockAbility());

                        yield return new WaitForSeconds(.5f);

                        yield return StartCoroutine(executeAbility());
                        break;
                    }

                    // Turn Clockwise 45degrees
                    AdjacentPositionSet adj = new AdjacentPositionSet(combatant.DirectionInfo);
                    Vector2Int newMapPosToFace = adj.AdjacentPositions[(TileDir)1];

                    if (MapManager.MGR.map.TryGetValue(newMapPosToFace, out OverlayTile newTileToFace))
                    {
                        FocusedTile = newTileToFace;
                    }
                }
            }
            // Can't see player, but check some random directions just in case,
            // and for the visual element.
            else
            {
                float lookAroundDuration = 2f;
                float changeTargetInterval = .5f;
                while (lookAroundDuration > 0)
                {
                    FocusedTile = MapManager.MGR.GetRandomUnblockedTile();                    
                    FocusedTileChanged?.Invoke(FocusedTile);

                    if (combatant.Abilities.SelectedAbility.PlayerFoundInTargets)
                    {
                        yield return StartCoroutine(lockAbility());

                        yield return new WaitForSeconds(.5f);

                        yield return StartCoroutine(executeAbility());
                        break;
                    }

                    lookAroundDuration -= changeTargetInterval;
                    yield return new WaitForSeconds(changeTargetInterval);
                }

            }

            yield return StartCoroutine(unequipAbility());

            yield return new WaitForSeconds(1f);
            FLAG_EndTurn = true;
        }
        // ======================================
        #endregion // Turn Management ===========


        #region Triggers
        // ======================================

        public override bool EndTurnTriggered()
        {
            return FLAG_EndTurn;
        }

        public override bool NextPhaseTriggered()
        {
            return FLAG_NextPhase;
        }

        public override bool BeginMovementTriggered()
        {
            return FLAG_BeginMovement;
        }

        public override bool UnequipTriggered()
        {
            return FLAG_Unequip;
        }

        public override bool EquipTriggered()
        {
            return FLAG_Equip;
        }

        public override bool LockTargetsTriggered()
        {
            return FLAG_LockTargets;
        }

        public override bool UseAbilityTriggered()
        {
            return FLAG_UseAbility;
        }

        public override void ResetFlags()
        {
            FLAG_EndTurn = false;
            FLAG_NextPhase = false;
            FLAG_BeginMovement = false;
            FLAG_Unequip = false;
            FLAG_Equip = false;
            FLAG_LockTargets = false;
            FLAG_UseAbility = false;
        }

        // ======================================
        #endregion // Triggers ==================


        #region Focused Tile
        // ======================================

        public override OverlayTile GetFocusedTile()
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
        // ======================================
        #endregion // Focused Tile ==============


        #region Movement

        /// <summary>
        /// Gets walkable neighbor tiles for random movement.
        /// </summary>
        private List<OverlayTile> GetWalkableNeighbourTiles()
        {
            List<OverlayTile> walkableTiles = new List<OverlayTile>();

            List<OverlayTile> neighbours = PathFinder.GetNeighbourTiles(combatant.CurrentTile);

            foreach (OverlayTile tile in neighbours)
            {
                if (tile.ValidForPlacement)
                {
                    walkableTiles.Add(tile);
                }
            }

            return walkableTiles;
        }
        #endregion


        #region Abilities
        private IEnumerator equipAbility()
        {
            FLAG_Equip = true;
            Debug.Log($"{name} waiting for equip");
            yield return new WaitUntil(() => combatant.Abilities.CurrentState == Abilities.State.EQUIPPED);
        }

        private IEnumerator unequipAbility()
        {
            FLAG_Unequip = true;
            Debug.Log($"{name} waiting for unequip");
            yield return new WaitUntil(() => combatant.Abilities.CurrentState == Abilities.State.UNEQUIPPED);
        }

        private IEnumerator lockAbility()
        {
            FLAG_LockTargets = true;
            Debug.Log($"{name} waiting for targ lock");
            yield return new WaitUntil(() => combatant.Abilities.CurrentState == Abilities.State.TARGETS_LOCKED);
        }

        private IEnumerator executeAbility()
        {
            FLAG_UseAbility = true;
            Debug.Log($"{name} waiting for execute");
            yield return new WaitUntil(() => combatant.Abilities.CurrentState == Abilities.State.EXECUTING);

            Debug.Log($"{name} waiting for complete");
            yield return new WaitUntil(() => combatant.Abilities.CurrentState == Abilities.State.COMPLETE);
        }
        #endregion


        #region Detection
        private bool IsInDetectionRange(Combatant target)
        {
            //int distance = Mathf.Abs(combatant.CurrentTile.gridLocation.x - target.CurrentTile.gridLocation.x) +
            //   Mathf.Abs(combatant.CurrentTile.gridLocation.y - target.CurrentTile.gridLocation.y);

            List<OverlayTile> path = getPathTo(target.CurrentTile);

            if (path.Count <= detectionRadius)
            {
                return true;
            }

            return false;
        }
        #endregion
    }
}

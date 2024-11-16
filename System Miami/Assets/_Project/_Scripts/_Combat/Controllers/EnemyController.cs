// Authors: Layla, Lee
using System.Collections;
using System.Collections.Generic;
using SystemMiami.AbilitySystem;
using SystemMiami.CombatSystem;
using SystemMiami.Enums;
using SystemMiami.Utilities;
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

        protected override void OnNextPhaseFailed()
        {
            base.OnNextPhaseFailed();
            FLAG_EndTurn = true;
        }

        private IEnumerator TurnBehavior()
        {
            yield return new WaitForSeconds(.5f);

            // Movement
            FLAG_BeginMovement = true;
            yield return new WaitUntil(() => destinationReached());
            yield return new WaitForSeconds(.5f);

            FLAG_NextPhase = true;
            yield return null;

            // Equip Ability
            // TODO use selectAbility() once that logic is figured out.
            typeToEquip = AbilityType.PHYSICAL;
            indexToEquip = 0;
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

                    yield return new WaitForSeconds(1f);

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

        protected override bool endTurnTriggered()
        {
            return FLAG_EndTurn;
        }

        protected override bool nextPhaseTriggered()
        {
            return FLAG_NextPhase;
        }

        protected override bool beginMovementTriggered()
        {
            return FLAG_BeginMovement;
        }


        protected override bool unequipTriggered()
        {
            return FLAG_Unequip;
        }

        protected override bool equipTriggered()
        {
            return FLAG_Equip;
        }

        protected override bool lockTargetsTriggered()
        {
            return FLAG_LockTargets;
        }

        protected override bool useAbilityTriggered()
        {
            return FLAG_UseAbility;
        }

        protected override void resetFlags()
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


        #region Phase Handling
        #endregion


        #region Focused Tile
        // ======================================

        protected override void resetFocusedTile()
        {
            FocusedTile = MapManager.MGR.GetRandomUnblockedTile();

            FocusedTileChanged?.Invoke(FocusedTile);
        }

        protected override void updateFocusedTile()
        {
            if (FocusedTile != null) { return; }

            FocusedTile = getFocusedTile();

            FocusedTileChanged?.Invoke(FocusedTile);
        }

        protected override OverlayTile getFocusedTile()
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
        /// Coroutine for combatant random movement when not chasing the player.
        /// </summary>
        private IEnumerator RandomMove()
        {
            //while ((int)combatant.Speed.Get() > 0)
            //{
            //    // Get walkable neighbor tiles
            //    List<OverlayTile> walkableTiles = GetWalkableNeighbourTiles();

            //    if (walkableTiles.Count == 0)
            //    {
            //        // No walkable tiles available
            //        break;
            //    }

            //    // Decrement speed
            //    combatant.Speed.Lose(1);

            //    // Choose a random tile
            //    int index = UnityEngine.Random.Range(0, walkableTiles.Count);
            //    OverlayTile tile = walkableTiles[index];

            //    // Update tiles' currentCharacter
            //    combatant.CurrentTile.currentCharacter = null;
            //    combatant.CurrentTile = tile;
            //    tile.currentCharacter = combatant;

            //    // Move combatant's position
            //    combatant.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);
            //    //combatant.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;

            //    yield return new WaitForSeconds(0.2f); // Wait for movement simulation
            //}

            yield return null;
        }

        /// <summary>
        /// Gets walkable neighbor tiles for random movement.
        /// </summary>
        private List<OverlayTile> GetWalkableNeighbourTiles()
        {
            List<OverlayTile> walkableTiles = new List<OverlayTile>();

            List<OverlayTile> neighbours = pathFinder.GetNeighbourTiles(combatant.CurrentTile);

            foreach (OverlayTile tile in neighbours)
            {
                if (!tile.isBlocked && tile.currentCharacter == null)
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

        ///// <summary>
        ///// Selects an ability for the enemy.
        ///// </summary>
        //private Ability selectAbility()
        //{
        //    // Select the first ability that can be used
        //    foreach (Ability ability in combatant.Abilities.Physical)
        //    {
        //        if (!ability.isOnCooldown)
        //        {
        //            // Check if any target is in range
        //            if (IsPlayerInAbilityRange(ability))
        //            {
        //                return ability;
        //            }
        //        }
        //    }

        //    foreach (Ability ability in combatant.Abilities.Magical)
        //    {
        //        if (!ability.isOnCooldown)
        //        {
        //            // Check if any target is in range
        //            if (IsPlayerInAbilityRange(ability))
        //            {
        //                return ability;
        //            }
        //        }
        //    }
        //    return null; // No ability can be used
        //}

        ///// <summary>
        ///// Currently only returns true
        ///// TODO:
        ///// Should check if any player is within the ability's range.
        ///// </summary>
        //private bool IsPlayerInAbilityRange(Ability ability)
        //{
        //    //int maxRange = 2; // You can adjust this or use ability.range if available

        //    //Combatant player = TurnManager.MGR.playerCharacter;


        //    //int distance = Mathf.Abs(combatant.CurrentTile.gridLocation.x - player.CurrentTile.gridLocation.x) +
        //    //               Mathf.Abs(combatant.CurrentTile.gridLocation.y - player.CurrentTile.gridLocation.y);

        //    //if (distance <= maxRange)
        //    //{
        //    //    return true;
        //    //}

        //    //return false;

        //    return true;
        //}
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

        ///// <summary>
        ///// Finds the nearest player character to the enemy.
        ///// </summary>
        //private Combatant FindNearestPlayer()
        //{
        //    Combatant nearestPlayer = null;
        //    int shortestDistance = int.MaxValue;

        //    foreach (Combatant player in TurnManager.MGR.playerCharacter)
        //    {
        //        int distance = Mathf.Abs(enemy.CurrentTile.gridLocation.x - player.CurrentTile.gridLocation.x) +
        //                       Mathf.Abs(enemy.CurrentTile.gridLocation.y - player.CurrentTile.gridLocation.y);

        //        if (distance < shortestDistance)
        //        {
        //            shortestDistance = distance;
        //            nearestPlayer = player;
        //        }
        //    }

        //    return nearestPlayer;
        //}

        ///// <summary>
        ///// Finds the nearest player character within a given radius of the enemy.
        ///// </summary>
        //private Combatant FindNearestPlayerWithinRadius(int radius)
        //{
        //    Combatant nearestPlayer = null;
        //    int shortestDistance = int.MaxValue;

        //    foreach (Combatant player in TurnManager.MGR.playerCharacter)
        //    {
        //        int distance = Mathf.Abs(enemy.CurrentTile.gridLocation.x - player.CurrentTile.gridLocation.x) +
        //                       Mathf.Abs(enemy.CurrentTile.gridLocation.y - player.CurrentTile.gridLocation.y);

        //        if (distance <= radius && distance < shortestDistance)
        //        {
        //            shortestDistance = distance;
        //            nearestPlayer = player;
        //        }
        //    }

        //    return nearestPlayer;
        //}

        //private void OnDestroy()
        //{
        //    // Remove the enemy from the TurnManager's list when destroyed
        //    TurnManager.MGR.enemyCharacters.Remove(enemy);
        //}
        #endregion
    }
}

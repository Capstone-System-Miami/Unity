using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami
{
    public class EnemyController_v2 : CombatantController
    {
        [SerializeField] private int detectionRadius = 2;

        #region Flags
        private bool FLAG_EndTurn;
        private bool FLAG_NextPhase;
        private bool FLAG_BeginMovement;
        private bool FLAG_TakeAction;
        #endregion


        #region Unity
        // ======================================

        protected override void LateUpdate()
        {
            base.LateUpdate();

            resetFlags();
        }

        private void OnDestroy()
        {
            // Remove the enemy from the TurnManager's list when destroyed
            TurnManager.MGR.enemyCharacters.Remove(combatant);
        }

        // ======================================
        #endregion // Unity =====================


        #region Turn Management
        // ======================================

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

        protected override bool useAbilityTriggered()
        {
            return FLAG_TakeAction;
        }

        private void resetFlags()
        {
            FLAG_EndTurn = false;
            FLAG_NextPhase = false;
            FLAG_BeginMovement = false;
        }

        // ======================================
        #endregion // Triggers ==================


        #region Focused Tile
        // ======================================

        protected override void resetFocusedTile()
        {
            FocusedTile = MapManager.MGR.GetRandomUnblockedTile();
        }

        protected override OverlayTile getFocusedTile()
        {
            Combatant targetPlayer = TurnManager.MGR.playerCharacter;

            // If player is not in range, don't change focus.
            if (IsInDetectionRange(targetPlayer))
            {
                return targetPlayer.CurrentTile;
            }
            else
            {
                return MapManager.MGR.GetRandomUnblockedTile();
            }
        }
        // ======================================
        #endregion // Focused Tile ==============

        protected override void useAbility()
        {
            HasActed = true;
        }

        private bool IsInDetectionRange(Combatant target)
        {
            int distance = Mathf.Abs(combatant.CurrentTile.gridLocation.x - target.CurrentTile.gridLocation.x) +
               Mathf.Abs(combatant.CurrentTile.gridLocation.y - target.CurrentTile.gridLocation.y);

            if (distance <= detectionRadius)
            {
                return true;
            }

            return false;
        }

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

        public override void StartTurn()
        {
            base.StartTurn();
            
            if (!IsMyTurn)
                { return; }

            StartCoroutine(TakeTurn());
        }

        private IEnumerator TakeTurn()
        {
            yield return new WaitForSeconds(.5f);

            FLAG_BeginMovement = true;
            yield return new WaitUntil(() => destinationReached());
            yield return new WaitForSeconds(.5f);

            FLAG_NextPhase = true;
            yield return null;

            FLAG_TakeAction = true;
            yield return new WaitUntil(() => HasActed);
            yield return new WaitForSeconds(.5f);

            FLAG_EndTurn = true;
        }
    }
}

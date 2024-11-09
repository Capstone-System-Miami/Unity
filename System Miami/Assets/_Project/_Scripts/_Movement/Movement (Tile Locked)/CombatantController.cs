using System;
using System.Collections.Generic;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatSystem

{
    // An abstract class that PlayerController and
    // EnemyController controller can derive from.
    public abstract class CombatantController : MonoBehaviour
    {
        public float movementSpeed;

        protected Combatant combatant;

        protected PathFinder pathFinder;
        protected List<OverlayTile> path = new List<OverlayTile>();
        protected int currentPathCost;

        public OverlayTile FocusedTile { get; protected set; }

        public Action<OverlayTile> FocusedTileChanged;
        public Action<DirectionalInfo> PathTileChanged;

        public bool IsMoving { get; protected set; }

        public Phase TurnPhase { get; protected set; }

        #region Unity
        private void Start()
        {
            pathFinder = new PathFinder();

            if (!TryGetComponent(out combatant))
            {
                print($"Didnt find a Combatant component on {name}.");
            }
        }

        private void Update()
        {
            if (!IsMyTurn()) { return; }

            updateFocusedTile();
        }

        private void LateUpdate()
        {
            if (!IsMyTurn()) { return; }

            // Q To end turn
            if (Input.GetKeyDown(KeyCode.Q))
            {
                TurnManager.Instance.EndPlayerTurn();
            }

            handleMovementPhase();
            moveAlongPath();

            handleActionPhase();
        }
        #endregion Unity

        #region Focused Tile
        protected void updateFocusedTile()
        {
            OverlayTile newFocus = getFocusedTile();

            if (newFocus == null) { return; }

            if (newFocus == FocusedTile) { return; }

            FocusedTile = newFocus;

            // Raise event when mouse tile  changes
            FocusedTileChanged?.Invoke(newFocus);
        }
        protected abstract void resetFocusedTile();
        protected abstract OverlayTile getFocusedTile();
        #endregion Focused Tile

        #region Phase Handling
        protected virtual void handleMovementPhase()
        {
            if (TurnManager.Instance.currentPhase != Phase.MovementPhase) { return; }
        }
        protected virtual void handleActionPhase()
        {
            if (TurnManager.Instance.currentPhase != Phase.ActionPhase) { return; }
        }
        #endregion Phase Handling

        #region Movement
        protected void createPathTo(OverlayTile tile)
        {
            // Calculate path
            List<OverlayTile> pathToTry = pathFinder.FindPath(combatant.CurrentTile, tile);

            // Check if path length is within movement points
            if (pathToTry.Count <= combatant.Speed.Get())
            {
                // If so, set global vars.
                path = pathToTry;
                currentPathCost = pathToTry.Count;

                // Subtract movement points
                //combatant.Speed.Lose(path.Count);

                // Start moving along path
                // Note: We should not call MoveAlongPath() here; instead, we should let Update() handle the movement
            }
            else
            {
                // Not enough movement points
                Debug.Log("Not enough movement points to move to that tile.");
            }
        }

        /// <summary>
        /// Moves the character along the calculated path.
        /// </summary>
        private void moveAlongPath()
        {
            if (path.Count <= 0 || combatant.Speed.Get() == 0)
            {
                IsMoving = false;
                return;
            }

            IsMoving = true;

            float step = movementSpeed * Time.deltaTime;

            //TODO
            //i want to add it here so that you have to
            //confirm the movement too so that the arrows
            //show up before you move and can show your path
            OverlayTile targetTile = path[0];
            float zIndex = targetTile.transform.position.z;
            combatant.transform.position = Vector2.MoveTowards(combatant.transform.position, targetTile.transform.position, step);
            combatant.transform.position = new Vector3(combatant.transform.position.x, combatant.transform.position.y, zIndex);

            // If character is close enough to a new tile
            if (Vector2.Distance(combatant.transform.position, targetTile.transform.position) < 0.0001f)
            {
                // Directional info based on the current tile
                // and the one we're moving to.
                DirectionalInfo newDir = new DirectionalInfo((Vector2Int)combatant.CurrentTile.gridLocation, (Vector2Int)targetTile.gridLocation);

                // Let any subscribers know that we are moving along path
                PathTileChanged(newDir);

                positionCharacterOnTile(targetTile);
                path.RemoveAt(0);
            }
        }

        /// <summary>
        /// Positions the character on the specified tile.
        /// </summary>
        private void positionCharacterOnTile(OverlayTile tile)
        {
            combatant.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);

            //character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;

            // Update CurrentTile
            combatant.CurrentTile = tile;

            // Set tile's currentCharacter
            tile.currentCharacter = combatant;
        }
        #endregion Movement

        #region General
        public bool IsMyTurn()
        {
            // TODO: Ask turn manager
            return TurnManager.Instance.isPlayerTurn;
        }

        protected virtual void onStartTurn()
        {
        }

        protected virtual void onNewPhase()
        {
        }

        protected virtual void onEndTurn()
        {
        }
        #endregion General
    }
}

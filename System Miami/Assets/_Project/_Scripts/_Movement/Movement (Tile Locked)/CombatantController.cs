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
        [SerializeField] protected float movementSpeed;

        protected Combatant combatant;

        protected PathFinder pathFinder;
        protected List<OverlayTile> currentPath = new List<OverlayTile>();
        protected int currentPathCost;

        public OverlayTile FocusedTile { get; protected set; }
        public OverlayTile DestinationTile { get; protected set; }

        public Action<OverlayTile> FocusedTileChanged;
        public Action<DirectionalInfo> PathTileChanged;
        
        public bool IsMoving { get; protected set; }
        public bool IsActing { get; protected set; }
        public bool HasActed { get; protected set; }
        public bool IsMyTurn { get; protected set; }

        public Phase CurrentPhase { get; protected set; }

        protected List<Phase> defaultPhases = new List<Phase>
        {
            Phase.MovementPhase,
            Phase.ActionPhase
        };

        protected List<Phase> remainingPhases = new List<Phase>();

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
            if (!IsMyTurn) { return; }

            updateFocusedTile();
        }

        private void LateUpdate()
        {
            if (!IsMyTurn) { return; }

            if (endTurnTriggered())
            {
                EndTurn();
                return;
            }

            if (nextPhaseTriggered())
            {
                TryNextPhase();
                return;
            }

            switch (CurrentPhase)
            {
                default:
                case Phase.MovementPhase:
                    handleMovementPhase();
                    break;

                case Phase.ActionPhase:
                    handleActionPhase();
                    break;
            }
        }
        #endregion Unity

        #region Triggers
        protected abstract bool endTurnTriggered();
        protected abstract bool nextPhaseTriggered();
        protected abstract bool beginMovementTriggered();
        protected /*abstract*/ bool useAbilityTriggered() { return false; }
        #endregion Triggers

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

        #region Movement Phase
        protected virtual void handleMovementPhase()
        {
            if (CurrentPhase != Phase.MovementPhase) { return; }
            Debug.Log($"{name} Pathcount: {currentPath?.Count}");

            if (beginMovementTriggered())
            {
                Debug.Log($"{name} is beginning movement");
                beginMovement();
            }

            if (!destinationReached())
            {
                moveAlongPath();
            }
            else
            {
                clearMovement();
            }
        }

        /// <summary>
        /// Sets the destination tile.
        /// Validates a path to the destination.
        /// </summary>
        protected virtual void beginMovement()
        {
            DestinationTile = FocusedTile;

            // The exact path to the focused tile.
            // If anyone can think of a use for this, it's here.
            List<OverlayTile> unmodifiedPath = getPathTo(DestinationTile);

            // The path leading up to the tile where the
            // combatant will be out of Speed (movement points)
            List<OverlayTile> truncatedPath = getTruncatedPathTo(DestinationTile);

            if (truncatedPath.Count > 0)
            {
                // New Destination Tile is the
                // last one in the truncated path.
                DestinationTile = truncatedPath[truncatedPath.Count - 1];

                // New path is the truncated one.
                currentPath = truncatedPath;

                // Create a copy of our path that contains the
                // starting position. We can use this modified list
                // To draw the arrows.
                List<OverlayTile> inclusivePath = currentPath;
                inclusivePath.Insert(0, combatant.CurrentTile);
                DrawArrows.Instance.DrawPath(inclusivePath);
            }
            else
            {
                Debug.Log($"{name} has no Speed remaining.");
            }
        }

        /// <summary>
        /// Returns true if there is
        /// no current destination tile,
        /// no current path,
        /// or if the combatant's current tile is the same
        /// as the current destination tile.
        /// </summary>
        /// <returns></returns>
        protected bool destinationReached()
        {
            if (DestinationTile == null)
                { return true; }

            if (currentPath == null)
                { return true; }

            if (currentPath.Count == 0)
                { return true; }

            return combatant.CurrentTile == DestinationTile;
        }

        /// <summary>
        /// Clears destination tile,
        /// current path, and movement bool.
        /// </summary>
        protected void clearMovement()
        {
            DestinationTile = null;
            currentPath = null;
            IsMoving = false;
        }

        /// <summary>
        /// Returns a path to a tile using the pathfinder.
        /// </summary>
        protected List<OverlayTile> getPathTo(OverlayTile tile)
        {
            return pathFinder.FindPath(combatant.CurrentTile, tile);
        }

        /// <summary>
        /// Returns a path to a tile that corresponds to
        /// the combatants current Speed (movement points).
        /// If the combatant's Speed is less than the
        /// tile distance to the destination, it
        /// returns a path that will get them part of the way.
        /// </summary>
        protected List<OverlayTile> getTruncatedPathTo(OverlayTile tile)
        {
            // Make a copy of the path to modify.
            List<OverlayTile> path = getPathTo(tile);

            // Farthest the combatant can move
            // with their current speed points.
            int truncatedLength = (int)combatant.Speed.Get();

            // Get the difference
            int tilesToRemove = path.Count - truncatedLength;

            // If there's a valid difference,
            // remove the rest of the tiles
            if (tilesToRemove > 0)
            {
                path.RemoveRange(truncatedLength, tilesToRemove);
            }

            return path;
        }

        /// <summary>
        /// Moves the character along the calculated path.
        /// </summary>
        private void moveAlongPath()
        {
            if (currentPath == null)
                { return; }

            if (currentPath.Count == 0)
                { return; }

            Debug.Log($"{name} calling move along path and tilecount is {currentPath.Count}");

            IsMoving = true;

            float step = movementSpeed * Time.deltaTime;

            //TODO
            //i want to add it here so that you have to
            //confirm the movement too so that the arrows
            //show up before you move and can show your path
            OverlayTile targetTile = currentPath[0];

            //float zIndex = targetTile.transform.position.z;

            combatant.transform.position = Vector2.MoveTowards(combatant.transform.position, targetTile.transform.position, step);

            //combatant.transform.position = new Vector3(combatant.transform.position.x, combatant.transform.position.y, zIndex);

            // If character is close enough to a new tile
            if (Vector2.Distance(combatant.transform.position, targetTile.transform.position) < 0.0001f)
            {
                // Directional info based on the current tile
                // and the one we're moving to.
                DirectionalInfo newDir = new DirectionalInfo((Vector2Int)combatant.CurrentTile.gridLocation, (Vector2Int)targetTile.gridLocation);

                // Let any subscribers know that we are moving along path
                PathTileChanged(newDir);

                positionCharacterOnTile(targetTile);
                currentPath.RemoveAt(0);
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
        #endregion Movement Phase

        #region Action Phase
        protected virtual void handleActionPhase()
        {
            if (CurrentPhase != Phase.ActionPhase) { return; }

            if (useAbilityTriggered())
            {
                // use ability
            }
        }
        #endregion Action Phase

        #region General
        public virtual void StartTurn()
        {
            remainingPhases = defaultPhases;

            if (TryNextPhase())
            {
                IsMyTurn = true;               
            }
            else
            {
                EndTurn();
            }
        }

        public virtual bool TryNextPhase()
        {
            if (remainingPhases.Count == 0)
                { return false; }

            CurrentPhase = remainingPhases[0];
            remainingPhases.RemoveAt(0);
            TurnManager.Instance.NewTurnPhase(CurrentPhase);
            return true;
        }

        public virtual void EndTurn()
        {
            combatant.ResetTurn();
            // TODO
            // Other end of turn things.
            IsMyTurn = false;
        }
        #endregion General
    }
}

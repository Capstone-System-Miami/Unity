using System;
using System.Collections;
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

        protected PathFinder pathFinder = new PathFinder();
        protected List<OverlayTile> currentPath = new List<OverlayTile>();
        protected int currentPathCost;

        public OverlayTile FocusedTile { get; protected set; }
        public OverlayTile DestinationTile { get; protected set; }

        public Action<OverlayTile> FocusedTileChanged;
        public Action<DirectionalInfo> PathTileChanged;

        #region Properties
        public bool CanMove
        {
            get
            {
                if (combatant == null) { return false; }
                if (CurrentPhase != Phase.Movement) { return false; }
                if (IsMoving) { return false; }

                return combatant.Speed.Get() > 0;
            }
        }

        public bool IsMoving { get; protected set; }

        public bool CanAct
        {
            get
            {
                if (combatant == null) { return false; }
                if (CurrentPhase != Phase.Action) { return false; }
                if (IsActing) { return false; }

                return !HasActed;
            }
        }

        public bool IsActing { get; protected set; }

        public bool HasActed { get; protected set; }

        public bool IsMyTurn { get; protected set; }


        public Phase CurrentPhase { get; protected set; }
        #endregion

        protected List<Phase> defaultPhases = new List<Phase>
        {
            Phase.Movement,
            Phase.Action
        };

        protected List<Phase> remainingPhases = new List<Phase>();

        #region Unity
        // ======================================

        private void Awake()
        {
            combatant = GetComponent<Combatant>();
        }
        private void Start()
        {
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

        protected virtual void LateUpdate()
        {
            if (!IsMyTurn) { return; }

            if (endTurnTriggered())
            {
                EndTurn();
                return;
            }

            if (nextPhaseTriggered())
            {
                if (!TryNextPhase())
                {
                    OnNextPhaseFailed();
                }
                return;
            }

            switch (CurrentPhase)
            {
                case Phase.Movement:
                    handleMovementPhase();
                    break;

                case Phase.Action:
                    handleActionPhase();
                    break;

                default:
                case Phase.None:
                    break;
            }
        }

        // ======================================
        #endregion // Unity =====================


        #region Turn Management
        // ======================================

        public virtual void StartTurn()
        {
            combatant.ResetTurn();
            remainingPhases = defaultPhases;

            IsMyTurn = true;

            if (!TryNextPhase())
            {
                OnNextPhaseFailed();
            }
        }

        public virtual bool TryNextPhase()
        {
            CurrentPhase = Phase.None;

            if (remainingPhases.Count == 0)
                { return false; }

            CurrentPhase = remainingPhases[0];
            remainingPhases.RemoveAt(0);

            TurnManager.MGR.NewTurnPhase(CurrentPhase);

            return true;
        }

        public virtual void OnNextPhaseFailed()
        {
            Debug.Log($"{combatant} is trying" +
                        $"to move to the next phase,\n" +
                        $"But has no phases remaining.");
        }

        public virtual void EndTurn()
        {
            // TODO
            // Other end of turn things.
            IsMyTurn = false;
        }
        // ======================================
        #endregion // Turn Management ===========


        #region Triggers
        // ======================================
        protected abstract bool endTurnTriggered();
        protected abstract bool nextPhaseTriggered();
        protected abstract bool beginMovementTriggered();
        protected abstract bool useAbilityTriggered();

        // ======================================
        #endregion // Triggers ==================


        #region Phase Handling
        // ======================================
        protected virtual void handleMovementPhase()
        {
            if (beginMovementTriggered() && CanMove)
            {
                Debug.Log($"{name} is beginning movement");
                beginMovement();
            }

            if (!destinationReached())
            {
                moveAlongPath();
            }
            else if (DestinationTile != null)
            {
                Debug.Log($"{name} Destination Reached. {DestinationTile?.gridLocation}");

                IsMoving = false;
                clearMovement();
            }
        }

        protected virtual void handleActionPhase()
        {
            if (useAbilityTriggered())
            {
                // use ability
            }
        }

        // ======================================
        #endregion // Phase Handling ============


        #region Focused Tile
        // ======================================
        protected abstract void updateFocusedTile();

        protected abstract void resetFocusedTile();

        protected abstract OverlayTile getFocusedTile();

        // ======================================
        #endregion // Focused Tile ==============


        #region Movement
        // ======================================

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
                { return false; }

            if (currentPath == null)
                { return false; }

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

            //Debug.Log($"{name} calling move along path and tilecount is {currentPath.Count}");

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

                MapManager.MGR.PositionCharacterOnTile(combatant, targetTile);
                currentPath.RemoveAt(0);
            }
        }

        // ======================================
        #endregion // Movement ==================


        #region Abilities
        // ======================================
        protected abstract void useAbility();
        // ======================================
        #endregion // Abilities =================


        #region Detection
        // ======================================

        /// <summary>
        /// Finds the nearest combatant to the enemy.
        /// </summary>
        protected Combatant FindNearestPlayer(List<Combatant> toCheck)
        {
            Combatant nearestPlayer = null;
            int shortestDistance = int.MaxValue;

            foreach (Combatant player in toCheck)
            {
                int distance = Mathf.Abs(combatant.CurrentTile.gridLocation.x - player.CurrentTile.gridLocation.x) +
                               Mathf.Abs(combatant.CurrentTile.gridLocation.y - player.CurrentTile.gridLocation.y);

                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestPlayer = player;
                }
            }

            return nearestPlayer;
        }

        /// <summary>
        /// Finds the nearest combatant within a given radius of this combatant.
        /// </summary>
        protected Combatant FindNearestPlayerWithinRadius(List<Combatant> toCheck, int radius)
        {
            Combatant nearestPlayer = null;
            int shortestDistance = int.MaxValue;

            foreach (Combatant player in toCheck)
            {
                int distance = Mathf.Abs(combatant.CurrentTile.gridLocation.x - player.CurrentTile.gridLocation.x) +
                               Mathf.Abs(combatant.CurrentTile.gridLocation.y - player.CurrentTile.gridLocation.y);

                if (distance <= radius && distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestPlayer = player;
                }
            }

            return nearestPlayer;
        }

        // ======================================
        #endregion // Detection =================
    }
}

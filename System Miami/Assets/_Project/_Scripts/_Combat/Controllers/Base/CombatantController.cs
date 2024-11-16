// Authors: Layla Hoey, Lee St Louis
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SystemMiami.AbilitySystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatSystem

{
    // An abstract class that PlayerController and
    // EnemyController controller can derive from.
    public abstract class CombatantController : MonoBehaviour
    {
        #region EVENTS
        // ======================================

        // Tiles
        public Action<OverlayTile> FocusedTileChanged;
        public Action<DirectionalInfo> PathTileChanged;

        // TODO:
        // Should the TRIGGERS below be refactored into events?

        // ======================================
        #endregion // EVENTS


        #region SERIALIZED
        // ======================================

        [SerializeField] protected float movementSpeed;

        // ======================================
        #endregion // SERIALIZED


        #region PROTECTED VARS
        // ======================================

        // Components
        protected Combatant combatant;

        // Phases
        protected readonly Phase[] defaultPhases = 
        {
            Phase.Movement,
            Phase.Action
        };

        protected List<Phase> remainingPhases = new List<Phase>();

        // Pathing
        protected PathFinder pathFinder = new PathFinder();
        protected List<OverlayTile> currentPath = new List<OverlayTile>();
        protected int currentPathCost;

        // Abilities
        protected AbilityType typeToEquip;
        protected int indexToEquip;

        // ======================================
        #endregion // PROTECTED VARS


        #region PROPERTIES
        // ======================================

        // Turns
        public bool IsMyTurn { get; protected set; }
        public Phase CurrentPhase { get; protected set; }

        // Tiles
        public OverlayTile FocusedTile { get; protected set; }
        public OverlayTile DestinationTile { get; protected set; }

        // Movement
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

        // Abilities
        public bool CanAct
        {
            get
            {
                if (combatant == null)
                    { return false; }

                if (CurrentPhase != Phase.Action)
                    { return false; }

                if (combatant.Abilities.CurrentState == Abilities.State.COMPLETE)
                {
                    HasActed = true;
                    return false;
                }

                if (combatant.Abilities.CurrentState == Abilities.State.EXECUTING)
                    { return false; }

                if (IsActing)
                {
                    //Debug.Log($"{name} is already acting");
                    return false;
                }

                if (HasActed)
                {
                    //Debug.Log($"{name} has already acted");
                    return false;
                }


                return true;
            }
        }

        public bool IsActing
        {
            get
            {
                if (combatant == null) { return false; }
                if (CurrentPhase != Phase.Action) { return false; }

                return combatant.Abilities.CurrentState == Abilities.State.EXECUTING;
            }
        }

        public bool HasActed { get; protected set; }

        // ======================================
        #endregion // PROPERTIES


        #region UNITY METHODS
        // ======================================

        private void Awake()
        {
            combatant = GetComponent<Combatant>();
        }

        private void Start()
        {
            if (!TryGetComponent(out combatant))
            {
                Debug.LogWarning($"Didnt find a Combatant component on {name}.");
            }
        }

        private void Update()
        {
            if (!IsMyTurn) { resetFlags(); return; }

            updateFocusedTile();

            if (endTurnTriggered())
            {
                EndTurn();
                resetFlags();
                return;
            }

            if (nextPhaseTriggered())
            {
                if (!TryNextPhase())
                {
                    OnNextPhaseFailed();
                }
                resetFlags();
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

            resetFlags();
        }

        protected void LateUpdate()
        {
            if (!IsMyTurn) { resetFlags(); return; }

            //if (endTurnTriggered())
            //{
            //    EndTurn();
            //    resetFlags();
            //    return;
            //}

            //if (nextPhaseTriggered())
            //{
            //    if (!TryNextPhase())
            //    {
            //        OnNextPhaseFailed();
            //    }
            //    resetFlags();
            //    return;
            //}

            //switch (CurrentPhase)
            //{
            //    case Phase.Movement:
            //        handleMovementPhase();
            //        break;

            //    case Phase.Action:
            //        handleActionPhase();
            //        break;

            //    default:
            //    case Phase.None:
            //        break;
            //}

            //resetFlags();
        }

        // ======================================
        #endregion // UNITY METHODS


        #region TURN MANAGEMENT
        // ======================================

        public virtual void StartTurn()
        {
            Debug.Log($"{name} starting turn");

            combatant.ResetTurn();
            remainingPhases.Clear();
            remainingPhases = defaultPhases.ToList();

            FocusedTile = null;
            DestinationTile = null;

            IsMoving = false;
            HasActed = false;

            IsMyTurn = true;

            if (!TryNextPhase())
            {
                OnNextPhaseFailed();
            }
        }

        protected virtual bool TryNextPhase()
        {
            CurrentPhase = Phase.None;

            if (remainingPhases.Count == 0)
                { return false; }

            CurrentPhase = remainingPhases[0];
            remainingPhases.RemoveAt(0);

            TurnManager.MGR.NewTurnPhase(CurrentPhase);

            return true;
        }

        protected virtual void OnNextPhaseFailed()
        {
            Debug.LogWarning($"{combatant} is trying" +
                        $"to move to the next phase,\n" +
                        $"But has no phases remaining.");
        }

        public virtual void EndTurn()
        {
            Debug.Log($"{name}Calling end of turn");
            // TODO
            // Other end of turn things.
            IsMyTurn = false;
        }
        // ======================================
        #endregion // TURN MANAGEMENT


        #region TRIGGERS
        // ======================================

        // Turn Control Triggers
        protected abstract bool endTurnTriggered();
        protected abstract bool nextPhaseTriggered();

        // Movement Triggers
        protected abstract bool beginMovementTriggered();

        // Ability Triggers
        protected abstract bool unequipTriggered();
        protected abstract bool equipTriggered();
        protected abstract bool lockTargetsTriggered();
        protected abstract bool useAbilityTriggered();

        protected abstract void resetFlags();

        // ======================================
        #endregion // TRIGGERS


        #region PHASE HANDLING
        // ======================================

        protected virtual void handleMovementPhase()
        {
            if (beginMovementTriggered() && CanMove)
            {
                Debug.Log($"{name} is beginning movement");
                beginMovement();
            }

            if (!IsMoving) { return; }

            if (!destinationReached())
            {
                moveAlongPath();
            }            
            else
            {
                clearMovement();
            }
        }

        protected virtual void handleActionPhase()
        {
            if (!CanAct) { return; }


            if (unequipTriggered())
            {
                combatant.Abilities.TryUnequip();
            }

            if (equipTriggered())
            {
                combatant.Abilities.TryEquip(typeToEquip, indexToEquip);
            }

            if (lockTargetsTriggered())
            {
                combatant.Abilities.TryLockTargets();
            }

            if (useAbilityTriggered())
            {
                if (combatant.Abilities.AbilityExecutionIsValid(out IEnumerator abilityProcess))
                {
                    StartCoroutine(abilityProcess);
                }
            }
        }

        // ======================================
        #endregion // PHASE HANDLING ============


        #region FOCUSED TILE
        // ======================================

        protected abstract void updateFocusedTile();

        protected abstract void resetFocusedTile();

        protected abstract OverlayTile getFocusedTile();

        // ======================================
        #endregion // FOCUSED TILE


        #region MOVEMENT
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

            if (truncatedPath == null) { return; }

            // New path is the truncated one.
            currentPath = truncatedPath;

            if (truncatedPath.Count > 0)
            {
                // New Destination Tile is the
                // last one in the truncated path.
                DestinationTile = truncatedPath[truncatedPath.Count - 1];


                // Create a copy of our path that contains the
                // starting position. We can use this modified list
                // To draw the arrows.
                List<OverlayTile> inclusivePath = currentPath;
                inclusivePath.Insert(0, combatant.CurrentTile);
                DrawArrows.MGR.DrawPath(inclusivePath);
            }
            else
            {
                DestinationTile = combatant.CurrentTile;
                Debug.Log($"{name} tried to move but cant. " +
                    $"Their calculated path was {truncatedPath.Count} tiles long.");
            }

            IsMoving = true;
        }

        /// <summary>
        /// Returns false if there is
        /// no current destination tile.
        /// Returns true if there is no current path,
        /// or if the combatant's current tile is the same
        /// as the current destination tile.
        /// </summary>
        /// <returns></returns>
        protected bool destinationReached()
        {
            return combatant.CurrentTile == DestinationTile;
        }

        /// <summary>
        /// Clears destination tile,
        /// current path, and movement bool.
        /// </summary>
        protected void clearMovement()
        {
            DestinationTile = combatant.CurrentTile;
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

            // If the destination is blocked (e.g. bc theres a charac there),
            // remove it from the end.
            if (path.Count > 0 && !path[path.Count - 1].Valid)
            {
                path.RemoveAt(path.Count - 1);
            }

            return path;
        }

        /// <summary>
        /// Moves the character along the calculated path.
        /// </summary>
        private void moveAlongPath()
        {
            //Debug.Log($"{name} calling move along path and tilecount is {currentPath.Count}");

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
        #endregion // MOVEMENT


        #region DETECTION
        // ======================================

        /// <summary>
        /// Currently only returns null.
        /// TODO:
        /// Should find the nearest combatant to the enemy.
        /// </summary>
        protected Combatant GetNearestCombatant(List<Combatant> toCheck)
        {
            Combatant nearestCombatant = null;
            //int shortestDistance = int.MaxValue;

            //foreach (Combatant combatant in toCheck)
            //{
            //    int distance = Mathf.Abs(combatant.CurrentTile.gridLocation.x - combatant.CurrentTile.gridLocation.x) +
            //                   Mathf.Abs(combatant.CurrentTile.gridLocation.y - combatant.CurrentTile.gridLocation.y);

            //    if (distance < shortestDistance)
            //    {
            //        shortestDistance = distance;
            //        nearestCombatant = combatant;
            //    }
            //}

            return nearestCombatant;
        }

        /// <summary>
        /// Currently only returns null.
        /// TODO:
        /// Should find the nearest combatant within a given radius of this combatant.
        /// </summary>
        protected Combatant GetNearestCombatantWithinRadius(List<Combatant> toCheck, int radius)
        {
            Combatant nearestCombatant = null;
            //int shortestDistance = int.MaxValue;

            //foreach (Combatant combatant in toCheck)
            //{
            //    int distance = Mathf.Abs(this.combatant.CurrentTile.gridLocation.x - combatant.CurrentTile.gridLocation.x) +
            //                   Mathf.Abs(this.combatant.CurrentTile.gridLocation.y - combatant.CurrentTile.gridLocation.y);

            //    if (distance <= radius && distance < shortestDistance)
            //    {
            //        shortestDistance = distance;
            //        nearestCombatant = combatant;
            //    }
            //}

            return nearestCombatant;
        }

        // ======================================
        #endregion // DETECTION
    }
}

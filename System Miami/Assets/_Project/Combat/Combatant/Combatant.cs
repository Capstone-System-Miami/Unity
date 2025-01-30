// Authors: Layla Hoey, Lee St Louis
using System;
using System.Collections.Generic;
using SystemMiami.AbilitySystem;
using SystemMiami.CombatRefactor;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [RequireComponent(
        typeof(Stats),
        typeof(Abilities)
        )]
    public abstract class Combatant : MonoBehaviour, IHighlightable, IDamageReciever, IHealReciever, IForceMoveReciever, ITargetable
    {
        protected const float PLACEMENT_RANGE = 0.0001f;

        [SerializeField] private Color _colorTag = Color.white;
        [SerializeField] private bool _printUItoConsole;
        [SerializeField] private float _movementSpeed;

        [SerializeField] private List<NewAbilitySO> physical;
        [SerializeField] private List<NewAbilitySO> magical;
        [SerializeField] private List<ConsumableSO> consumable;

        #region priv
        private CombatantStateFactory stateFactory;
        private CombatantState currentState;

        private Stats _stats;
        private float _endOfTurnDamage;

        private SpriteRenderer _renderer;
        private Color _defaultColor;

        private Animator _animator;
        private int dirParam = Animator.StringToHash("TileDir");

        OverlayTile positionTile;
        OverlayTile focusTile;
        DirectionContext directionContext;

        private bool isDamageable = true;
        private bool isHealable = true;
        private bool isMovable = true;
        private bool isStunned = false;
        private bool isInvisible = false;
        #endregion priv

        #region Properties
        public int ID { get; set; }
        public Color ColorTag { get { return _colorTag; } }

        // State Machine
        public CombatantStateFactory Factory { get { return stateFactory; } }
        public CombatantState CurrentState
        {
            get
            {
                return currentState;
            }
            set
            {
                if (value.combatant != this)
                {
                    Debug.LogError(
                        $"{name} is trying to transition" +
                        $"to a state belonging to" +
                        $"{value.combatant.name}"
                        );
                    return;
                }

                currentState = value;
            }
        }
        public bool PrintUItoConsole { get { return _printUItoConsole; } }

        public bool IsMyTurn { get; set; }
        public bool ReadyToStart { get { return currentState is Idle; } }
        public Phase CurrentPhase { get { return currentState.phase; } }

        // Stats & Resources
        public Stats Stats { get { return _stats; } }
        public Resource Health { get; set; }
        public Resource Stamina { get; set; }
        public Resource Mana { get; set; }
        public Resource Speed { get; set; }


        public Loadout loadout;
        public CombatAction selectedAbility { get; set; }
        // ^^^ refactored, testing ^^^

        public Animator Animator { get { return _animator; } }


        // TODO Should only be null if dead.
        public OverlayTile PositionTile
        {
            get { return positionTile; }
            private set { value = positionTile; }
        }
        public OverlayTile FocusTile
        {
            get { return focusTile; }

            set
            {
                if (value == focusTile) { return; }

                OverlayTile previous = focusTile;
                focusTile = value;
                OnFocusTileChanged(previous, focusTile);

                CurrentDirectionContext =
                    new((Vector2Int)PositionTile.GridLocation,
                    (Vector2Int)focusTile.GridLocation);                
            }
        }
        public DirectionContext CurrentDirectionContext
        {
            get { return directionContext; }
            set
            {
                DirectionContext previous = directionContext;
                directionContext = value;

                if (previous.BoardDirection == directionContext.BoardDirection)
                {
                    return;
                }

                OnDirectionChanged(directionContext);
            }
        }
        #endregion Properties

        #region Events
        public event EventHandler<FocusTileChangedEventArgs> FocusTileChanged;
        public event EventHandler<DirectionChangedEventArgs> DirectionChanged;
        #endregion Events


        #region Unity
        private void Awake()
        {
            _stats = GetComponent<Stats>();

            _renderer = GetComponent<SpriteRenderer>();
            _defaultColor = _renderer.color;

            _animator = GetComponent<Animator>();

        }

        protected virtual void Start()
        {
            initResources();
            initLoadout();
            initDirection();
            initStateMachine();
        }

        private void Update()
        {
            UpdateResources();

            FocusTile = GetNewFocus();

            CurrentState.Update();
            CurrentState.MakeDecision();
        }

        #endregion Unity

        #region Construction
        private void initResources()
        {
            Health = new Resource(_stats.GetStat(StatType.MAX_HEALTH));
            Stamina = new Resource(_stats.GetStat(StatType.STAMINA));
            Mana = new Resource(_stats.GetStat(StatType.MANA));
            Speed = new Resource(_stats.GetStat(StatType.SPEED));
        }

        private void initLoadout()
        {
            loadout = new(physical, magical, consumable, this);
        }

        private void initDirection()
        {
            Vector2Int currentPos
                = (Vector2Int)PositionTile.GridLocation;

            Vector2Int forwardPos = MapManager.MGR.CenterPos;

            CurrentDirectionContext = new(currentPos, forwardPos);

            UpdateAnimDirection(CurrentDirectionContext.ScreenDirection);
        }

        private void initStateMachine()
        {
            stateFactory = new(this);
            currentState = stateFactory.Idle();
            currentState.OnEnter();
        }

        private void initAbilities()
        {

        }
        #endregion Construction

        #region Movement
        public void StepTowards(OverlayTile target)
        {
            float stepDistance = _movementSpeed * Time.deltaTime;

            Vector2 positionAfterStep = Vector2.MoveTowards(
                transform.position,
                target.transform.position,
                stepDistance
            );

            transform.position = positionAfterStep;
        }

        public bool InPlacementRangeOf(OverlayTile targetTile)
        {
            float distanceToTarget = Vector2.Distance(
                transform.position,
                targetTile.transform.position
                );
            return distanceToTarget < PLACEMENT_RANGE;
        }

        public void SnapTo(OverlayTile tile)
        {
            if (!tile.TryAddOccupier(this))
            {
                Debug.LogWarning(
                    $"{name}'s {this} could not place itself" +
                    $"on {tile.gameObject.name}'s {tile}.");
            }

            PositionTile?.RemoveOccupier(this);
            PositionTile = tile;
        }
        #endregion Movement

        #region Focus
        /// <summary>
        /// Whatever tile the combatant
        /// is currently focusing on.
        /// The methods for determining this
        /// must be defined in derived classes.
        /// </summary>
        public abstract OverlayTile GetNewFocus();
        public OverlayTile GetDefaultFocus()
        {
            OverlayTile result;

            Vector2Int forwardPos
                = CurrentDirectionContext.ForwardA;

            if (MapManager.MGR.map.TryGetValue(forwardPos, out result))
            {
                return result;
            }

            if (MapManager.MGR.map.TryGetValue(MapManager.MGR.CenterPos, out result))
            {

            }
                Debug.LogError(
                    $"FATAL | {name}'s {this}" +
                    $"FOUND NO TILE TO FOCUS ON."
                    );

            return result;
        }
        #endregion Focus

        #region Updates
        private void UpdateResources()
        {
            Health = new Resource(_stats.GetStat(StatType.MAX_HEALTH), Health.Get());
            Stamina = new Resource(_stats.GetStat(StatType.STAMINA), Stamina.Get());
            Mana = new Resource(_stats.GetStat(StatType.MANA), Mana.Get());
            Speed = new Resource(_stats.GetStat(StatType.SPEED), Speed.Get());
        }

        public void UpdateAnimDirection(ScreenDir screenDirection)
        {
            Animator.SetInteger(
                dirParam,
                (int)screenDirection
                );
        }

        /// <summary>
        /// TODO:
        /// These are being subscribed to by
        /// Ability's TargetingPatterns.
        /// 
        /// <para>
        /// These "subscription" response fns in
        /// TargettingPattern should just be called
        /// in ActionEquipped states when necessary.</para>
        /// 
        /// <para>
        /// Patterns that subscribe to OnSubjectChanged
        /// should have their funcitons called when
        /// DirectionContext.TilePositionB changes.</para>
        /// 
        /// <para>
        /// OnDirectionChanged subscribers
        /// should have their functions called when
        /// DirectionContext.BoardDirection changes.</para>
        /// </summary>
        //public void NotifyTargetingPatterns(DirectionContext directionContext)
        //{
        //    OnSubjectChanged?.Invoke(directionContext);
        //    OnDirectionChanged?.Invoke(directionContext);
        //}

        #endregion Updates

        #region IHighlightable

        public void Highlight()
        {
            Debug.Log($"Highlight (no args overload) called on {name}.\n" +
                $"This should be called from OverlayTile when the player\n" +
                $"mouses over a tile containing a combatant.\n" +
                $"The function should enable / instantiate a\n" +
                $"worldspace UI canvas with combatant info.");
        }

        public void Highlight(Color color)
        {
            if (!isInvisible)
            {
                print($"{name} is being highlighted");
                _renderer.color = color;
            }
            else
            {
                print($"{name} is not being highlighted, because it's invisible");
            }
        }

        public void UnHighlight()
        {
            print($"{name} is no longer highlighted");
            _renderer.color = _defaultColor;
        }

        public GameObject GameObject()
        {
            return gameObject;
        }
        #endregion

        #region IDamageable
        public bool IsCurrentlyDamageable()
        {
            return isDamageable;
        }

        public void RecieveDamageAmount(float amount)
        {
            print($"{name} took {amount} damage.");

            Health.Lose(amount);
        }
        public void RecieveDamagePercent(float percent, bool ofMax)
        {
            float amount = ofMax
                ? (Health.Get() * percent)
                : (Health.GetMax() * percent);

            print($"{name} took {amount} damage.");

            Health.Lose(amount);
        }
        #endregion

        #region IHealable
        public bool IsCurrentlyHealable()
        {
            return isHealable;
        }

        public void RecieveFullHeal()
        {
            print($"{name} gained full health.");

            Health.Reset();
        }

        public void ReceiveHealAmount(float amount)
        {
            print($"{name} gained {amount} health.");
            Health.Gain(amount);
        }

        public void RecieveHealPercent(float percent, bool ofMax)
        {
            float amount = ofMax
                ? (Health.Get() * percent)
                : (Health.GetMax() * percent);

            print($"{name} gained {amount} health.");

            Health.Gain(amount);
        }
        #endregion

        public void RestoreResource(Resource resource, float amount)
        {
            print($"{name} gained {amount} {resource}.");
            resource.Gain(amount);
        }

        #region IMovable
        public bool IsCurrentlyMovable()
        {
            throw new NotImplementedException();
        }

        public Vector2Int GetTilePos()
        {
            return (Vector2Int)PositionTile.GridLocation;
        }

        public bool TryMoveTo(Vector2Int tilePos)
        {
            if (isMovable)
            {
                // TODO: Implement movement logic
                print($"{name} would move to {tilePos}, but this mechanic has not been implemented");
                return true;
            }
            else
            {
                print($"{name} cannot move");
                return false;
            }
        }

        public bool TryMoveInDirection(Vector2Int boardDirection, int distance)
        {
            if (isMovable)
            {
                // TODO: Implement directional movement logic
                Vector2Int newPos = (Vector2Int)PositionTile.GridLocation + boardDirection * distance;

                print($"{name} would move to {newPos}, but this mechanic has not been implemented");
                return true;
            }
            else
            {
                print($"{name} cannot move");
                return false;
            }
        }
        #endregion

        public void InflictStatusEffect(StatusEffect effect)
        {
            _stats.AddStatusEffect(effect);
            _endOfTurnDamage = effect.Damage;
        }

        #region ITargetable
        //============================================================
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="preferredColor"></param>
        public void HandleBeginTargeting(Color preferredColor)
        {
            ///
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="preferredColor"></param>
        public void HandleEndTargeting(Color preferredColor)
        {
            ///
        }

        /// <summary>
        /// TODO this might end up returning a state,
        /// rather than the combatant itself.
        /// This way, states can decide what to do when
        /// Damage methods are called on the combatant.
        /// </summary>
        /// <param name="damageInterface"></param>
        /// <returns></returns>
        public bool TryGetDamageable(out IDamageReciever damageInterface)
        {
            damageInterface = this;
            return true;
        }

        /// <summary>
        /// TODO this might end up returning a state,
        /// rather than the combatant itself.
        /// This way, states can decide what to do when
        /// Heal methods are called on the combatant.
        /// </summary>
        /// <param name="healInterface"></param>
        /// <returns></returns>
        public bool TryGetHealable(out IHealReciever healInterface)
        {
            healInterface = this;
            return true;
        }

        /// <summary>
        /// TODO this might end up returning a state,
        /// rather than the combatant itself.
        /// This way, states can decide what to do when
        /// ForceMove methods are called on the combatant.
        /// </summary>
        /// <param name="moveInterface"></param>
        /// <returns></returns>
        public bool TryGetMovable(out IForceMoveReciever moveInterface)
        {
            moveInterface = this;
            return true;
        }
        #endregion ITargetable

        protected virtual void OnFocusTileChanged(OverlayTile prevTile, OverlayTile newTile)
        {
            FocusTileChanged(this, new FocusTileChangedEventArgs(prevTile, newTile));
        }
        protected virtual void OnDirectionChanged(DirectionContext newDirection)
        {
            DirectionChanged(this, new DirectionChangedEventArgs(newDirection));
        }
    }

    public class FocusTileChangedEventArgs : EventArgs
    {
        public OverlayTile previousTile;
        public OverlayTile newTile;

        public FocusTileChangedEventArgs(OverlayTile previousTile, OverlayTile newTile)
        {
            this.previousTile = previousTile;
            this.newTile = newTile;
        }
    }

    public class DirectionChangedEventArgs : EventArgs
    {
        public DirectionContext newDirectionContext;

        public DirectionChangedEventArgs(DirectionContext newDirectionContext)
        {
            this.newDirectionContext = newDirectionContext;
        }
    }
}

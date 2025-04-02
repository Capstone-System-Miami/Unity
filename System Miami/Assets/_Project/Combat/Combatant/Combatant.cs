// Authors: Layla Hoey, Lee St Louis
using System;
using System.Collections.Generic;
using SystemMiami.AbilitySystem;
using SystemMiami.CombatRefactor;
using SystemMiami.InventorySystem;
using SystemMiami.Management;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [RequireComponent(
        typeof(Stats),
        typeof(Abilities)
        )]
    public abstract class Combatant : MonoBehaviour, ITargetable, ITileOccupant, IHighlightable, IDamageReceiver, IResourceReceiver, IForceMoveReceiver, IStatusEffectReceiver
    {
        protected const float PLACEMENT_RANGE = 0.0001f;

        #region Serialized Vars
        //============================================================
        [Header("General Info")]
        [SerializeField] private Color _colorTag = Color.white;

        
        
        [Header("Settings"), Space(10)]
        [SerializeField] private bool _printUItoConsole;
        [SerializeField] private float _movementSpeed;

        [Header("Animation")]
        [SerializeField] protected AnimatorOverrideController idleController;
        [SerializeField] protected AnimatorOverrideController walkingController;
        
        #endregion Serialized Vars


        #region Private Vars
        //============================================================

        // State Machine
        private CombatantStateFactory stateFactory;
        private CombatantState currentState;
        
        // Rendering
        private SpriteRenderer _renderer;
        private Color _defaultColor;

        // Stats & status
        private Stats _stats;
        private bool isDamageable = true;
        private bool isHealable = true;
        private bool isMovable = true;
        private bool isStunned = false;
        private bool isInvisible = false;
        public bool hasResourceEffect = false;
        public Dictionary<ResourceType, int> restoreResourceEffects = new();
        public float _endOfTurnDamage;
        public float _endOfTurnHeal;
        public float _endOfTurnStamina;
        public float _endOfTurnMana;

        // Animator
        private Animator _animator;
        private int dirParam = Animator.StringToHash("TileDir");

        // Tile Management
        private OverlayTile positionTile;
        private OverlayTile focusTile;
        private OverlayTile previousFocus;
        private DirectionContext directionContext;

        public event Action<Combatant> DamageTaken;

        private object eventLock = new object();
        #endregion Private Vars


        #region Properties
        //============================================================

        // General Info
        public int ID { get; set; }
        public Color ColorTag { get { return _colorTag; } }

        [SerializeField] public Inventory _inventory;
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

        // Stats, Status, Resources
        public Stats Stats { get { return _stats; } }
        public Resource Health { get; set; }
        public Resource Stamina { get; set; }
        public Resource Mana { get; set; }
        public Resource Speed { get; set; }

        // Animator
        public Animator Animator { get { return _animator; } }
        public AnimatorOverrideController AnimControllerIdle { get { return idleController; } }
        public AnimatorOverrideController AnimControllerWalking { get { return walkingController; } }

        // Tile Management
        // TODO Should only be null if dead.
        public OverlayTile PositionTile
        {
            get { return positionTile; }
            set { positionTile = value; }
        }

        public OverlayTile FocusTile
        {
            get { return focusTile; }

            set
            {
                if (value == focusTile) { return; }

                previousFocus = focusTile;
                focusTile = value;
                OnFocusTileChanged();
            }
        }

        public DirectionContext CurrentDirectionContext
        {
            get { return directionContext; }
            private set
            {
                DirectionContext previous = directionContext;
                directionContext = value;

                if (previous.BoardDirection
                    == directionContext.BoardDirection)
                {
                    return;
                }

                OnDirectionChanged();
            }
        }

        // Loadout
        public Loadout Loadout { get; protected set; }
        public CombatAction SelectedAbility { get; set; }

        #endregion Properties


        #region Events
        public event EventHandler<FocusTileChangedEventArgs> FocusTileChanged;
        public event EventHandler<DirectionChangedEventArgs> DirectionChanged;
        #endregion Events

        // ******************************************************
        // Methods
        // ******************************************************

        #region Unity Methods
        //============================================================
        private void Awake()
        {
            InitComponents();
        }

        private void OnEnable()
        {
            UI.MGR.CombatantLoadoutCreated += HandleLoadoutCreated;
        }


        private void OnDisable()
        {
            UI.MGR.CombatantLoadoutCreated -= HandleLoadoutCreated;
            currentState = null;
        }

        protected virtual void Start()
        {
            InitResources();
            InitLoadout();
            InitDirection();
            InitStateMachine();
        }

        private void Update()
        {
            if(this == null) return;
            UpdateResources();

            CurrentState.Update();
            CurrentState.MakeDecision();
        }

        #endregion Unity Methods


        #region Construction
        //============================================================
        public void InitAll()
        {
            InitResources();
            InitLoadout();
            InitDirection();
            InitStateMachine();
        }

        private void InitComponents()
        {
            _stats = GetComponent<Stats>();

            _renderer = GetComponent<SpriteRenderer>();
            _defaultColor = _renderer.color;

            _animator = GetComponent<Animator>();
        }

        private void InitResources()
        {
            Health = new Resource(_stats.GetStat(StatType.MAX_HEALTH));
            Stamina = new Resource(_stats.GetStat(StatType.STAMINA));
            Mana = new Resource(_stats.GetStat(StatType.MANA));
            Speed = new Resource(_stats.GetStat(StatType.SPEED));
        }

        protected abstract void InitLoadout();

        private void HandleLoadoutCreated(Loadout loadout, Combatant combatant)
        {
            if(combatant != this) { return; }

            Loadout = loadout;
        }

        private void InitDirection()
        {
            Vector2Int currentPos
                = (Vector2Int)PositionTile.GridLocation;

            Vector2Int forwardPos = MapManager.MGR.CenterPos;

            CurrentDirectionContext = new(currentPos, forwardPos);

            UpdateAnimDirection();
        }

        private void InitStateMachine()
        {
            stateFactory = new(this);
            currentState = stateFactory.Idle();
            currentState.OnEnter();
        }
        #endregion Construction


        #region Movement
        //============================================================
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
        #endregion Movement


        #region Tile Management
        //============================================================
        public void UpdateFocus()
        {            
            FocusTile = GetNewFocus() ?? GetDefaultFocus();
        }

        public void UpdateDirection()
        {
            CurrentDirectionContext = new(
                (Vector2Int)PositionTile.GridLocation,
                (Vector2Int)focusTile.GridLocation
                );
        }

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

        public void SnapToPositionTile()
        {
            if (PositionTile == null)
            {
                Debug.LogError(
                    $"{gameObject}'s {this} tried to " +
                    $"snap to its posiiton tile, but " +
                    $"its PositionTile was null.");
            }
            transform.position = PositionTile.OccupiedPosition;
        }
        #endregion Tile Management


        #region Resource Management
        //============================================================
        private void UpdateResources()
        {
            Health = new Resource(_stats.GetStat(StatType.MAX_HEALTH), Health.Get());
            Stamina = new Resource(_stats.GetStat(StatType.STAMINA), Stamina.Get());
            Mana = new Resource(_stats.GetStat(StatType.MANA), Mana.Get());
            Speed = new Resource(_stats.GetStat(StatType.SPEED), Speed.Get());
            Debug.Log($"{gameObject.name} has {Health.Get()} health.");
        }
        #endregion Resource Management


        #region Animation
        //============================================================
        public void UpdateAnimDirection()
        {
            if (FocusTile == null) { return; }
            Animator.SetInteger(
                dirParam,
                (int)CurrentDirectionContext.ScreenDirection
                );
        }
        #endregion Animation


        #region IHighlightable
        //============================================================
        public void Highlight()
        {
            //Debug.Log($"Highlight (no args overload) called on {name}.\n" +
            //    $"This should be called from OverlayTile when the player\n" +
            //    $"mouses over a tile containing a combatant.\n" +
            //    $"The function should enable / instantiate a\n" +
            //    $"worldspace UI canvas with combatant info.");
        }

        public void Highlight(Color color)
        {
            if (!isInvisible)
            {
                //print($"{name} is being highlighted");
                _renderer.color = color;
            }
            else
            {
                //print($"{name} is not being highlighted, because it's invisible");
            }
        }

        public void UnHighlight()
        {
            if(_renderer == null) return;
            //print($"{name} is no longer highlighted");
            _renderer.color = _defaultColor;
        }

        public GameObject GameObject()
        {
            return gameObject;
        }
        #endregion


        #region IDamageReciever
        //============================================================
        public bool IsCurrentlyDamageable()
        {
            return isDamageable;
        }

        public void PreviewDamage(float amount, bool perTurn, int durationTurns)
        {
            float currentHealth = Health.Get();
            Debug.Log(
                $"{gameObject.name} will take {amount} damage,\n" +
                $"taking its health from {currentHealth} to " +
                $"{currentHealth - amount}");
        }

        public void ReceiveDamage(float amount, bool perTurn, int durationTurns)
        {

            if(perTurn)
            {
                hasResourceEffect = true;
                _endOfTurnDamage -= amount;
                restoreResourceEffects.Add(ResourceType.Health, durationTurns);
            }
            else
            {
                Health.Lose(amount);
                Debug.Log(
                    $"{gameObject.name} took {amount} damage,\n" +
                    $"its Health is now {Health.Get()}");
            }

        }
        #endregion IDamageReciever


        #region IResourceReceiver
        //============================================================
        public bool IsCurrentlyHealable()
        {
            return isHealable;
        }

        public void PreviewResourceReceived(float amount, ResourceType type, bool perTurn, int duraationTurns)
        {
            // preview heal
        }

        public void ReceiveResource(float amount, ResourceType type, bool perTurn, int durationTurns)
        {

            if (perTurn)
            {
                hasResourceEffect = true;
                restoreResourceEffects.Add(type,durationTurns);
                if(type == ResourceType.Health)
                {
                    _endOfTurnHeal += amount;
                }
                else if (type == ResourceType.Stamina)
                {
                    _endOfTurnStamina += amount;
                }
                else if (type == ResourceType.Mana)
                {
                    _endOfTurnMana += amount;
                }
            }
            else
            {
               GainResource(type, amount);
            }

        }

        public void GainResource(ResourceType type, float amount)
        {
            switch (type)
            {
                case ResourceType.Health:
                    if (amount > 0)
                    {
                        Health.Gain(amount);
                    }
                    else
                    {
                        Health.Lose(-amount);
                    }
                    break;
                case ResourceType.Stamina:
                    Stamina.Gain(amount);
                    break;
                case ResourceType.Mana:
                    Mana.Gain(amount);
                    break;
                default:
                    Health.Gain(amount);
                    break;

            }
        }
        #endregion IHealReceiver


        #region IForceMoveReciever
        //============================================================
        public bool IsCurrentlyMovable()
        {
            throw new NotImplementedException();
        }
        public void PreviewForceMove(int distance, Vector2Int direction)
        {
            throw new NotImplementedException();
        }

        public void ReceiveForceMove(int distance, Vector2Int direction)
        {
            throw new NotImplementedException();
        }

        //public Vector2Int GetTilePos()
        //{
        //    return (Vector2Int)PositionTile.GridLocation;
        //}

        //public bool TryMoveTo(Vector2Int tilePos)
        //{
        //    if (isMovable)
        //    {
        //        // TODO: Implement movement logic
        //        print($"{name} would move to {tilePos}, but this mechanic has not been implemented");
        //        return true;
        //    }
        //    else
        //    {
        //        print($"{name} cannot move");
        //        return false;
        //    }
        //}

        //public bool TryMoveInDirection(Vector2Int boardDirection, int distance)
        //{
        //    if (isMovable)
        //    {
        //        // TODO: Implement directional movement logic
        //        Vector2Int newPos = (Vector2Int)PositionTile.GridLocation + boardDirection * distance;

        //        print($"{name} would move to {newPos}, but this mechanic has not been implemented");
        //        return true;
        //    }
        //    else
        //    {
        //        print($"{name} cannot move");
        //        return false;
        //    }
        //}
        #endregion IForceMoveReciever


        #region IStatusEffectReceiver
        //============================================================
        bool IStatusEffectReceiver.IsCurrentlyStatusEffectable()
        {
            return true;
        }

        void IStatusEffectReceiver.PreviewEffect(
            StatSet statMod,
            float damagePerTurn,
            float healPerTurn,
            int durationTurns)
        {
            Debug.Log(
                $"{gameObject.name} will have {statMod} applied,\n" +
                $"will begin taking {damagePerTurn} per turn,\n" +
                $"and healing {healPerTurn} per turn.\n" +
                $"The effect will last for {durationTurns} turns.");
        }

        void IStatusEffectReceiver.ReceiveEffect(
            StatSet statMod,
            float damagePerTurn,
            float healPerTurn,
            int durationTurns)
        {
            _stats.AddStatusEffect(statMod, durationTurns);

            /// TODO:   (from layla)
            /// These are not implemented and I can't think
            /// too deeply about them right now, I'm sorry lol
            
        }
        #endregion // IStatusEffectReveiver


        #region ITargetable
        //============================================================
        List<ISubactionCommand> ITargetable.TargetedBy { get; set; } = new();
        public string nameMessageForDB { get { return gameObject.name; } set { ; } }
        void ITargetable.SubscribeTo(
            ref EventHandler<TargetingEventArgs> combatActionEvent)
        {
            Debug.LogWarning($"inside {gameObject}'s Subscribe to action fn");

            combatActionEvent += HandleTargetingEvent;
        }

        void ITargetable.UnsubscribeTo(
            ref EventHandler<TargetingEventArgs> combatActionEvent)
        {
            combatActionEvent -= HandleTargetingEvent;
        }

        public void HandleTargetingEvent(object sender, TargetingEventArgs args)
        {
           // Debug.Log($"Trying to process a TargetingEvent of type {args.EventType}", gameObject);
           if (this is not ITargetable me) { return; }
            
          
            if(sender == null) return;
            
            switch (args.EventType)
            {
                
                case TargetingEventType.CANCELLED:
                    UnHighlight();
                    me.PreviewOff();
                    break;

                case TargetingEventType.STARTED:
                    Highlight(Color.magenta + new Color(-0.2f, 0, 0, 0));
                    break;

                case TargetingEventType.LOCKED:
                    Highlight(Color.magenta);
                    me.PreviewOn();
                    break;

                case TargetingEventType.EXECUTING:
                    me.ApplyCombatAction();
                    break;

                case TargetingEventType.COMPLETED:
                    /// TODO: Wait until !TargetedBy.Any() ?
                    break;

                case TargetingEventType.REPORTBACK:
                    Debug.Log("Im subbed.", this);
                    break;

                default:
                    break;
            }
        }

        public void PreviewOn()
        {
            if (this is not ITargetable me) { return; }

            me.TargetedBy.ForEach(subaction => subaction.Preview());
            //Debug.Log(
            //    $"{gameObject.name} wants to START" +
            //    $"displaying a preivew.");
        }

        public void PreviewOff()
        {
            if (this == null) return;
            Debug.Log(
                $"{gameObject.name} wants to STOP" +
                $"displaying a preivew.");
        }

        void ITargetable.ApplyCombatAction()
        {
            if (this is not ITargetable me) { return; }

            me.TargetedBy.ForEach(subaction => subaction.Execute());
        }

        /// <inheritdoc />
        /// <remarks>
        /// TODO (specific to Combatant implementation) :
        /// This might end up returning a state,
        /// rather than the combatant itself.
        /// This way, states can decide what to do when
        /// Damage methods are called on the combatant.
        /// </remarks>
        public virtual IDamageReceiver GetDamageInterface()
        {
            //Example of how these state-driven interfaces might work.
            // return currentState is IDamageReciever ? currentState : null;
            return this;
        }

        /// <inheritdoc />
        /// <remarks>
        /// TODO (specific to Combatant implementation):
        /// This might end up returning a state,
        /// rather than the combatant itself.
        /// This way, states can decide what to do when
        /// Heal methods are called on the combatant.
        /// </remarks>
        public virtual IResourceReceiver GetHealInterface()
        {
            return this;
        }

        /// <inheritdoc />
        /// <remarks>
        /// TODO (specific to Combatant implementation):
        /// This might end up returning a state,
        /// rather than the combatant itself.
        /// This way, states can decide what to do when
        /// ForceMove methods are called on the combatant.
        /// </remarks>
        public virtual IForceMoveReceiver GetMoveInterface()
        {
            return this;
        }

        /// <inheritdoc />
        /// <remarks>
        /// TODO (specific to Combatant implementation):
        /// This might end up returning a state,
        /// rather than the combatant itself.
        /// This way, states can decide what to do when
        /// StatusEffect methods are called on the combatant.
        /// </remarks>
        public virtual IStatusEffectReceiver GetStatusEffectInterface()
        {
            return this;
        }
        #endregion ITargetable


        #region Tile Event Raisers
        //============================================================
        protected virtual void OnFocusTileChanged()
        {
            UpdateDirection();

            FocusTileChanged?.Invoke(
                this,
                new FocusTileChangedEventArgs(
                    previousFocus,
                    focusTile,
                    directionContext)
            );
        }
        protected virtual void OnDirectionChanged()
        {
            UpdateAnimDirection();

            DirectionChanged?.Invoke(
                this,
                new DirectionChangedEventArgs(directionContext)
            );
        }
        #endregion Tile Event Raisers
    }


    #region Event Args
    //============================================================
    //============================================================
    public class FocusTileChangedEventArgs : EventArgs
    {
        public OverlayTile previousTile;
        public OverlayTile newTile;
        public DirectionContext directionContext;

        public FocusTileChangedEventArgs(
            OverlayTile previousTile,
            OverlayTile newTile,
            DirectionContext directionContext)
        {
            this.previousTile = previousTile;
            this.newTile = newTile;
            this.directionContext = directionContext;
        }
    }

    public class DirectionChangedEventArgs : EventArgs
    {
        public DirectionContext newDirectionContext;

        public DirectionChangedEventArgs(
            DirectionContext newDirectionContext)
        {
            this.newDirectionContext = newDirectionContext;
        }
    }
    #endregion Event Args
}

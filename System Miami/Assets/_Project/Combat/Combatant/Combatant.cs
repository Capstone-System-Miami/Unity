// Authors: Layla Hoey, Lee St Louis
using System;
using System.Collections;
using System.Collections.Generic;
using SystemMiami.AbilitySystem;
using SystemMiami.CombatRefactor;
using SystemMiami.InventorySystem;
using SystemMiami.Management;
using SystemMiami.Utilities;
using UnityEngine;
using UnityEngine.Assertions;

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
        [Header("Debug")]
        [SerializeField] private dbug log;
        [SerializeField] private bool detailedFocusHighlight;
        private AdjacentTileSet focusAdjacent;

        [Header("General Info")]
        [SerializeField] private Color _colorTag = Color.white;

        [Header("Settings"), Space(10)]
        [SerializeField] private bool _printUItoConsole;
        [SerializeField] private float _movementSpeed;
        [SerializeField] public KeyCode flowKey;

        [Header("Animation")]
        [SerializeField] protected AnimatorOverrideController idleController;
        [SerializeField] protected AnimatorOverrideController walkingController;
        #endregion Serialized Vars


        #region Private Vars
        //============================================================
        private bool initialized = false;

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
        // private bool isMovable = true;
        // private bool isStunned = false;
        private bool isInvisible = false;
        public bool hasResourceEffect = false;
        public List<IPerTurn> resourceEffects = new();
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

        // NOTE:
        // Commenting this out to get rid of annoying Console warnings.
        // Uncomment if needed.
        // public event Action<Combatant> DamageTaken;

        private object eventLock = new object();
        #endregion Private Vars


        #region Properties
        //============================================================

        // General Info
        public int ID { get; set; }
        public Color ColorTag { get { return _colorTag; } }

        [SerializeField] public Inventory _inventory;

        [SerializeField, ReadOnly] private string currentStateType;

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
                    log.error(
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
        public bool Initialized {
            get
            {
                return _stats != null
                    && _renderer != null
                    && _animator != null
                    && Health != null
                    && Stamina != null
                    && Mana != null
                    && Speed != null
                    && MapManager.MGR != null
                    && CurrentDirectionContext != null
                    && Factory != null
                    && CurrentState != null;
            }
        }
        public Phase CurrentPhase { get { return currentState?.phase ?? Phase.None; } }

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
                if (detailedFocusHighlight)
                {
                    focusAdjacent?.UnhighlightAll();
                    focusAdjacent = new(value);
                    focusAdjacent.HighlightAll();
                }
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
        public Loadout Loadout { get;  protected set; }
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

            if (TurnManager.MGR != null)
            {
                TurnManager.MGR.DungeonCleared += HandleDungeonCleared;
                TurnManager.MGR.DungeonFailed += HandleDungeonFailed;
            }
        }


        private void OnDisable()
        {
            UI.MGR.CombatantLoadoutCreated -= HandleLoadoutCreated;
            if (TurnManager.MGR != null)
            {
                TurnManager.MGR.DungeonCleared -= HandleDungeonCleared;
                TurnManager.MGR.DungeonFailed -= HandleDungeonFailed;
            }
            currentState = null;
        }

        protected virtual void Start()
        {
            InitAll();
        }

        private void Update()
        {
            if (this == null) return;
            if (!Initialized) return;
            UpdateResources();

            CurrentState.Update();
            CurrentState.MakeDecision();


            currentStateType = CurrentState.GetType().ToString();
            TEST_currentTile = PositionTile;

            if (TRIGGER_ForceMovePreviewTest)
            {
                TRIGGER_ForceMovePreviewTest = false;
                ForceMoveCommand testCommand = new(this, TEST_origin, TEST_moveType, TEST_distance);
                testCommand.Preview();
            }
            if (TRIGGER_ForceMoveExecuteTest)
            {
                TRIGGER_ForceMoveExecuteTest = false;
                ForceMoveCommand testCommand = new(this, TEST_origin, TEST_moveType, TEST_distance);
                testCommand.Execute();
            }
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
            Assert.IsNotNull(loadout, $"Incoming loadout was null HandleLoadoutCreated");

            Loadout = loadout;
        }

        private void InitDirection()
        {
            if (MapManager.MGR.map == null)
            {
                log.error($"{name}: Mapmanager is null in InitDirection. Ensure it is set before calling InitDirection.");
            }
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
            log.warn($"{name} is initializing state machine");
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

            if (!MapManager.MGR.map.TryGetValue(MapManager.MGR.CenterPos, out result))
            {
                log.error(
                    $"FATAL | {name}'s {this}" +
                    $"FOUND NO TILE TO FOCUS ON.");
            }

            return result;
        }

        public void SnapToPositionTile()
        {
            if (PositionTile == null)
            {
                log.error(
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
            log.print($"{gameObject.name} has {Health.Get()} health.");
        }
        #endregion Resource Management


        #region Animation
        //============================================================
        public void UpdateAnimDirection()
        {
            if (FocusTile == null) { return; }
            Animator.SetInteger(
                dirParam,
                (int)CurrentDirectionContext.ScreenDirection);
        }
        #endregion Animation


        #region IHighlightable
        //============================================================
        public void Highlight()
        {
            // log.print($"Highlight (no args overload) called on {name}.\n" +
            //     $"This should be called from OverlayTile when the player\n" +
            //     $"mouses over a tile containing a combatant.\n" +
            //     $"The function should enable / instantiate a\n" +
            //     $"worldspace UI canvas with combatant info.");
        }

        public void Highlight(Color color)
        {
            if (!isInvisible)
            {
                // log.print($"{name} is being highlighted");
                _renderer.color = color;
            }
            else
            {
                // log.print($"{name} is not being highlighted, because it's invisible");
            }
        }

        public void UnHighlight()
        {
            if(_renderer == null) return;
            // log.print($"{name} is no longer highlighted");
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
            log.print(
                $"{gameObject.name} will take {amount} damage,\n" +
                $"taking its health from {currentHealth} to " +
                $"{currentHealth - amount}");
        }

        public void ReceiveDamage(float amount, bool perTurn, int durationTurns)
        {
            float dmgRDX = _stats.GetStat(StatType.DMG_RDX);
            float healthToLose = Mathf.Max( amount - dmgRDX,0);
           
            Debug.Log("Damage after rdx: " + (healthToLose) + " dmgRDX is " + dmgRDX);
                
            Health.Lose(healthToLose);
            log.print(
              $"{gameObject.name} took {amount} damage,\n" +
              $"its Health is now {Health.Get()}");
              GAME.MGR.NotifyDamageTaken(this);
            
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
            GainResource(type, amount);
        }

        public void GainResource(ResourceType type, float amount)
        {
            switch (type)
            {
                case ResourceType.Health:
                    Health.Gain(amount);
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
        public Vector2Int TEST_origin;
        public int TEST_distance;
        public MoveType TEST_moveType;
        public bool TRIGGER_ForceMovePreviewTest;
        public bool TRIGGER_ForceMoveExecuteTest;
        [field: ReadOnly] public OverlayTile TEST_currentTile;

        bool IForceMoveReceiver.IsCurrentlyMovable()
        {
            return true;
        }
        void IForceMoveReceiver.PreviewForceMove(OverlayTile destinationTile)
        {
            // log.error(
            //     $"{name} is trying to priview movement to " +
            //     $"{destinationTile.name}, but its RecieveForceMove() method " +
            //     $"has not been implemented.",
            //     destinationTile);
            MovementPath pathToTile = new(PositionTile, destinationTile, true);
            StartCoroutine(TEST_ShowMovementPath(pathToTile));
        }

        void IForceMoveReceiver.RecieveForceMove(OverlayTile destinationTile)
        {
            MovementPath pathToTile = new(PositionTile, destinationTile, true);
            CurrentState.SwitchState(Factory.ForcedMovementExecution(pathToTile));

            // log.error(
            //     $"{name} is trying to move to {destinationTile.name}, but " +
            //     $"its RecieveForceMove() method has not been implemented.",
            //     destinationTile);
        }

        private IEnumerator TEST_ShowMovementPath(MovementPath path)
        {
            float duration = 1f;

            path.HighlightValidMoves(Color.cyan);
            path.DrawArrows();

            while (duration > 0)
            {
                duration -= Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }

            path.Unhighlight();
            path.UnDrawAll();
        }
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
            log.print(
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
        }
        #endregion // IStatusEffectReveiver


        #region ITargetable
        //============================================================
        Vector2Int ITargetable.BoardPos => PositionTile.BoardPos;
        List<ISubactionCommand> ITargetable.TargetedBy { get; set; } = new();
        public string nameMessageForDB { get { return gameObject.name; } set { ; } }
        void ITargetable.SubscribeTo(
            ref EventHandler<TargetingEventArgs> combatActionEvent)
        {
            log.warn($"inside {gameObject}'s Subscribe to action fn");

            combatActionEvent += HandleTargetingEvent;
        }

        void ITargetable.UnsubscribeTo(
            ref EventHandler<TargetingEventArgs> combatActionEvent)
        {
            combatActionEvent -= HandleTargetingEvent;
        }

        public void HandleTargetingEvent(object sender, TargetingEventArgs args)
        {
            // log.print($"Trying to process a TargetingEvent of type {args.EventType}", gameObject);
            if (this is not ITargetable me) { return; }
            if (sender == null) { return; }

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
                    log.print("Im subbed.", this);
                    break;

                default:
                    break;
            }
        }

        public void PreviewOn()
        {
            if (this is not ITargetable me) { return; }

            me.TargetedBy.ForEach(subaction => subaction.Preview());
            // log.print(
            //     $"{gameObject.name} wants to START" +
            //     $"displaying a preivew.");
        }

        public void PreviewOff()
        {
            if (this == null) return;
            log.print(
                $"{gameObject.name} wants to STOP" +
                $"displaying a preivew.");
        }

        void ITargetable.ApplyCombatAction()
        {
            if (this is not ITargetable me) { return; }

            me.TargetedBy.ForEach(subaction =>
            {
                if (subaction is IPerTurn effect && effect.perTurn)
                {
                    resourceEffects.Add(effect);
                    log.print($"Added a resource effect to {gameObject.name} with {subaction.GetType()}");
                    return;
                }
                subaction.Execute();

            });
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


        protected virtual void HandleDungeonCleared()
        {
            currentState.SwitchState(Factory.Idle());
        }

        protected virtual void HandleDungeonFailed()
        {
            if (currentState is not Dying && currentState is not Dead)
            {
                currentState.SwitchState(Factory.Idle());
            }
        }
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


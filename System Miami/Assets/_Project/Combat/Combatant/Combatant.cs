// Authors: Layla Hoey, Lee St Louis
using System;
using System.Collections.Generic;
using SystemMiami.AbilitySystem;
using SystemMiami.CombatRefactor;
using SystemMiami.Management;
using SystemMiami.Utilities;
using SystemMiami.Enums;
using UnityEngine;
using System.Collections;
using SystemMiami.ui;

namespace SystemMiami.CombatSystem
{
    [RequireComponent(
        typeof(Stats),
        typeof(Abilities)
        )]
    public abstract class Combatant : MonoBehaviour, IHighlightable, IDamageable, IHealable, IMovable
    {
        protected const float PLACEMENT_RANGE = 0.0001f;

        // These Actions will be subscribed to by each ability's
        // CombatActions when they are equipped and targeting.
        // Which one they subscribe to will depend on the origin
        // of the pattern.
        public Action<DirectionContext> OnSubjectChanged;
        public Action<DirectionContext> OnDirectionChanged;

        [SerializeField] private Color _colorTag = Color.white;
        [SerializeField] private bool _printUItoConsole;
        [SerializeField] private float _movementSpeed;

        private CombatantStateFactory stateFactory;
        private CombatantState currentState;

        private Stats _stats;
        private float _endOfTurnDamage;

        private Abilities _abilities;

        private SpriteRenderer _renderer;
        private Color _defaultColor;

        private Animator _animator;
        private int dirParam = Animator.StringToHash("TileDir");


        public bool IsDamageable = true;
        public bool IsHealable = true;
        public bool IsMovable = true;
        public bool IsStunned = false;
        public bool IsInvisible = false;

        public int ID { get; set; }
        public Color ColorTag { get { return _colorTag; } }
        public bool PrintUItoConsole { get { return _printUItoConsole; } }

        public bool IsMyTurn { get; set; }
        public bool ReadyToStart { get { return currentState is Idle; } }
        public Phase CurrentPhase { get { return currentState.phase; } }

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

        public Stats Stats { get { return _stats; } }
        [HideInInspector] public Resource Health;
        [HideInInspector] public Resource Stamina;
        [HideInInspector] public Resource Mana;
        [HideInInspector] public Resource Speed;

        public Abilities Abilities { get { return _abilities; } }
        // vvv refactored, testing vvv
        public List<AbilityPhysical> Physical { get; private set; } = new();
        public List<AbilityMagical> Magical { get; private set; } = new();
        public List<Consumable> Consumables { get; private set; } = new();

        public CombatAction selectedAbility { get; set; }
        // ^^^ refactored, testing ^^^

        public Animator Animator { get { return _animator; } }


        // Should only be null if dead.
        public OverlayTile CurrentTile { get; private set; }
        public DirectionContext CurrentDirectionContext;


        #region Unity
        private void Awake()
        {
            _stats = GetComponent<Stats>();

            _abilities = GetComponent<Abilities>();

            _renderer = GetComponent<SpriteRenderer>();
            _defaultColor = _renderer.color;

            _animator = GetComponent<Animator>();

        }

        protected virtual void Start()
        {
            initResources();
            initDirection();
            initStateMachine();
        }

        private void Update()
        {
            UpdateResources();

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

        private void initDirection()
        {
            Vector2Int currentPos
                = (Vector2Int)CurrentTile.GridLocation;

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

        public void SnapTo(OverlayTile target)
        {
            CurrentTile?.RemoveCombatant();
            CurrentTile = target;
            target.PlaceCombatant(this);
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

            if (!MapManager.MGR.map.TryGetValue(forwardPos, out result))
            {
                Debug.LogError(
                    $"FATAL | {name}'s {this}" +
                    $"FOUND NO TILE TO FOCUS ON."
                    );
            }

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
        public void NotifyTargetingPatterns(DirectionContext directionContext)
        {
            OnSubjectChanged?.Invoke(directionContext);
            OnDirectionChanged?.Invoke(directionContext);
        }

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
            if (!IsInvisible)
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
        public void Damage(float amount)
        {
            if (IsDamageable)
            {
                print($"{name} lost {amount} health.");

                Health.Lose(amount);
            }
            else
            {
                print($"{name} took no damage");
            }
        }
        #endregion

        #region IHealable
        public void Heal()
        {
            if (IsHealable)
            {
                print($"{name} gained full health.");

                Health.Reset();
            }
            else
            {
                print($"{name} is not healable");
            }
        }

        public void Heal(float amount)
        {
            if (IsHealable)
            {
                print($"{name} gained {amount} health.");

                Health.Gain(amount);
            }
            else
            {
                print($"{name} is not healable");
            }
        }
        #endregion

        public void RestoreResource(Resource type, float amount)
        {
            print($"{name} gained {amount} {type}.");
            type.Gain(amount);
        }

        #region IMovable
        public Vector2Int GetTilePos()
        {
            return (Vector2Int)CurrentTile.GridLocation;
        }

        public bool TryMoveTo(Vector2Int tilePos)
        {
            if (IsMovable)
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
            if (IsMovable)
            {
                // TODO: Implement directional movement logic
                Vector2Int newPos = (Vector2Int)CurrentTile.GridLocation + boardDirection * distance;

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

        public void ResetTurn()
        {
            Speed = new Resource(_stats.GetStat(StatType.SPEED));
            _abilities.ReduceCooldowns();
            _stats.UpdateStatusEffects();
            Health?.Lose(_endOfTurnDamage);
        }

        public void SelectPhysicalAbility(ui.AbilitySlot slot)
        {
            int ind = slot.Index;

            if (Physical.Count <= ind)
            {
                Debug.Log($"{name} Phys count less than {ind}");

                return;
            }

            if (Physical[ind] == null)
            {
                Debug.Log($"{name} Phys count nothing found at {ind}");

                return;
            }

            selectedAbility = Physical[ind];

            Debug.Log($"{name} selected {selectedAbility}");
        }
    }
}

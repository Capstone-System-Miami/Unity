// Authors: Layla Hoey, Lee St Louis
using System;
using System.Collections.Generic;
using SystemMiami.AbilitySystem;
using SystemMiami.CombatRefactor;
using SystemMiami.Management;
using SystemMiami.Utilities;
using SystemMiami.Enums;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [RequireComponent(
        typeof(Stats),
        typeof(Abilities),
        typeof(CombatantStateMachine))]
    public class Combatant : MonoBehaviour, IHighlightable, IDamageable, IHealable, IMovable
    {
        protected const float PLACEMENT_RANGE = 0.0001f;

        [SerializeField] private float _movementSpeed;

        public bool IsPlayer
        {
            get
            {
                return gameObject == PlayerManager.MGR.gameObject;
            }
        }

        [HideInInspector] public bool IsMyTurn;


        [SerializeField] private Color _colorTag = Color.white;


        private Animator _animator;
        private int dirParam = Animator.StringToHash("TileDir");


        // Should only be null if dead.
        public OverlayTile CurrentTile { get; private set; }

        public DirectionContext CurrentDirectionContext;

        public int ID { get; set; }

        [HideInInspector] public CombatantStateMachine StateMachine;

        private Stats _stats;
        private Abilities _abilities;

        // vvv refactored, testing vvv
        public List<AbilityPhysical> Physical { get; private set; } = new();
        public List<AbilityMagical> Magical { get; private set; } = new();
        public List<Consumable> Consumables { get; private set; } = new();
        // ^^^ refactored, testing ^^^

        private float _endOfTurnDamage;

        private SpriteRenderer _renderer;
        private Color _defaultColor;

        public Sprite[] PlayerDirSprites;
        public Sprite currentSprite;

        [HideInInspector] public Resource Health;
        [HideInInspector] public Resource Stamina;
        [HideInInspector] public Resource Mana;
        [HideInInspector] public Resource Speed;

        public bool IsDamageable = true;
        public bool IsHealable = true;
        public bool IsMovable = true;
        public bool IsStunned = false;
        public bool IsInvisible = false;

        public Color ColorTag { get { return _colorTag; } }

        // These Actions will be subscribed to by each ability's
        // CombatActions when they are equipped and targeting.
        // Which one they subscribe to will depend on the origin
        // of the pattern.
        public Action<DirectionContext> OnSubjectChanged;
        public Action<DirectionContext> OnDirectionChanged;

        public Stats Stats { get { return _stats; } }

        public Abilities Abilities { get { return _abilities; } }

        public Animator Animator { get { return _animator; } }


        #region Unity
        private void Awake()
        {
            _stats = GetComponent<Stats>();
            _abilities = GetComponent<Abilities>();
            StateMachine = GetComponent<CombatantStateMachine>();
            _renderer = GetComponent<SpriteRenderer>();
            _defaultColor = _renderer.color;
            _animator = GetComponent<Animator>();
            currentSprite = GetComponent<SpriteRenderer>().sprite;
        }

        private void OnEnable()
        {
            GAME.MGR.CombatantDeath += onCombatantDeath;
        }

        private void OnDisable()
        {
            GAME.MGR.CombatantDeath -= onCombatantDeath;
        }

        protected virtual void Start()
        {
            initResources();
            initDirection();
        }

        private void Update()
        {
            CheckDead();
            UpdateResources();
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
        #endregion Construction

        public void SwitchState(CombatantState newState)
        {
            StateMachine.SetState(newState);
        }

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
            CurrentTile = target;
            target.PlaceCombatant(this);
        }

        #region Updates
        private void CheckDead()
        {
            if (Health.Get() == 0)
            {
                GAME.MGR.CombatantDeath.Invoke(this);
            }
        }

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
        /// Now that DirectionContext is being updated each frame,
        /// these "subscriptions" should just be converted to
        /// public functions on TargetingPatterns that can
        /// be called by CombatantStates when necessary.</para>
        /// </summary>
        public void NotifyTargetingPatterns(DirectionContext directionContext)
        {
            OnSubjectChanged?.Invoke(directionContext);
            OnDirectionChanged?.Invoke(directionContext);
        }

        #endregion Updates

        private void onCombatantDeath(Combatant deadCombatant)
        {
            if (deadCombatant == this)
            {
                Die();
            }
        }

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

        public virtual void Die()
        {
            // Player died
            Debug.Log($"{name} has died.");
            Destroy(gameObject);
        }
    }
}

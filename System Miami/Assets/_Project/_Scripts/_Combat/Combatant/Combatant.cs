// Authors: Layla Hoey, Lee St Louis
using System;
using SystemMiami.AbilitySystem;
using SystemMiami.Enums;
using SystemMiami.Management;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [RequireComponent(
        typeof(Stats),
        typeof(Abilities)
        /*typeof(CombatantController)*/)]
    public class Combatant : MonoBehaviour, IHighlightable, IDamageable, IHealable, IMovable
    {
        [SerializeField] private Color _colorTag = Color.white;

        public OverlayTile CurrentTile;

        public Animator Animator;

        public DirectionalInfo DirectionInfo;

        public int ID { get; set; }

        private CombatantController _controller;
        private Stats _stats;
        private Abilities _abilities;

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
        public Action<DirectionalInfo> OnSubjectChanged;
        public Action<DirectionalInfo> OnDirectionChanged;

        public CombatantController Controller { get { return _controller; } }

        public Stats Stats { get { return _stats; } }

        public Abilities Abilities { get { return _abilities; } }


        #region Unity
        private void Awake()
        {
            _stats = GetComponent<Stats>();
            _abilities = GetComponent<Abilities>();
            _controller = GetComponent<CombatantController>();
            _renderer = GetComponent<SpriteRenderer>();
            _defaultColor = _renderer.color;
            Animator = GetComponent<Animator>();
            currentSprite = GetComponent<SpriteRenderer>().sprite;
        }

        private void OnEnable()
        {
            GAME.MGR.CombatantDeath += onCombatantDeath;

            if (_controller == null) { return; }
            _controller.FocusedTileChanged += onFocusedTileChanged;
            _controller.PathTileChanged += onPathTileChanged;
        }

        private void OnDisable()
        {
            GAME.MGR.CombatantDeath -= onCombatantDeath;

            if (_controller == null) { return; }
            _controller.FocusedTileChanged -= onFocusedTileChanged;
            _controller.PathTileChanged -= onPathTileChanged;
        }

        protected virtual void Start()
        {
            initResources();
            initCurrentTile();
            initDirection();
        }

        private void Update()
        {
            checkDead();
            updateResources();
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

        private void initCurrentTile()
        {
            if (CurrentTile == null)
            {
                Vector2Int gridPos = (Vector2Int)Coordinates.ScreenToIso(transform.position, 0);

                if (MapManager.MGR.map.ContainsKey(gridPos))
                {
                    CurrentTile = MapManager.MGR.map[gridPos];
                }
            }
        }

        private void initDirection()
        {
            Vector2Int currentPos = (Vector2Int)CurrentTile.GridLocation;
            setDirection(new DirectionalInfo(currentPos,  currentPos + Vector2Int.one));
        }
        #endregion Construction

        #region Subscriptions
        private void onFocusedTileChanged(OverlayTile newTile)
        {
            setDirectionByTile(newTile);
        }

        private void onPathTileChanged(DirectionalInfo newDirection)
        {
            setDirection(newDirection);

            // Decrement speed when combatant moves to a new tile.
            Speed.Lose(1);
        }
        #endregion Subscriptions

        #region Update
        private void checkDead()
        {
            if (Health.Get() == 0)
            {
                GAME.MGR.CombatantDeath.Invoke(this);
            }
        }

        private void updateResources()
        {
            Health = new Resource(_stats.GetStat(StatType.MAX_HEALTH), Health.Get());
            Stamina = new Resource(_stats.GetStat(StatType.STAMINA), Stamina.Get());
            Mana = new Resource(_stats.GetStat(StatType.MANA), Mana.Get());
            Speed = new Resource(_stats.GetStat(StatType.SPEED), Speed.Get());
        }
        #endregion Update

        #region Directions (priv)
        /// <summary>
        /// Sets the combatant's directional info.
        /// For players, this is based on mouse position.
        /// For enemies, it's based on their target or movement.
        /// </summary>
        private void setDirectionByTile(OverlayTile targetTile)
        {
            if (_controller.IsMoving) { return; }
            if (_abilities.CurrentState == Abilities.State.TARGETS_LOCKED) { return; }
            if (_abilities.CurrentState == Abilities.State.EXECUTING) { return; }
            if (targetTile == null) { return; }

            Vector2Int currentPos = (Vector2Int)CurrentTile.GridLocation;
            Vector2Int forwardPos;

            forwardPos = (Vector2Int)targetTile.GridLocation;

            DirectionalInfo newDirection = new DirectionalInfo(currentPos, forwardPos);

            OnSubjectChanged?.Invoke(newDirection);

            //if (newDirection.DirectionVec != DirectionInfo.DirectionVec)
            //{
            if ((int)newDirection.DirectionName == 0)
            {

                Animator.SetInteger("TileDir", 7);
            }
            else
            {
                Animator.SetInteger("TileDir", (int)newDirection.DirectionName - 1);
            }

            OnDirectionChanged?.Invoke(newDirection);
            //}
            Debug.Log(newDirection.DirectionName);

            DirectionInfo = newDirection;
        }

        /// <summary>
        /// Allows setting directional info directly.
        /// </summary>
        private void setDirection(DirectionalInfo newDirection)
        {
            DirectionInfo = newDirection;
            if ((int)newDirection.DirectionName == 0)
            {

              Animator.SetInteger("TileDir", 7 );
            }
            else
            {
                Animator.SetInteger("TileDir", (int)newDirection.DirectionName - 1);
            }

            OnSubjectChanged?.Invoke(newDirection);
            OnDirectionChanged?.Invoke(newDirection);
            Debug.Log(newDirection.DirectionName);
            
        }
        #endregion Directions (priv)


        private void onCombatantDeath(Combatant deadCombatant)
        {
            if (deadCombatant == this)
            {
                Die();
            }
        }

        //public void SwapSprite(Vector2Int direction)
        //{
        //    if (PlayerDirSprites == null || PlayerDirSprites.Length == 0) { return; }

        //    TileDir dir = DirectionHelper.GetTileDir(direction);

        //    currentSprite = PlayerDirSprites[(int)dir];

        //    GetComponent<SpriteRenderer>().sprite = currentSprite;
        //}

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

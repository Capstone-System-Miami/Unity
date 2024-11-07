// Authors: Layla Hoey, Lee St Louis
using System;
using System.Linq;
using SystemMiami.Enums;
using SystemMiami.Management;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [RequireComponent(typeof(Stats))]
    public class Combatant : MonoBehaviour, ITargetable, IDamageable, IHealable, IMovable
    {
        public OverlayTile CurrentTile;

        public Animator Animator;

        public DirectionalInfo DirectionInfo;

        public int ID { get; set; }

        private Vector2Int _direction;

        private MouseController _controller;
        private Stats _stats;
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

        // These Actions will be subscribed to by each ability's
        // CombatActions when they are equipped and targeting.
        // Which one they subscribe to will depend on the origin
        // of the pattern.
        public Action<DirectionalInfo> OnSubjectChanged;
        public Action<DirectionalInfo> OnDirectionChanged;

        public bool HasActed { get; set; }
        public bool IsMoving { get; set; }

        public MouseController Controller { get { return _controller; } }


        public Stats Stats { get { return _stats; } }

        private void OnEnable()
        {
            _stats = GetComponent<Stats>();
            _controller = GetComponent<MouseController>();
            _renderer = GetComponent<SpriteRenderer>();
            _defaultColor = _renderer.color;
            Animator = GetComponent<Animator>();
            currentSprite = GetComponent<SpriteRenderer>().sprite;

            if (_controller != null)
            {
                _controller.OnMouseTileChanged += setDirectionalInfo;
                _controller.OnPathTileChanged += SetDirectionalInfo;
            }

            GAME.MGR.CombatantDeath += onCombatantDeath;
        }

        private void OnDisable()
        {
            if (_controller != null)
            {
                _controller.OnMouseTileChanged -= setDirectionalInfo;
                _controller.OnPathTileChanged -= SetDirectionalInfo;
            }

            GAME.MGR.CombatantDeath -= onCombatantDeath;
        }

        protected virtual void Start()
        {
            if (CurrentTile == null)
            {
                Vector2Int gridPos = (Vector2Int)Coordinates.ScreenToIso(transform.position, 0);

                if (MapManager.MGR.map.ContainsKey(gridPos))
                {
                    CurrentTile = MapManager.MGR.map[gridPos];
                }
            }

            Health = new Resource(_stats.GetStat(StatType.MAX_HEALTH));
            Stamina = new Resource(_stats.GetStat(StatType.STAMINA));
            Mana = new Resource(_stats.GetStat(StatType.MANA));
            Speed = new Resource(_stats.GetStat(StatType.SPEED));

            initDirection();
        }

        private void Update()
        {
            if (Health.Get() == 0)
            {
                GAME.MGR.CombatantDeath.Invoke(this);
            }

            updateResources();
        }

        private void updateResources()
        {
            Health = new Resource(_stats.GetStat(StatType.MAX_HEALTH), Health.Get());
            Stamina = new Resource(_stats.GetStat(StatType.STAMINA), Stamina.Get());
            Mana = new Resource(_stats.GetStat(StatType.MANA), Mana.Get());
            Speed = new Resource(_stats.GetStat(StatType.SPEED), Speed.Get());
        }

        private void initDirection()
        {
            Vector2Int currentPos = (Vector2Int)CurrentTile.gridLocation;
            SetDirectionalInfo(new DirectionalInfo(currentPos, Vector2Int.zero));
        }

        /// <summary>
        /// Sets the combatant's directional info.
        /// For players, this is based on mouse position.
        /// For enemies, it's based on their target or movement.
        /// </summary>
        private void setDirectionalInfo(OverlayTile targetTile)
        {
            if (IsMoving) { return; }

            Vector2Int currentPos = (Vector2Int)CurrentTile.gridLocation;
            Vector2Int forwardPos;

            // If the combatant has a Mouse Controller (is the player)
            if (TryGetComponent(out _controller))
            {
                forwardPos = (Vector2Int)targetTile.gridLocation;
            }
            else
            {
                // For enemies, forward direction can be set manually
                // or towards the player. Here, we'll keep the current direction.
                return;
            }

            DirectionalInfo newDirection = new DirectionalInfo(currentPos, forwardPos);

            OnSubjectChanged?.Invoke(newDirection);

            if (newDirection.DirectionVec != DirectionInfo.DirectionVec)
            {
                SwapSprite(newDirection.DirectionVec);
                OnDirectionChanged?.Invoke(newDirection);
            }

            DirectionInfo = newDirection;
            _direction = newDirection.DirectionVec;
        }

        /// <summary>
        /// Allows setting directional info directly, useful for enemies.
        /// </summary>
        public void SetDirectionalInfo(DirectionalInfo newDirection)
        {
            DirectionInfo = newDirection;
            _direction = newDirection.DirectionVec;
            SwapSprite(_direction);

            OnSubjectChanged?.Invoke(newDirection);
            OnDirectionChanged?.Invoke(newDirection);
        }

        private void onCombatantDeath(Combatant deadCombatant)
        {
            if (deadCombatant == this)
            {
                Die();
            }
        }

        public void SwapSprite(Vector2Int direction)
        {
            if (PlayerDirSprites == null || PlayerDirSprites.Length == 0) { return; }

            TileDir dir = DirectionHelper.GetTileDir(direction);

            currentSprite = PlayerDirSprites[(int)dir];

            GetComponent<SpriteRenderer>().sprite = currentSprite;
        }

        #region ITargetable
        public void Target()
        {
            print($"{name} is being targeted");
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
            return (Vector2Int)CurrentTile.gridLocation;
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
                Vector2Int newPos = (Vector2Int)CurrentTile.gridLocation + boardDirection * distance;

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
        }

        public void ResetTurn()
        {
            Speed = new Resource(_stats.GetStat(StatType.SPEED));
            HasActed = false;
        }

        public virtual void Die()
        {
            if (TurnManager.Instance.playerCharacters.Contains(this))
            {
                // Player died
                Debug.Log($"{name} (Player) has died.");
                // Handle player death logic here
            }
            else if (TurnManager.Instance.enemyCharacters.Contains(this))
            {
                // Enemy died
                Debug.Log($"{name} (Enemy) has died.");
                TurnManager.Instance.enemyCharacters.Remove(this);
                Destroy(gameObject);
            }
            else
            {
                // Not found in any list
                Debug.Log($"{name} has died but was not found in any character list.");
                Destroy(gameObject);
            }
        }
    }
}

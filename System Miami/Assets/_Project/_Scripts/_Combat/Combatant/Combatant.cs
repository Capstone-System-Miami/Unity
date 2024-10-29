// Authors: Layla Hoey, Lee St Louis
using System;
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

        private Vector2Int _startFrameDirection;
        private Vector2Int _endFrameDirection;

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

        Action Pause; //??

        // These Actions will be subscribed to by each ability's
        // CombatActions when they are equppped and targeting.
        // Which one they subscribe to will depend on the origin
        // of the pattern.
        public Action<DirectionalInfo> OnSubjectChanged;
        public Action<DirectionalInfo> OnDirectionChanged;
        private Vector2Int _direction;
        public bool HasActed { get; set; }
        public bool IsMoving { get; set; }

        public MouseController Controller { get { return _controller; } }


        public Attributes Attributes;

        private void OnEnable()
        {
            _stats = GetComponent<Stats>();
            _controller = GetComponent<MouseController>();
            _renderer = GetComponent<SpriteRenderer>();
            _defaultColor = _renderer.color;
            Animator = GetComponent<Animator>();
            currentSprite = GetComponent<SpriteRenderer>().sprite;

            Attributes = GetComponent<Attributes>();

            if (_controller != null)
            {
                _controller.OnMouseTileChanged += setDirectionalInfo;
                _controller.OnPathTileChanged += setDirectionalInfo;
            }

            GAME.MGR.CombatantDeath += onCombatantDeath;
        }

        private void OnDisable()
        {
            if (_controller != null)
            {
                _controller.OnMouseTileChanged -= setDirectionalInfo;
                _controller.OnPathTileChanged -= setDirectionalInfo;
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
        }

        private void Update()
        {
            if (Health.Get() == 0)
            {
                GAME.MGR.CombatantDeath.Invoke(this);
            }
        }

        /// <summary>
        /// TODO:
        /// Because this is now tied into a MouseController event,
        /// it will do nothing for enemies, who don't have mouse controllers.
        /// This should be refactored once a
        /// movement system is finalized in a
        /// structured way.
        /// Sets the _player's directional info
        /// </summary>
        private void setDirectionalInfo(OverlayTile mouseTile)
        {
            if (IsMoving) { return; }

            Debug.LogWarning("Dir info changing");
            Vector2Int playerPos = (Vector2Int)CurrentTile.gridLocation;
            Vector2Int playerFwd;

            // If the _player has a Mouse Controller (is the user)
            if (TryGetComponent(out _controller))
            {
                playerFwd = (Vector2Int)mouseTile.gridLocation;
            }
            // If the _player doesn't have a mouse controller (is an enemy)
            else
            {
                // TODO: Set up an equivalent for enemies.
                // Right now, I guess this would happen in TurnManager?
                // That's were enemy movement is currently being handled.
                // Movement component should have a currentTile and a previous tile.
                // Enemies DirectionVec would be either
                    // [wherever they moved to] - [where they moved from] or
                    // [_player position] - [wherever they are right now]

                // For now, set the enemy's forward to the _player position
                playerFwd = (Vector2Int)TurnManager.Instance.playerCharacters[0].CurrentTile.gridLocation;
            }

            DirectionalInfo newDirection = new DirectionalInfo(playerPos, playerFwd);

            OnSubjectChanged?.Invoke(newDirection);

            if (newDirection.DirectionVec != DirectionInfo.DirectionVec)
            {
                SwapSprite(newDirection.DirectionVec);
                OnDirectionChanged?.Invoke(newDirection);
            }

            DirectionInfo = newDirection;
            _direction = newDirection.DirectionVec;
        }

        // As of 10/29/24 this is only being called when
        // OnPathTileChanged is invoked on mouse controller.
        private void setDirectionalInfo(DirectionalInfo newDirection)
        {
            DirectionInfo = newDirection;
            _direction = newDirection.DirectionVec;
            SwapSprite(_direction);
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
            print($"{name} is not longer highlighted");
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
                print($"{name} lost {amount} health.\n");

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
                print($"{name} gained full health.\n");

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
                print($"{name} gained {amount} health.\n");

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
                // TODO: Move the character to a certain position
                print($"{name} would be moved to {tilePos}, but this mechanic has not been implemented");
                return true;
            }
            else
            {
                print ($"{name} is sturdy");
                return false;
            }
        }

        public bool TryMoveInDirection(Vector2Int boardDirection, int distance)
        {
            if (IsMovable)
            {
                // TODO: Move the character a certain amount of tiles
                // in a certain boardDirection
                // if there are obstacles / edge of board in the way,
                // move as far as possible before hitting them.
                // Take damage if they hit an obstacle?

                Vector2Int newPos = (Vector2Int)CurrentTile.gridLocation + boardDirection * distance;

                print($"{name} would be moved to {newPos}, but this mechanic has not been implemented");
                return true;
            }
            else
            {
                print($"{name} is sturdy");
                return false;
            }
        }
        #endregion

        public void ResetTurn()
        {
            Speed = new Resource(_stats.GetStat(StatType.SPEED));
            HasActed = false;
        }

        public virtual void Die()
        {
            // I'm not sure that Enemy should be a derived class of Combatant.

            // If it wasn't, we would probably have something like this:
            // if player
                // game over
            // else
                // destroy game object

            // But since we don't we'll let the derived class handle it for now.
        }
    }
}

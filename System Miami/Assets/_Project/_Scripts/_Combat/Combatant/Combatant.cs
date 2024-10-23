// Authors: Layla Hoey
using SystemMiami.Utilities;
using System;
using Unity.VisualScripting;
using UnityEditor.UIElements;
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


        private Resource _health;
        private Resource _stamina;
        private Resource _mana;
        private Resource _speed;

        private bool _isDamageable;
        private bool _isMoveable;

        Action Pause; //??

        // These Actions will be subscribed to by each ability's
        // CombatActions when they are equppped and targeting.
        // Which one they subscribe to will depend on the origin
        // of the pattern.
        public Action<DirectionalInfo> OnSubjectChanged;
        public Action<DirectionalInfo> OnDirectionChanged;

        public bool HasActed { get; set; }
        public Resource Speed { get { return _speed; } }
        public MouseController Controller { get { return _controller; } }


        public Attributes Attributes;

        private void OnEnable()
        {
            _stats = GetComponent<Stats>();
            _controller = GetComponent<MouseController>();
            _renderer = GetComponent<SpriteRenderer>();
            _defaultColor = _renderer.color;
            Animator = GetComponent<Animator>();

            Attributes = GetComponent<Attributes>();
            _health = new Resource(_stats.GetStat(StatType.MAX_HEALTH));
            _stamina = new Resource(_stats.GetStat(StatType.STAMINA));
            _mana = new Resource(_stats.GetStat(StatType.MANA));
            _speed = new Resource(_stats.GetStat(StatType.SPEED));



            _controller.OnMouseTileChanged += setDirectionalInfo;
        }

        private void OnDisable()
        {
            _controller.OnMouseTileChanged -= setDirectionalInfo;
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


        }

        /// <summary>
        /// TODO:
        /// Because this is now tied into a MouseController event,
        /// it will do nothing for enemies, who don't have mouse controllers.
        /// This should be refactored once a
        /// movement system is finalized in a
        /// structured way.
        /// Sets the player's directional info
        /// </summary>
        private void setDirectionalInfo(OverlayTile mouseTile)
        {
            Debug.LogWarning("Dir info changing");
            Vector2Int playerPos = (Vector2Int)CurrentTile.gridLocation;
            Vector2Int playerFwd;

            // If the player has a Mouse Controller (is the user)
            if (TryGetComponent(out _controller))
            {
                playerFwd = (Vector2Int)mouseTile.gridLocation;
            }
            // If the player doesn't have a mouse controller (is an enemy)
            else
            {
                // TODO: Set up an equivalent for enemies.
                // Right now, I guess this would happen in TurnManager?
                // That's were enemy movement is currently being handled.
                // Movement component should have a currentTile and a previous tile.
                // Enemies DirectionVec would be either
                    // [wherever they moved to] - [where they moved from] or
                    // [player position] - [wherever they are right now]

                // For now, set the enemy's forward to the player position
                playerFwd = (Vector2Int)TurnManager.Instance.playerCharacters[0].CurrentTile.gridLocation;
            }

            DirectionalInfo newDirection = new DirectionalInfo(playerPos, playerFwd);

            OnSubjectChanged?.Invoke(newDirection);

            if (newDirection.DirectionVec != DirectionInfo.DirectionVec)
            {
                OnDirectionChanged?.Invoke(newDirection);
            }

            DirectionInfo = newDirection;
        }

        public void Heal()
        {
            print($"{name} gained full health.\n");

            _health.Reset();
        }

        public void Heal(float amount)
        {
            print($"{name} gained {amount} health.\n");

            _health.Gain(amount);
        }

        public void TakeDamage(float amount)
        {
            if (_isDamageable)
            {
                print($"{name} lost {amount} health.\n");

                _health.Lose(amount);
            }
            else
            {
                print($"{name} took no damage");
            }
        }

        public void GetPushed(int distance, Vector2Int directionVector)
        {
            if (_isMoveable)
            {
                print($"{name} got pushed {distance} tiles.\n");

                // TODO: Attach & implement IMovable to the character's tile movement controller.
                // This will be null right now
                IMovable controller = GetComponent<IMovable>();

                Vector2Int checkPos = (distance * directionVector) + controller.GetTilePos();

                controller?.TryMoveTo(new Vector2Int(checkPos.x, checkPos.y));

                // TODO:
                // Try to move to tile at checkPos
                // if there are obstacles / edge of board in the way, move as far as possible before hitting them.
                // Take damage if they hit an obstacle?
            }
            else
            {
                print($"{name} is sturdy");
            }
        }

        public void Target()
        {
            print($"{name} is being targeted");
        }

        public void Target(Color color)
        {
            print($"{name} is being targeted");
            _renderer.color = color;
        }

        public void UnTarget()
        {
            print($"{name} is not longer targeted");
            _renderer.color = _defaultColor;
        }
        public GameObject GameObject()
        {
            return gameObject;
        }

        public void Damage(float amount)
        {
            print($"{name} took {amount} damage");
        }

        public void ResetTurn()
        {
            _speed = new Resource(_stats.GetStat(StatType.SPEED));
            HasActed = false;
        }

        public Vector2Int GetTilePos()
        {
            return (Vector2Int)CurrentTile.gridLocation;
        }

        public bool TryMoveTo(Vector2Int tilePos)
        {
            // TODO: Move the character to a certain position

            print($"{name} is being moved to {tilePos}");
            return true;
        }

        public bool TryMoveInDirection(Vector2Int boardDirection, int distance)
        {
            Vector2Int newPos = (Vector2Int)CurrentTile.gridLocation + boardDirection * distance;

            // TODO: Move the character a certain amount of tiles
            // in a certain boardDirection

            print($"{name} is being moved to {newPos}");
            return true;
        }
    }
}

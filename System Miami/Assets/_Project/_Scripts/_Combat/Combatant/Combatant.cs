// Authors: Layla Hoey
using Unity.VisualScripting;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [RequireComponent(typeof(Stats))]
    public class Combatant : MonoBehaviour, ITargetable, IDamageable, IHealable
    {
        public OverlayTile CurrentTile;
        public bool HasActed {  get; private set; }

        public Vector2Int fakePlayerPos;
        public Vector2Int fakePlayerForward;

        private Stats _stats;

        private Resource _health;
        private Resource _stamina;
        private Resource _mana;
        private Resource _speed;

        private bool _isDamageable;
        private bool _isMoveable;


        private void Start()
        {
            _stats = GetComponent<Stats>();

            _health = new Resource(_stats.GetStat(StatType.MAX_HEALTH));
            _stamina = new Resource(_stats.GetStat(StatType.STAMINA));
            _mana = new Resource(_stats.GetStat(StatType.MANA));
            _speed = new Resource(_stats.GetStat(StatType.SPEED));
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
                IMovable controller = GetComponent<IMovable>();

                Vector2Int checkPos = (distance * directionVector) + controller.GetTilePos2D();

                controller.TryMoveTo(new Vector3Int(checkPos.x, checkPos.y, 0));

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

        public void UnTarget()
        {
            print($"{name} is not longer targeted");
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
    }
}

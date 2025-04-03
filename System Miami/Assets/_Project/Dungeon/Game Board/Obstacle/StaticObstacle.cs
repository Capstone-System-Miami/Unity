using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public class StaticObstacle : Obstacle, IDamageReceiver
    {
        [SerializeField] private bool isDamageable;

        private Resource health;

        public void Initialize(Sprite sprite, int maxHealth)
        {
            base.Initialize(sprite);
            isDamageable = (maxHealth != int.MaxValue) && (maxHealth > 0);

            ObstacleType = isDamageable
                ? ObstacleType.DYNAMIC_DAMAGEABLE
                : ObstacleType.DYNAMIC_UNDAMAGEABLE;

            health = new(maxHealth);
        }

        public override IDamageReceiver GetDamageInterface()
        {
            return isDamageable ? this : null;

        }

        public override IForceMoveReceiver GetMoveInterface()
        {
            return null;
        }

        public bool IsCurrentlyDamageable()
        {
            return isDamageable;
        }

        public void PreviewDamage(float amount, bool perTurn, int durationTurns)
        {
            log.print(
                $"{gameObject} will take {amount} damage from this action\n" +
                $"<UI NOT IMPLEMENTED>");
        }

        public void ReceiveDamage(float amount, bool perTurn, int durationTurns)
        {
            float prev = health.Get();
            health.Lose(amount);
            float current = health.Get();

            // TODO: Test
            log.print(
                $"{gameObject} Received Damage." +
                $"Prev Health :{prev}, After attack: {current}");
        }
    }
}

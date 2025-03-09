using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public class StaticObstacle : Obstacle, IDamageReceiver
    {
        [SerializeField] private bool isDamageable;

        protected override void Awake()
        {
            base.Awake();
            ObstacleType = isDamageable
                ? ObstacleType.STATIC_DAMAGEABLE
                : ObstacleType.STATIC_UNDAMAGEABLE;
        }

        public override IDamageReceiver GetDamageInterface()
        {
            return this;
        }

        public override IForceMoveReceiver GetMoveInterface()
        {
            return null;
        }

        public bool IsCurrentlyDamageable()
        {
            return true;
        }

        public void PreviewDamage(float amount)
        {
            log.print(
                $"{gameObject} will take {amount} damage from this action");
        }

        public void ReceiveDamage(float amount)
        {
            log.print(
                $"{gameObject} would have taken {amount} damage, but health " +
                $"has not been implemented on {GetType()} yet.");
        }
    }
}

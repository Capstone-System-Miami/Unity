using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public class DynamicObstacle : Obstacle, IDamageReceiver, IForceMoveReceiver
    {
        [SerializeField] private bool isDamageable;

        private Resource health;

        private bool isDamageOnBump;

        protected override void Update()
        {
            base.Update();
            if (TRIGGER_ForceMovePreviewTest)
            {
                TRIGGER_ForceMovePreviewTest = false;
                ForceMoveCommand testCommand = new(this, TEST_origin, TEST_moveType, TEST_distance);
                testCommand.Preview();
            }
        }

        public void Initialize(Sprite sprite, float maxHealth, bool isDamageOnBump)
        {
            base.Initialize(sprite);
            isDamageable = (maxHealth != int.MaxValue) && (maxHealth > 0);

            ObstacleType = isDamageable
                ? ObstacleType.DYNAMIC_DAMAGEABLE
                : ObstacleType.DYNAMIC_UNDAMAGEABLE;

            health = new(maxHealth);

            this.isDamageOnBump = isDamageOnBump;
        }

        #region ITargetable Overrides
        // ==================================================================
        public override IDamageReceiver GetDamageInterface()
        {
            return isDamageable ? this : null;
        }

        public override IForceMoveReceiver GetMoveInterface()
        {
            return this;
        }
        #endregion ITargetable Overrides


        #region IDamageable
        // ==================================================================
        public bool IsCurrentlyDamageable()
        {
            return isDamageable;
        }
        public void PreviewDamage(float amount)
        {
            // TODO: Implement
            log.print(
                $"{gameObject} will take {amount} damage from this action.\n" +
                $"<UI NOT IMPLEMENTED>");
        }
        public void ReceiveDamage(float amount)
        {
            float prev = health.Get();
            health.Lose(amount);
            float current = health.Get();

            // TODO: Test
            log.print(
                $"{gameObject} Received Damage." +
                $"Prev Health :{prev}, After attack: {current}");

        }
        #endregion IDamageable

        #region IForceMoveReceiver
        // ==================================================================
        public bool IsCurrentlyMovable()
        {
            return true;
        }

        public Vector2Int TEST_origin;
        public int TEST_distance;
        public MoveType TEST_moveType;
        public bool TRIGGER_ForceMovePreviewTest;

        /// <summary>
        /// TODO: Implement, Test
        /// </summary>
        /// 
        /// <param name="distance">
        /// In tiles </param>
        /// 
        /// <param name="directionVector">
        /// A direction in Worldspace (but really
        /// "BoardSpace" / "TileSpace" or whatever) </param>
        public void PreviewForceMove(OverlayTile destinationTile)
        {
            log.error(
                $"{name} is trying to priview movement to " +
                $"{destinationTile.name}, but its RecieveForceMove() method " +
                $"has not been implemented.",
                destinationTile);
        }

        public void RecieveForceMove(OverlayTile destinationTile)
        {
            log.error(
                $"{name} is trying to move to {destinationTile.name}, but " +
                $"its RecieveForceMove() method has not been implemented.",
                destinationTile);
        }
        public void PreviewForceMove(int distance, Vector2Int directionVector)
        {
            // TODO: Should this param be Direction of attack, where we would
            directionVector = DirectionHelper.GetNormalized(directionVector);

            // then have  vv this vv  ?
            // TODO: Direction of movement is the opposite?
            Vector2Int movementVector = directionVector * -1;

            log.print(
                $"{gameObject} will move {distance} " +
                $"tiles in ({DirectionHelper.GetTileDir(movementVector)}) from this action.\n" +
                $"<UI NOT IMPLEMENTED>");
        }

        public void ReceiveForceMove(int distance, Vector2Int directionVector)
        {
            // TODO: Should this param be Direction of attack, where we would
            directionVector = DirectionHelper.GetNormalized(directionVector);

            // then have  vv this vv  ?
            // TODO: Direction of movement is the opposite?
            Vector2Int movementVector = directionVector * -1;

            log.print(
                $"{gameObject} is moving {distance} " +
                $"tiles in ({DirectionHelper.GetTileDir(movementVector)}) due to " +
                $"an inflicted CombatAction\n" +
                $"<NOT IMPLEMENTED>");
        }

        #endregion IForceMoveReceiver
    }
}

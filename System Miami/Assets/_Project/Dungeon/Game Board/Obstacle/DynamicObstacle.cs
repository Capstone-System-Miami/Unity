using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using Unity.VisualScripting;
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
                PreviewForceMove(TEST_origin, TEST_distance, TEST_moveType);
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
        public void PreviewForceMove(Vector2Int origin, int distance, MoveType type)
        {
            Vector2Int dirVec = DirectionHelper.GetDirectionVec(origin, (Vector2Int)PositionTile.GridLocation);

            if (type == MoveType.PULL)
            {
                dirVec *= -1;
            }

            Vector2Int targetPos = (this as ITargetable).BoardPos + (dirVec * distance);

            int adjustedX = System.Math.Clamp(targetPos.x, MapManager.MGR.Bounds.xMin, MapManager.MGR.Bounds.xMax);
            int adjustedY = System.Math.Clamp(targetPos.y, MapManager.MGR.Bounds.yMin, MapManager.MGR.Bounds.yMax);

            targetPos = new(adjustedX, adjustedY);

            if (MapManager.MGR.TryGetTile(targetPos, out OverlayTile targetTile))
            {
                log.error($"{name} would move to {targetTile.gameObject.name}", targetTile);
            }
            else
            {
                log.error($"{name} didn't find a tile...", this);
            }

        }

        public void ReceiveForceMove(Vector2Int origin, int distance, MoveType type)
        {
            throw new System.NotImplementedException();
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

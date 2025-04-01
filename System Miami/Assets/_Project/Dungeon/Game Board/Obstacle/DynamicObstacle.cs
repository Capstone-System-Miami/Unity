using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public class DynamicObstacle : Obstacle, IDamageReceiver, IForceMoveReceiver
    {
        protected const float PLACEMENT_RANGE = 0.0001f;

        [SerializeField] private float moveSpeed = 2f;

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
            if (TRIGGER_ForceMoveExecuteTest)
            {
                TRIGGER_ForceMoveExecuteTest = false;
                ForceMoveCommand testCommand = new(this, TEST_origin, TEST_moveType, TEST_distance);
                testCommand.Execute();
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
        // =====================================================================
        public bool IsCurrentlyMovable()
        {
            return true;
        }

        public Vector2Int TEST_origin;
        public int TEST_distance;
        public MoveType TEST_moveType;
        public float TEST_movementTimeout;
        public bool TRIGGER_ForceMovePreviewTest;
        public bool TRIGGER_ForceMoveExecuteTest;

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
            MovementPath pathToTile = new(PositionTile, destinationTile, true);

            StartCoroutine(TEST_ShowMovementPath(pathToTile));
            //log.error(
            //    $"{name} is trying to priview movement to " +
            //    $"{destinationTile.name}, but its RecieveForceMove() method " +
            //    $"has not been implemented.",
            //    destinationTile);
        }


        public void RecieveForceMove(OverlayTile destinationTile)
        {
            MovementPath pathToDestination = new(PositionTile, destinationTile, true);
            StartCoroutine(ForcedMovementExecution(pathToDestination, TEST_movementTimeout));
        }

        private IEnumerator TEST_ShowMovementPath(MovementPath path)
        {
            float duration = 1f;

            path.HighlightValidMoves(Color.cyan);
            path.DrawArrows();

            while (duration > 0)
            {
                duration -= Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }

            path.Unhighlight();
            path.UnDrawAll();
        }
        #endregion IForceMoveReceiver

        #region Board Movement
        // =====================================================================
        public void StepTowards(OverlayTile target)
        {
            float stepDistance = moveSpeed * Time.deltaTime;

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

        private IEnumerator ForcedMovementExecution(MovementPath pathToTile, float timeout)
        {
            /// Copy path so we can remove elements as we go.
            List<OverlayTile> pathToConsume = new(pathToTile.ForMovement);

            /// Highlight the path
            pathToTile.HighlightValidMoves(Color.red);
            pathToTile.DrawArrows();

            while (pathToConsume.Count > 0 && timeout > 0)
            {
                StepTowards(pathToConsume[0]);

                if (InPlacementRangeOf(pathToConsume[0]))
                {
                    Debug.Log($"{name} in placement range of {pathToConsume[0].name}");
                    if (!MapManager.MGR.TryPlaceOnTile(this, pathToConsume[0]))
                    {
                        Debug.LogError(
                            $"{name} " +
                            $"was not able to be placed on" +
                            $"{pathToConsume[0].gameObject}.");
                        break;
                    }

                    pathToConsume.RemoveAt(0);

                    Debug.Log($"{name} moved along path.");
                }

                timeout -= Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }

            pathToTile.UnDrawAll();
        }
        #endregion // Board Movement
    }
}

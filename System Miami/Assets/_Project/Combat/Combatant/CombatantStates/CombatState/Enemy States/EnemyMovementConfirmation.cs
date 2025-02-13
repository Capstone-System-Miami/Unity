using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class EnemyMovementConfirmation : MovementConfirmation
    {
        private const float DELAY = 0.5f;
        private CountdownTimer delayTimer;


        public EnemyMovementConfirmation(
            Combatant combatant,
            MovementPath path)
                : base(combatant, path)
        { }

        public override void OnEnter()
        {
            base.OnEnter();

            delayTimer = new(combatant, DELAY);
            delayTimer.Start();
        }

        /// <summary>
        /// Will return FALSE every time
        /// because the enemy makes its
        /// movement selection based on
        /// (a) randomness
        /// (b) player location within range,
        /// but this decision happens on the EnemyCombatant
        /// </summary>
        protected override bool CancelSelection()
        {
            return false;
        }

        /// <summary>
        /// Will return TRUE every time
        /// because the enemy makes its
        /// movement selection based on
        /// (a) randomness
        /// (b) player location within range,
        /// but this decision happens in the
        /// Selection state
        /// </summary>
        protected override bool ConfirmSelection()
        {
            return delayTimer.IsFinished;
        }
    }
}

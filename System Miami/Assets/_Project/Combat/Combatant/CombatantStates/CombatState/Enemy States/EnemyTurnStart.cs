using System.Collections;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class EnemyTurnStart : TurnStart
    {
        private const float DELAY = 0.5f;

        private CountdownTimer delayTimer;

        public EnemyTurnStart(Combatant combatant)
            : base(combatant) { }


        public override void OnEnter()
        {
            base.OnEnter();
            InputPrompts =
                $"{combatant.name} Turn Start!";

            delayTimer = new(combatant, DELAY);
            delayTimer.Start();
        }

        /// <inheritdoc/>
        /// <summary>
        /// Enemies won't "request" to proceed until their 
        /// delay timer is up.
        /// </summary>
        /// <returns></returns>
        protected override bool ProceedRequested()
        {
            return delayTimer.IsFinished;
        }
    }
}

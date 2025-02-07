using System.Collections;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class EnemyTurnStart : TurnStart
    {
        private float delay = .5f;
        private bool flag_proceed;

        public EnemyTurnStart(Combatant combatant)
            : base(combatant) { }


        public override void OnEnter()
        {
            base.OnEnter();
            InputPrompts =
                $"{combatant.name} Turn Start!";
        }
        protected override bool ProceedRequested()
        {
            return flag_proceed;
        }

        IEnumerator delayForMessage()
        {
            yield return new WaitForSeconds(delay);
            flag_proceed = true;
        }

        // NOTE
        // Enemies could use
        // () => ( (float waitedFor) > (float timerTime) )
        // as a condition
        //
        // could also use
        // () => ( (int tilesChecked) > (int tilesToCheck) )
        // as a condition
    }
}

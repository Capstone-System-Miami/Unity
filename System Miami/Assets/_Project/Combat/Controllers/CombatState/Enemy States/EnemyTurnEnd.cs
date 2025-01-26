using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class EnemyTurnEnd : TurnEnd
    {
        public EnemyTurnEnd(Combatant combatant)
            : base (combatant) { }

        protected override bool Proceed()
        {
            // Don't wait for anything
            return true;
        }

        protected override void GoToIdle()
        {
            machine.SetState(new EnemyIdle(combatant));
        }
    }
}

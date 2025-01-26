using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class EnemyActionExecution : ActionExecution
    {
        public EnemyActionExecution(Combatant combatant)
            : base(combatant) { }

        protected override void GoToEndTurn()
        {
            machine.SetState(new EnemyTurnEnd(combatant));
        }
    }
}

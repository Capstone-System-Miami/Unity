using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class PlayerActionExecution : ActionExecution
    {
        public PlayerActionExecution(Combatant combatant)
            : base(combatant) { }

        protected override void GoToEndTurn()
        {
            machine.SetState(new PlayerTurnEnd(combatant));
        }
    }
}

using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class PlayerTurnEnd : TurnEnd
    {
        public PlayerTurnEnd(Combatant combatant)
            : base(combatant) { }

        protected override bool Proceed()
        {
            // Wait for input?
            return true;
        }

        protected override void GoToIdle()
        {
            machine.SetState(new PlayerIdle(combatant));
        }
    }
}

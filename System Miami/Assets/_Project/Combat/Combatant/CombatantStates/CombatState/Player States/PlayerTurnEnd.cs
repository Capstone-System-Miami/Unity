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
            // Wait for input
            return Input.GetKeyDown(KeyCode.Return);
        }
    }
}

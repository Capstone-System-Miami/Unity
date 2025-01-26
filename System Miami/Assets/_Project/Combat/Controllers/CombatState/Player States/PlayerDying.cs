using UnityEngine;
using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public class PlayerDying : Dying
    {
        public PlayerDying(Combatant combatant)
            : base (combatant) { }


        protected override bool Proceed()
        {
            // Check animation flag?
            return true;
        }

        protected override void GoToDead()
        {
            machine.SetState(new PlayerDead(combatant));
        }
    }
}

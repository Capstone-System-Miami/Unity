using UnityEngine;
using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public class PlayerDying : Dying
    {
        public PlayerDying(Combatant combatant)
            : base (combatant) { }


        protected override bool DeadRequested()
        {
            // Check animation flag?
            return true;
        }
    }
}
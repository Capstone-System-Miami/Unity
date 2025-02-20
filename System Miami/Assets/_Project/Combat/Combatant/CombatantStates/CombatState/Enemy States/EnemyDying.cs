using UnityEngine;
using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public class EnemyDying : Dying
    {
        public EnemyDying(Combatant combatant)
            : base (combatant) { }

        protected override bool DeadRequested()
        {
            // check animation flags?
            return true;
        }
    }
}

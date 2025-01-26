using UnityEngine;
using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public class EnemyDying : Dying
    {
        public EnemyDying(Combatant combatant)
            : base (combatant) { }

        protected override bool Proceed()
        {
            // check animation flags?
            return true;
        }

        protected override void GoToDead()
        {
            machine.SetState(new EnemyDead(combatant));
        }
    }
}

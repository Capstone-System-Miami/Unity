using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class EnemyActionConfirmation : ActionConfirmation
    {
        public EnemyActionConfirmation(Combatant combatant)
            : base(combatant) { }

        protected override bool ConfirmSelection()
        {
            // always? bypass this state?
            return true;
        }

        protected override bool CancelSelection()
        {
            return false;
        }
    }
}

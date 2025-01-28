using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class EnemyActionConfirmation : ActionConfirmation
    {
        public EnemyActionConfirmation(Combatant combatant, CombatAction combatAction)
            : base(combatant, combatAction) { }

        protected override bool ConfirmSelection()
        {
            // always? bypass this state?
            return true;
        }

        protected override bool CancelConfirmation()
        {
            return false;
        }
    }
}

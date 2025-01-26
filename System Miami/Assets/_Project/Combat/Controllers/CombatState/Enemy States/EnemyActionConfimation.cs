using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class EnemyActionConfimation : ActionConfirmation
    {
        public EnemyActionConfimation(Combatant combatant)
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

        protected override void GoToActionExecution()
        {
            machine.SetState(new EnemyActionExecution(combatant));
        }

        protected override void GoToActionEquipped()
        {
            machine.SetState(new EnemyActionConfimation(combatant));
        }
    }
}

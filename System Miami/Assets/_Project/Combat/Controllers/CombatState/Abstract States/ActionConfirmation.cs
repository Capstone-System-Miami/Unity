using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public abstract class ActionConfirmation : CombatantState
    {
        public ActionConfirmation(Combatant combatant)
            : base(combatant, Phase.Action) { }

        public override void MakeDecision()
        {
            if (ConfirmSelection())
            {
                GoToActionExecution();
                return;
            }

            if (CancelSelection())
            {
                GoToActionEquipped();
                return;
            }
        }

        // Decision
        protected abstract bool ConfirmSelection();
        protected abstract bool CancelSelection();

        // Outcomes
        protected abstract void GoToActionExecution();
        protected abstract void GoToActionEquipped();
    }
}

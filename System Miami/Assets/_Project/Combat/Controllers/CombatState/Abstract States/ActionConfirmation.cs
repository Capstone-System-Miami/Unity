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
                SwitchState(factory.ActionExecution());
                return;
            }

            if (CancelSelection())
            {
                SwitchState(factory.ActionEquipped());
                return;
            }


            // Should this be checking 'input'
            // storing a requested state transition (if any),
            // 
            // Then there's another function that checks
            // if there is request for a state transition,
            // which checks the conditions for the
            // requested transition?
        }

        // Decision
        protected abstract bool ConfirmSelection();
        protected abstract bool CancelSelection();
    }
}

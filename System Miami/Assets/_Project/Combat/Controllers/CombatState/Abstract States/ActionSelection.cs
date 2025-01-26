using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public abstract class ActionSelection : CombatantState
    {
        public ActionSelection(Combatant combatant)
            : base(combatant, Phase.Action) { }

        public override void Update()
        {
            // TODO
            //UpdateFocusedTile();
        }

        public override void MakeDecision()
        {
            if (ActionSelected())
            {
                GoToActionEquipped();
                return;
            }

            if (SkipPhase())
            {
                GoToEndTurn();
                return;
            }
        }

        // Decision
        protected abstract bool ActionSelected();
        protected abstract bool SkipPhase();

        // Outcomes
        protected abstract void GoToActionEquipped();
        protected abstract void GoToEndTurn();
    }
}

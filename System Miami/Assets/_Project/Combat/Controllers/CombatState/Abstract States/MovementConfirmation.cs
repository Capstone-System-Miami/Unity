using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public abstract class MovementConfirmation : CombatantState
    {
        protected MovementPath path;

        protected MovementConfirmation(
            Combatant combatant,
            MovementPath path)
                : base(
                    combatant,
                    Phase.Movement
                )
        {
            this.path = path;
        }

        public override void OnEnter()
        {
            path.Draw();
        }

        public override void MakeDecision()
        {
            if (ConfirmSelection())
            {
                GoToMovementExecution();
                return;
            }

            if (CancelSelection())
            {
                GoToMovementTileSelection();
                return;
            }
        }

        // Decision
        protected abstract bool ConfirmSelection();
        protected abstract bool CancelSelection();

        // Outcomes
        protected abstract void GoToMovementExecution();
        protected abstract void GoToMovementTileSelection();
    }
}

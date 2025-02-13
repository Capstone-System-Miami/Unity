using SystemMiami.CombatSystem;
using UnityEngine;

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
            base.OnEnter();

            /// Preview the path to the selected tile.
            path.DrawArrows();
            path.HighlightValidMoves(Color.cyan);

            InputPrompts =
                "Hover over a tile to preview a path to it.\n" +
                "Click to lock your path.\n" +
                "(You will still be able to change your mind)\n";
        }

        public override void MakeDecision()
        {
            if (ConfirmSelection())
            {
                SwitchState(factory.MovementExecution(path));
                return;
            }

            if (CancelSelection())
            {
                SwitchState(factory.MovementTileSelection());
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            path.UnDrawAll();
        }

        // Decision
        protected abstract bool ConfirmSelection();
        protected abstract bool CancelSelection();
    }
}

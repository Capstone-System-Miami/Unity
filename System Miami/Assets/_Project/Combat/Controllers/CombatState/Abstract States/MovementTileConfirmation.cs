using System.Collections.Generic;
using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public abstract class MovementTileConfirmation : CombatantState
    {
        MovementPath path;

        protected MovementTileConfirmation(
            Combatant combatant,
            MovementPath path)
                : base(
                    combatant,
                    Phase.Movement
                )
        {
            this.path = path;
        }

        public override void aOnEnter()
        {
        }

        public override void bUpdate()
        {
            path.Draw();
        }

        public override abstract void cMakeDecision();

        public override void eOnExit()
        {
        }

        protected abstract bool ConfirmSelection();
        protected abstract bool CancelSelection();
    }
}

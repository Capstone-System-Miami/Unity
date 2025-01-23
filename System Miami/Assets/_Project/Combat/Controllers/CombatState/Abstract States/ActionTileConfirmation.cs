using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class ActionTileConfirmation : CombatantState
    {
        public ActionTileConfirmation(CombatantStateMachine machine)
            : base(machine, Phase.Action) { }

        public override void aOnEnter()
        {
            throw new System.NotImplementedException();
        }

        public override void bUpdate()
        {
            throw new System.NotImplementedException();
        }

        public abstract override void cMakeDecision();

        public override void eOnExit()
        {
            throw new System.NotImplementedException();
        }

        protected abstract bool ConfirmSelection();
        protected abstract bool CancelSelection();
    }
}

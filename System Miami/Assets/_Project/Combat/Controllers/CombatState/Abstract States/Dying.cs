using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class Dying : CombatantState
    {
        protected Dying(CombatantStateMachine machine)
            : base(machine, Phase.None) { }

        public override void aOnEnter()
        {
            throw new System.NotImplementedException();
        }

        public override void bUpdate()
        {
            throw new System.NotImplementedException();
        }

        public override void cMakeDecision()
        {
            throw new System.NotImplementedException();
        }

        public override void eOnExit()
        {
            throw new System.NotImplementedException();
        }
    }
}

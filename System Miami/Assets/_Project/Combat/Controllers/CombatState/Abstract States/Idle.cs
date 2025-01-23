using SystemMiami.CombatSystem;
using UnityEngine;
using SystemMiami.CombatRefactor;

namespace SystemMiami
{
    public class Idle : CombatantState
    {
        public Idle(CombatantStateMachine context)
            : base(context, Phase.Movement) { }

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

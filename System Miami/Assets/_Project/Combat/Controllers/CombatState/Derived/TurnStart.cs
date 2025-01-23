using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class TurnStart : CombatantState
    {
        protected TurnStart(CombatantStateMachine context)
            : base(context, Phase.None) { }

        public override void aOnEnter()
        {
            Debug.Log($"{machine.name} starting turn");
            machine.combatant.ResetTurn();
            
            machine.combatant.ResetTileFlags();
        }

        public override void bUpdate()
        {
        }

        public override abstract void cMakeDecision();

        public override void eOnExit()
        {
        }
    }
}

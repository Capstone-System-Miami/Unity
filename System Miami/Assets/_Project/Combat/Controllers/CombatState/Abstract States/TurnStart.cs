using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class TurnStart : CombatantState
    {
        protected TurnStart(Combatant combatant)
            : base(combatant, Phase.None) { }

        public override void aOnEnter()
        {
            Debug.Log($"{machine.name} starting turn");
            machine.combatant.ResetTurn();
            
            machine.combatant.ResetTileContext();
        }

        public override void bUpdate()
        {
        }

        public override abstract void cMakeDecision();

        public override void eOnExit()
        {
        }

        public abstract void GoToMovementTileSelect();
    }
}

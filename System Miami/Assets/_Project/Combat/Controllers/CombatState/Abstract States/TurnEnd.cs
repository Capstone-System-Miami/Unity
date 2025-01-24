using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class TurnEnd : CombatantState
    {
        public TurnEnd(Combatant combatant)
            : base(combatant, Phase.None) { }

        public override void aOnEnter()
        {
            Debug.Log($"{machine.name}Calling end of turn");
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

using SystemMiami.CombatSystem;
using UnityEngine;
using SystemMiami.CombatRefactor;

namespace SystemMiami.CombatRefactor
{
    public abstract class Idle : CombatantState
    {
        public Idle(Combatant combatant)
            : base(combatant, Phase.Movement) { }
    }

    public class PlayerIdle : Idle
    {
        public PlayerIdle(Combatant combatant)
            : base(combatant) { }

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

    public class EnemyIdle : Idle
    {
        public EnemyIdle(Combatant combatant)
            : base(combatant) { }

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

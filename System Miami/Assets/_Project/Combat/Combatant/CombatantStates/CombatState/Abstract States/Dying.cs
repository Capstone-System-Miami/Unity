using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class Dying : CombatantState
    {
        private float deathDuration;
        private CountdownTimer deathTimer;
        private Conditions readyToDie = new();

        protected Dying(Combatant combatant)
            : base(combatant, Phase.None) { }

        public override void OnEnter()
        {
            base.OnEnter();

            deathTimer = new(combatant, 0.1f);
            readyToDie.Add( () => deathTimer.IsStarted );
            readyToDie.Add( () => deathTimer.IsFinished );

            deathTimer.Start();
            // start animation?
        }

        public override void MakeDecision()
        {
            if (DeadRequested() && readyToDie.AllMet())
            {
                SwitchState(factory.Dead());
            }
        }

        protected abstract bool DeadRequested();
    }
}

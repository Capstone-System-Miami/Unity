using SystemMiami.CombatSystem;
using SystemMiami.Management;

namespace SystemMiami.CombatRefactor
{
    public abstract class Idle : CombatantState
    {
        public Idle(Combatant combatant)
            : base(combatant, Phase.None) { }


        public override void MakeDecision()
        {
            base.MakeDecision();
            
            if (DyingRequested())
            {
                SwitchState(factory.Dying());
            }
        }

        protected bool DyingRequested()
        {
            return combatant.Health.Get() == 0;
        }
    }
}

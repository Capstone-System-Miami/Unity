using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public abstract class Idle : CombatantState
    {
        public Idle(Combatant combatant)
            : base(combatant, Phase.None) { }
    }
}

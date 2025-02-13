using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public class PlayerDead : Dead
    {
        public PlayerDead(Combatant combatant)
            : base(combatant) { }
    }
}

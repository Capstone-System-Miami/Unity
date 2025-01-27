using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public class EnemyMovementExecution : MovementExecution
    {
        public EnemyMovementExecution(
            Combatant combatant,
            MovementPath path)
                : base (combatant, path) { }
    }
}

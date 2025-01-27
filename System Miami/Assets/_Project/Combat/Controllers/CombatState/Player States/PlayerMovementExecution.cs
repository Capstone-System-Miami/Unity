using SystemMiami.CombatSystem;
using SystemMiami;
using System.Linq;

namespace SystemMiami.CombatRefactor
{
    public class PlayerMovementExecution : MovementExecution
    {
        public PlayerMovementExecution(Combatant combatant, MovementPath path)
            : base(combatant, path) { }
    }
}

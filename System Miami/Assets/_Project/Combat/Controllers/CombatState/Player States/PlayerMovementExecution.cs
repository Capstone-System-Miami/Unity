using SystemMiami.CombatSystem;
using SystemMiami;
using System.Linq;

namespace SystemMiami.CombatRefactor
{
    public class PlayerMovementExecution : MovementExecution
    {
        public PlayerMovementExecution(Combatant combatant, MovementPath path)
            : base(combatant, path) { }

        protected override bool MoveAgain()
        {
            if (combatant.Speed.Get() > 0)
            {
                return true;
            }

            return false;
        }

        protected override void GoToMovementTileSelection()
        {
            machine.SetState(new PlayerMovementTileSelection(combatant));
        }

        protected override void GoToActionSelection()
        {
            machine.SetState(new PlayerActionSelection(combatant));
        }

        protected override void GoToTurnEnd()
        {
            machine.SetState(new PlayerTurnEnd(combatant));
        }
    }
}

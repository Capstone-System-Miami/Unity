using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public class EnemyMovementExecution : MovementExecution
    {
        public EnemyMovementExecution(
            Combatant combatant,
            MovementPath path)
                : base (combatant, path) { }

        protected override bool MoveAgain()
        {
            // never, right?
            return false;
        }

        protected override void GoToActionSelection()
        {
            machine.SetState(new EnemyActionSelection(combatant));
        }

        protected override void GoToMovementTileSelection()
        {
            machine.SetState(new EnemyMovementTileSelection(combatant));
        }

        protected override void GoToTurnEnd()
        {
            machine.SetState(new EnemyTurnEnd(combatant));
        }
    }
}

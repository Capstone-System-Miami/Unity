using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public class EnemyTurnStart : TurnStart
    {
        public EnemyTurnStart(Combatant combatant)
            : base(combatant) { }

        protected override bool Proceed()
        {
            return true;
        }

        public override void GoToMovementTileSelect()
        {
            machine.SetState(new EnemyMovementTileSelection(combatant));
        }
    }
}

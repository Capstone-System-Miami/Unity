using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public class EnemyTurnStart : TurnStart
    {
        public EnemyTurnStart(Combatant combatant)
            : base(combatant) { }

        public override void MakeDecision()
        {
            GoToMovementTileSelect();
        }

        public override void GoToMovementTileSelect()
        {
            machine.SetState(new EnemyMovementTileSelection(combatant));
        }
    }
}

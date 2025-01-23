namespace SystemMiami.CombatRefactor
{
    public class EnemyTurnStart : TurnStart
    {
        public EnemyTurnStart(CombatantStateMachine machine)
            : base(machine) { }

        public override void cMakeDecision()
        {
            GoToMovementTileSelect();
        }

        public override void GoToMovementTileSelect()
        {
            machine.SwitchState(new EnemyMovementTileSelection(machine));
        }
    }
}

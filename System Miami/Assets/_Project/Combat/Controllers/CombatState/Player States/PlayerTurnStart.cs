namespace SystemMiami.CombatRefactor
{
    public class PlayerTurnStart : TurnStart
    {
        public PlayerTurnStart(CombatantStateMachine machine)
            : base(machine) { }

        public override void cMakeDecision()
        {
            GoToMovementTileSelect();
        }

        public override void GoToMovementTileSelect()
        {
            machine.SetState(new PlayerMovementTileSelection(machine));
        }
    }
}

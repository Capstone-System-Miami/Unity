namespace SystemMiami.CombatRefactor
{
    public class PlayerTurnStart : TurnStart
    {
        public PlayerTurnStart(CombatantStateMachine machine)
            : base(machine) { }

        public override void cMakeDecision()
        {
            machine.SwitchState(new PlayerMovementTileSelection(machine));
        }
    }
}

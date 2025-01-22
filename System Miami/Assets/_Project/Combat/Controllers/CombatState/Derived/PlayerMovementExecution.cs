namespace SystemMiami.CombatRefactor
{
    public class PlayerMovementExecution : MovementExecution
    {
        public PlayerMovementExecution(CombatantStateMachine machine)
            : base(machine) { }

        public override void cMakeDecision()
        {
            if (!destinationReached()) { return; }

            // If they have speed left
            if (machine.combatant.Speed.Get() > 0)
            {
                // Go back to tile selection for movement
                machine.SwitchState(new PlayerMovementTileSelection(machine));
                return;
            }
            else
            {
                // Proceed to CombatAction selection
                machine.SwitchState(new PlayerActionSelection(machine));
                return;
            }
            
        }
    }
}

namespace SystemMiami.CombatRefactor
{
    public class EnemyMovementExecution : MovementExecution
    {
        public EnemyMovementExecution(CombatantStateMachine machine)
            : base (machine) { }

        public override void cMakeDecision()
        {
            if (!destinationReached()) { return; }

            // If they have speed left
            if (machine.combatant.Speed.Get() > 0)
            {
                // Go back to tile selection for movement
                machine.SetState(new PlayerMovementTileSelection(machine));
            }
            else
            {
                // Proceed to CombatAction selection
                machine.SetState(new PlayerActionSelection(machine));
            }
        }
    }
}

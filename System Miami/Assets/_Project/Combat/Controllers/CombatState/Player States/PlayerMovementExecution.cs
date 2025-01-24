using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public class PlayerMovementExecution : MovementExecution
    {
        public PlayerMovementExecution(Combatant combatant)
            : base(combatant) { }

        public override void cMakeDecision()
        {
            if (!destinationReached()) { return; }

            // If they have speed left
            if (machine.combatant.Speed.Get() > 0)
            {
                // Go back to tile selection for movement
                machine.SetState(new PlayerMovementTileSelection(machine));
                return;
            }
            else
            {
                // Proceed to CombatAction selection
                machine.SetState(new PlayerActionSelection(machine));
                return;
            }
            
        }
    }
}

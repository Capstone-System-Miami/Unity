using SystemMiami.CombatSystem;
using SystemMiami.Management;

namespace SystemMiami.CombatRefactor
{
    public class PlayerIdle : Idle
    {
        public PlayerIdle(Combatant combatant)
            : base(combatant) { }

        public override void OnEnter()
        {
            base.OnEnter();

            if (TurnManager.MGR.CurrentTurnOwner == null)
            {
                UI.MGR.ClearInputPrompt();
                return;
            }

            InputPrompts = 
                $"Enemies are taking their turns...\n";

            UI.MGR.UpdateInputPrompt(InputPrompts);
        }
    }
}

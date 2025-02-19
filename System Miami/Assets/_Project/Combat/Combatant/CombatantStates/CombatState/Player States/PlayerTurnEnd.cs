using SystemMiami.CombatSystem;
using SystemMiami.Management;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class PlayerTurnEnd : TurnEnd
    {
        public PlayerTurnEnd(Combatant combatant)
            : base(combatant) { }

        public override void OnEnter()
        {
            base.OnEnter();

            InputPrompts = 
                $"Turn Over.\n\n" +
                $"Press Enter to proceed.";

            UI.MGR.UpdateInputPrompt(InputPrompts);
        }

        protected override bool Proceed()
        {
            // Wait for input
            return Input.GetKeyDown(KeyCode.Return);
        }
    }
}

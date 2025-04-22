using SystemMiami.CombatSystem;
using SystemMiami.Management;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class PlayerTurnStart : TurnStart
    {
        public PlayerTurnStart(Combatant combatant)
            : base(combatant) { }

        public override void OnEnter()
        {
            base.OnEnter();
            InputPrompts =
                $"Turn Start!\n\n" +
                $"Press {combatant.flowKey} to begin.";

            UI.MGR.UpdateInputPrompt(InputPrompts);
        }

        protected override bool ProceedRequested()
        {
            return Input.GetKeyDown(combatant.flowKey);
        }
    }
}

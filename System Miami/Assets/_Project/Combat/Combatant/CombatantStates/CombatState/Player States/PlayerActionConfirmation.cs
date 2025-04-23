using SystemMiami.CombatSystem;
using SystemMiami.Management;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class PlayerActionConfirmation : ActionConfirmation
    {
        public PlayerActionConfirmation(Combatant combatant, CombatAction combatAction)
            : base(combatant, combatAction) { }


        public override void OnEnter()
        {
            base.OnEnter();

            // It still feels clunky to have to use the
            // Database to get the name of the action.
            string actionName = Database.MGR.GetDataWithJustID(combatAction.ID).Name;

            InputPrompts =
                // $"(NOT IMPLEMENTED) Hover over a target to " +
                // $"preview {actionName}'s effects.\n\n" +
                $"Press {combatant.flowKey} to confirm your targets\n" +
                $"and use {actionName}\n\n" +
                $"Or Right Click to select new targets.";

            UI.MGR.UpdateInputPrompt(InputPrompts);
        }

        protected override bool CancelConfirmation()
        {
            // Player right clicks
            return Input.GetMouseButtonDown(1);
        }

        protected override bool ConfirmSelection()
        {
            // Player presses enter
            return Input.GetKeyDown(combatant.flowKey);
        }
    }
}

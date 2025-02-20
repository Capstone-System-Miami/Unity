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

            InputPrompts =
                $"Press Enter to confirm your targets,\n" +
                $"and attack with {Database.MGR.GetDataWithJustID(combatAction.ID).Name}\n\n" +
                $"or Right Click to select new targets.";

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
            return Input.GetKeyDown(KeyCode.Return);
        }
    }
}

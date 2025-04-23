using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.Management;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class PlayerMovementConfirmation : MovementConfirmation
    {
        public PlayerMovementConfirmation(
            Combatant combatant,
            MovementPath path)
                : base(
                    combatant,
                    path
                )
        { }

        public override void OnEnter()
        {
            base.OnEnter();

            InputPrompts =
                $"Press {combatant.flowKey} to Move,\n\n" +
                $"Or Right Click to select a different path.";

            UI.MGR.UpdateInputPrompt(InputPrompts);
        }

        protected override bool CancelSelection()
        {
            return Input.GetMouseButtonDown(1);
        }

        protected override bool ConfirmSelection()
        {
            return Input.GetKeyDown(combatant.flowKey);
        }
    }
}

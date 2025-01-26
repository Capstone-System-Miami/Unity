using System.Collections.Generic;
using SystemMiami.CombatSystem;
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

        protected override bool CancelSelection()
        {
            return Input.GetMouseButtonDown(1);
        }

        protected override bool ConfirmSelection()
        {
            return Input.GetKeyDown(KeyCode.Return);
        }

        protected override void GoToMovementExecution()
        {
            machine.SetState(new PlayerMovementExecution(combatant, path));
        }

        protected override void GoToMovementTileSelection()
        {
            machine.SetState(new PlayerMovementTileSelection(combatant));
        }
    }
}

using System.Collections.Generic;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class PlayerMovementTileConfirmation : MovementTileConfirmation
    {
        public PlayerMovementTileConfirmation(
            Combatant combatant,
            MovementPath limitedPath)
                : base(
                    combatant,
                    limitedPath
                )
        { }

        public override void cMakeDecision()
        {
            if (CancelSelection())
            {
                machine.SetState(new PlayerMovementTileSelection(combatant));
                return;
            }

            if (ConfirmSelection())
            {
                machine.SetState(new PlayerMovementExecution(combatant));
                return;
            }
        }

        protected override bool CancelSelection()
        {
            return Input.GetMouseButtonDown(1);
        }

        protected override bool ConfirmSelection()
        {
            return Input.GetKeyDown(KeyCode.Return);
        }
    }
}

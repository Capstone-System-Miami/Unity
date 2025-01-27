using System.Linq;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class PlayerMovementTileSelection : MovementTileSelection
    {
        public PlayerMovementTileSelection(Combatant combatant)
                : base(combatant) { }

        // Decision
        protected override bool TurnEndRequested()
        {
            return Input.GetKeyDown(KeyCode.Q);
        }

        protected override bool SkipMovementRequested()
        {
            return Input.GetKeyDown(KeyCode.Tab);
        }
        protected override bool ConfirmPathRequested()
        {

            return Input.GetMouseButtonDown(0);
        }
    }
}

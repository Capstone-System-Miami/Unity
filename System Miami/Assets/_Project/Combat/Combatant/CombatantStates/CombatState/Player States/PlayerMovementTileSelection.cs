using System.Linq;
using SystemMiami.CombatSystem;
using SystemMiami.Management;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class PlayerMovementTileSelection : MovementTileSelection
    {
        public PlayerMovementTileSelection(Combatant combatant)
                : base(combatant) { }

        public override void OnEnter()
        {
            base.OnEnter();
            InputPrompts =
                $"Hover over a tile to preview movement.\n\n" +
                $"Click to lock in your path,\n" +
                $"(You will still be able to change your mind)\n\n" +
                $"Or press Tab to skip Movement and select an Action\n";

            UI.MGR.UpdateInputPrompt(InputPrompts);
        }

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

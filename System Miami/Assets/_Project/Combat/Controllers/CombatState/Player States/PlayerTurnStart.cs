using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class PlayerTurnStart : TurnStart
    {
        public PlayerTurnStart(Combatant combatant)
            : base(combatant) { }

        protected override bool Proceed()
        {
            return Input.GetKeyDown(KeyCode.Return);
        }

        public override void GoToMovementTileSelect()
        {
            machine.SetState(new PlayerMovementTileSelection(combatant));
        }
    }
}

using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public class PlayerTurnStart : TurnStart
    {
        public PlayerTurnStart(Combatant combatant)
            : base(combatant) { }

        public override void MakeDecision()
        {
            GoToMovementTileSelect();
        }

        public override void GoToMovementTileSelect()
        {
            machine.SetState(new PlayerMovementTileSelection(combatant));
        }
    }
}

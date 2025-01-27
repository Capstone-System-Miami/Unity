using SystemMiami.CombatSystem;
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
            UI_Prompts =
                $"{combatant.name} Turn Start!\n" +
                $"Press {KeyCode.Return} to begin.";
        }

        protected override bool ProceedRequested()
        {
            return Input.GetKeyDown(KeyCode.Return);
        }
    }
}

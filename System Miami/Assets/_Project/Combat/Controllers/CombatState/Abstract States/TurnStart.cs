using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class TurnStart : CombatantState
    {
        protected TurnStart(Combatant combatant)
            : base(combatant, Phase.None) { }

        public override void OnEnter()
        {
            Debug.Log($"{combatant.name} starting turn");
            combatant.IsMyTurn = true;         
        }

        public override void MakeDecision()
        {
            GoToMovementTileSelect();
        }

        public abstract void GoToMovementTileSelect();        
    }
}

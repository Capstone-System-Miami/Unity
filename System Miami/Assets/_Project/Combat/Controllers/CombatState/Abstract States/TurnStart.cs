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
            base.OnEnter();
            Debug.Log($"{combatant.name} is my name");
            combatant.IsMyTurn = true;         
        }

        public override void MakeDecision()
        {
            if (Proceed())
            {
                GoToMovementTileSelect();
            }
        }

        protected abstract bool Proceed();

        public abstract void GoToMovementTileSelect();        
    }
}

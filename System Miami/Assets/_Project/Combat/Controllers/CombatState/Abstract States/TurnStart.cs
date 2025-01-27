using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class TurnStart : CombatantState
    {
        Conditions movementTileSelectionConditions = new();
        Conditions actionSelectionConditions = new();

        protected TurnStart(Combatant combatant)
            : base(combatant, Phase.None) { }

        public override void OnEnter()
        {
            base.OnEnter();
            combatant.IsMyTurn = true;
            combatant.ResetTurn();
        }

        public override void MakeDecision()
        {
            if (ProceedRequested())
            {
                if (movementTileSelectionConditions.Met())
                {
                    SwitchState(factory.MovementTileSelection());
                    return;
                }
                
                if (actionSelectionConditions.Met())
                {
                    SwitchState(factory.ActionSelection());
                    return;
                }

                SwitchState(factory.TurnEnd());
            }
        }

        protected abstract bool ProceedRequested();
    }
}

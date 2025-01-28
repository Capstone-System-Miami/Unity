using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class TurnStart : CombatantState
    {
        protected Conditions movementTileSelectionConditions = new();
        protected Conditions actionSelectionConditions = new();

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

            }
        }

        protected abstract bool ProceedRequested();

        protected void HandleProceedRequest()
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
}

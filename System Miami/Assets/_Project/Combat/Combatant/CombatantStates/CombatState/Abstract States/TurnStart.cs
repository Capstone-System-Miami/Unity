using SystemMiami.AbilitySystem;
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
            ResetTurn();
        }

        public override void MakeDecision()
        {
            if (ProceedRequested())
            {
                HandleProceedRequest();
            }
        }

        protected abstract bool ProceedRequested();

        protected void HandleProceedRequest()
        {
            if (movementTileSelectionConditions.AllMet())
            {
                SwitchState(factory.MovementTileSelection());
                return;
            }

            if (actionSelectionConditions.AllMet())
            {
                SwitchState(factory.ActionSelection());
                return;
            }

            SwitchState(factory.TurnEnd());
        }

        public void ResetTurn()
        {
            combatant.Loadout.ReduceCooldowns();
            combatant.Stats.DecrementStatusEffectDurations();
            combatant.Speed = new Resource(combatant.Stats.GetStat(StatType.SPEED));
        }
    }
}

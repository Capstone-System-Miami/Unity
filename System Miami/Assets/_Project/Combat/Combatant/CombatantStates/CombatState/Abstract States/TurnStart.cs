using System.Collections.Generic;
using System.Linq;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;
using UnityEngine.Assertions;

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
            Debug.Log($"{combatant.name}: ResetTurn called.");
           // Assert.IsNotNull(combatant.Loadout, $"{combatant.name}'s Loadout was null");
            combatant.Loadout.ReduceCooldowns();
            combatant.Stats.DecrementStatusEffectDurations();
            ApplyResourceEffects();
            combatant.Speed = new Resource(combatant.Stats.GetStat(StatType.SPEED));
        }
        
        public void ApplyResourceEffects()
        {
            if(combatant.resourceEffects == null || combatant.resourceEffects.Count == 0)
            {
                return;
            }
            combatant.resourceEffects = combatant.resourceEffects.Where(effect => effect.RemainingTurns > 0).ToList();
            combatant.resourceEffects.ForEach(effect =>
            {
               
                effect.Execute();
            });
        }

    }
}

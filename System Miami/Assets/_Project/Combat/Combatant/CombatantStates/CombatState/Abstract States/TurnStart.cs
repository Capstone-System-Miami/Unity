using System.Collections.Generic;
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
            GainResource();
            combatant.Stats.DecrementStatusEffectDurations();
            DecrementRestoreResourceDurations();
            combatant.Speed = new Resource(combatant.Stats.GetStat(StatType.SPEED));
        }
        private void GainResource()
        {
            if(combatant.hasResourceEffect)
            {
                if (combatant.restoreResourceEffects.ContainsKey(ResourceType.Health))
                {
                    combatant.GainResource(ResourceType.Health,combatant._endOfTurnHeal);
                    combatant.GainResource(ResourceType.Health,combatant._endOfTurnDamage);
                }
                else if(combatant.restoreResourceEffects.ContainsKey(ResourceType.Mana))
                {
                    combatant.GainResource(ResourceType.Mana, combatant._endOfTurnMana);
                }
                else if (combatant.restoreResourceEffects.ContainsKey(ResourceType.Stamina))
                {
                    combatant.GainResource(ResourceType.Stamina, combatant._endOfTurnStamina);
                }
                
            }
        }
        public void DecrementRestoreResourceDurations()
        {
            if (combatant.restoreResourceEffects.Count > 0 && combatant != null)
            {
                List<ResourceType> keysToRemove = new List<ResourceType>();

                // First pass: decide which keys need changing / removing
                foreach (var kvp in combatant.restoreResourceEffects)
                {
                    if (kvp.Value > 0)
                    {
                        combatant.restoreResourceEffects[kvp.Key] = kvp.Value - 1;
                    }
                    if (kvp.Value <= 0)
                    {
                        keysToRemove.Add(kvp.Key);
                    }
                }

                // Second pass: remove them outside the iteration
                foreach (var key in keysToRemove)
                {
                    combatant.restoreResourceEffects.Remove(key);
                }
            }
           
        }
    }
}

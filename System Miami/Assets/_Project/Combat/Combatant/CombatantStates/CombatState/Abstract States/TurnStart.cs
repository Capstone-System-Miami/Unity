using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;
using UnityEngine.Assertions;

namespace SystemMiami.CombatRefactor
{
    public abstract class TurnStart : CombatantState
    {
        private readonly object resourceEffectLock = new object();
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
           // Debug.Log($"{combatant.name}: GainResource called.");
            if(combatant.hasResourceEffect)
            {
               // Debug.Log($"{combatant.name}: GainResource called because it has a resource effect.");
                if (combatant.restoreResourceEffects.ContainsKey(ResourceType.Health))
                {
                    //Debug.Log($"Combatant has Health restore effect. restoring {combatant._endOfTurnHeal} resources.");
                   // Debug.Log($"Combatant has Health damage effect. restoring {combatant._endOfTurnDamage} resources.");
                    combatant.GainResource(ResourceType.Health,combatant._endOfTurnHeal);
                    combatant.GainResource(ResourceType.Health,combatant._endOfTurnDamage);
                }
                if(combatant.restoreResourceEffects.ContainsKey(ResourceType.Mana))
                {
                    combatant.GainResource(ResourceType.Mana, combatant._endOfTurnMana);
                }
                if (combatant.restoreResourceEffects.ContainsKey(ResourceType.Stamina))
                {
                    combatant.GainResource(ResourceType.Stamina, combatant._endOfTurnStamina);
                }
                
            }
        }
       public void DecrementRestoreResourceDurations()
       {
           lock (resourceEffectLock)
           {
             //  Debug.Log($"{combatant.name}: DecrementRestoreResourceDurations called.");
               if (combatant.restoreResourceEffects.Count > 0 && combatant != null)
               {
                   // copy of the dictionary so no enum modification error
                   var restoreResourceEffectsCopy = new Dictionary<ResourceType, int>(combatant.restoreResourceEffects);
                   List<ResourceType> keysToRemove = new List<ResourceType>();

                   foreach (var kvp in restoreResourceEffectsCopy)
                   {
                      // Debug.Log($"{combatant.name}: Decrementing {kvp.Key}, current value: {kvp.Value}");
                       if (kvp.Value > 1) // Decrement only if duration is greater than 1
                       {
                           combatant.restoreResourceEffects[kvp.Key] = kvp.Value - 1;
                       }
                       else if (kvp.Value == 1) // Mark for removal when duration reaches 1
                       {
                           keysToRemove.Add(kvp.Key);
                       }
                   }

                 
                   foreach (var key in keysToRemove)
                   {
                      // Debug.Log($"{combatant.name}: Removing expired effect for {key}");
                       combatant.restoreResourceEffects.Remove(key);
                       
                       switch (key)
                       {
                           case ResourceType.Health:
                               combatant._endOfTurnHeal = 0;
                               combatant._endOfTurnDamage = 0;
                               break;
                           case ResourceType.Stamina:
                               combatant._endOfTurnStamina = 0;
                               break;
                           case ResourceType.Mana:
                               combatant._endOfTurnMana = 0;
                               break;
                       }
                   }
               }
               else if (combatant.restoreResourceEffects.Count == 0)
               {
                   //Debug.Log($"{combatant.name}: No restore resource effects remaining.");
                   combatant.hasResourceEffect = false;
               }
           }

       }

    }
}

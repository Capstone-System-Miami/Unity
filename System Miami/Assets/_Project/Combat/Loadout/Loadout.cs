using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.InventorySystem;
using UnityEngine;
using UnityEngine.Assertions;

namespace SystemMiami.CombatRefactor
{
    public class Loadout
    {
        public List<AbilityPhysical> PhysicalAbilities { get; private set; } = new();
        public List<AbilityMagical>  MagicalAbilities  { get; private set; } = new();
        public List<Consumable>      Consumables       { get; private set; } = new();

        private Combatant user;

      
        public Loadout(Inventory inventory, Combatant user)
        {
            this.user = user;
            if (inventory == null)
            {
                Debug.LogError($"Loadout: Inventory is null. Returning empty loadout.");
                return;
            }
            PhysicalAbilities = ConvertPhysical(inventory.QuickslotPhysicalAbilityIDs);
            MagicalAbilities  = ConvertMagical(inventory.QuickslotMagicalAbilityIDs);
            Consumables       = ConvertConsumable(inventory.QuickslotConsumableIDs);
        }
        
        public Loadout(List<int> abilities, Combatant user)
        {
            this.user = user;

            foreach (int abilityID in abilities)
            {
                if (Database.MGR.GetDataType(abilityID) == ItemType.PhysicalAbility)
                {
                    AbilityPhysical ability = Database.MGR.CreateInstance(abilityID, user) as AbilityPhysical;
                    PhysicalAbilities.Add(ability);
                }
                else if (Database.MGR.GetDataType(abilityID) == ItemType.MagicalAbility)
                {
                    AbilityMagical ability = Database.MGR.CreateInstance(abilityID, user) as AbilityMagical;
                    MagicalAbilities.Add(ability);
                }
                else if (Database.MGR.GetDataType(abilityID) == ItemType.Consumable)
                {
                    Consumable ability = Database.MGR.CreateInstance(abilityID, user) as Consumable;
                    Consumables.Add(ability);
                }
            }
           
        }

        
        private List<AbilityPhysical> ConvertPhysical(List<int> physicalIDs)
        {
            List<AbilityPhysical> result = new();

            if (physicalIDs == null)
            {
                Debug.LogWarning($"Loadout: PhysicalIDs list is null. Returning empty list.");
                return result;
            }

            foreach (int id in physicalIDs)
            {
                Debug.Log("Database.Instance is: " + (Database.MGR == null ? "NULL" : "NOT NULL"));
                CombatAction action = Database.MGR.CreateInstance(id, user);
                if (action is AbilityPhysical physical)
                {
                    result.Add(physical);
                }
                else
                {
                    Debug.LogWarning($"Loadout: ID {id} is not a valid PhysicalAbility.");
                }
            }

            return result;
        }

        
        private List<AbilityMagical> ConvertMagical(List<int> magicalIDs)
        {
            List<AbilityMagical> result = new();

            if (magicalIDs == null)
            {
                Debug.LogWarning($"Loadout: MagicalIDs list is null. Returning empty list.");
                return result;
            }

            foreach (int id in magicalIDs)
            {
                CombatAction action = Database.MGR.CreateInstance(id, user);
                if (action is AbilityMagical magical)
                {
                    result.Add(magical);
                }
                else
                {
                    Debug.LogWarning($"Loadout: ID {id} is not a valid MagicalAbility.");
                }
            }

            return result;
        }

        
        private List<Consumable> ConvertConsumable(List<int> consumableIDs)
        {
            List<Consumable> result = new();

            if (consumableIDs == null)
            {
                Debug.LogWarning($"Loadout: ConsumableIDs list is null. Returning empty list.");
                return result;
            }

            foreach (int id in consumableIDs)
            {
                CombatAction action = Database.MGR.CreateInstance(id, user);
                Assert.IsNotNull(action);
                if (action is Consumable consumable)
                {
                    result.Add(consumable);
                    // Hook into the consumed event so we can remove it from this list when used up
                    consumable.Consumed += HandleConsume;
                }
                else
                {
                    Debug.LogWarning($"Loadout: ID {id} is not a valid Consumable.");
                }
            }

            return result;
        }

        
        public void ReduceCooldowns()
        {
            PhysicalAbilities.ForEach(a => a.ReduceCooldown());
            MagicalAbilities.ForEach(a => a.ReduceCooldown());            
        }

       
        private void HandleConsume(Consumable consumable)
        {
            if (Consumables.Contains(consumable))
            {
                // Remove from the loadout so we can't use it again
                Consumables.Remove(consumable);
                consumable.Consumed -= HandleConsume;
            }
        }

        // =========================================
        //   Helper: Combine all actions if needed
        // =========================================
        public List<CombatAction> GetAllActions()
        {
            List<CombatAction> allActions = new();
            allActions.AddRange(PhysicalAbilities);
            allActions.AddRange(MagicalAbilities);
            allActions.AddRange(Consumables);
            return allActions;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using SystemMiami.CombatRefactor;
using SystemMiami.CombatSystem;

namespace SystemMiami.Management
{
    public class Inventory
    {
        // Tracks what the player OWNS
        public List<int> OwnedAbilityIDs { get; private set; } = new();
        public List<int> OwnedConsumableIDs { get; private set; } = new();
        
        private DatabaseSO database;
        private Combatant user;

        // The actively equipped items
        public Loadout PlayerLoadout { get; private set; }

        /// <summary>
        /// Constructs the Inventory and an empty Loadout.
        /// </summary>
        public Inventory(DatabaseSO database, Combatant user)
        {
            this.database = database;
            this.user = user;

            // Start with empty ID lists for loadout
            PlayerLoadout = new Loadout(database, user, new List<int>(), new List<int>());
        }

        /// <summary>
        /// Add an ability to the inventory, then automatically try to equip it.
        /// </summary>
        public void AddAbility(int abilityID)
        {
            if (!OwnedAbilityIDs.Contains(abilityID))
                OwnedAbilityIDs.Add(abilityID);

            EquipAbility(abilityID);
        }

        /// <summary>
        /// Add a consumable to the inventory, then automatically try to equip it.
        /// </summary>
        public void AddConsumable(int consumableID)
        {
            if (!OwnedConsumableIDs.Contains(consumableID))
                OwnedConsumableIDs.Add(consumableID);

            EquipConsumable(consumableID);
        }

        /// <summary>
        /// Equip an owned ability ID into the Loadout.
        /// </summary>
        public void EquipAbility(int abilityID)
        {
            CombatAction instance = database.CreateInstance(abilityID, user);
            if (instance is AbilityPhysical physicalAbility 
                && !PlayerLoadout.PhysicalAbilities.Contains(physicalAbility))
            {
                PlayerLoadout.PhysicalAbilities.Add(physicalAbility);
            }
            else if (instance is AbilityMagical magicalAbility 
                && !PlayerLoadout.MagicalAbilities.Contains(magicalAbility))
            {
                PlayerLoadout.MagicalAbilities.Add(magicalAbility);
            }
        }

        /// <summary>
        /// Equip an owned consumable ID into the Loadout.
        /// </summary>
        public void EquipConsumable(int consumableID)
        {
            CombatAction instance = database.CreateInstance(consumableID, user);
            if (instance is Consumable consumable 
                && !PlayerLoadout.Consumables.Contains(consumable))
            {
                PlayerLoadout.Consumables.Add(consumable);
            }
        }

        

        // TODO Equipmods 
    }
}

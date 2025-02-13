using System.Collections.Generic;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class Loadout
    {
        public List<AbilityPhysical> PhysicalAbilities { get; private set; } = new();
        public List<AbilityMagical> MagicalAbilities { get; private set; } = new();
        public List<Consumable> Consumables { get; private set; } = new();
        public List<CombatAction> AllCombatActions { get; private set; } = new();

        private Combatant user;
        private DatabaseSO database;

        #region Construction
        public Loadout(DatabaseSO database, Combatant user, List<int> abilityIDs, List<int> consumableIDs)
        {
            this.database = database;
            this.user = user;

            PhysicalAbilities = LoadPhysicalAbilities(abilityIDs);
            MagicalAbilities = LoadMagicalAbilities(abilityIDs);
            Consumables = LoadConsumables(consumableIDs);

            // Subscribe consumables to consumption event
            Consumables.ForEach(consumable => consumable.Consumed += HandleConsume);
        }

        private List<AbilityPhysical> LoadPhysicalAbilities(List<int> abilityIDs)
        {
            List<AbilityPhysical> result = new();
            foreach (int id in abilityIDs)
            {
                CombatAction instance = database.CreateInstance(id, user);
                if (instance is AbilityPhysical physicalAbility)
                {
                    result.Add(physicalAbility);
                }
            }
            return result;
        }

        private List<AbilityMagical> LoadMagicalAbilities(List<int> abilityIDs)
        {
            List<AbilityMagical> result = new();
            foreach (int id in abilityIDs)
            {
                CombatAction instance = database.CreateInstance(id, user);
                if (instance is AbilityMagical magicalAbility)
                {
                    result.Add(magicalAbility);
                }
            }
            return result;
        }

        private List<Consumable> LoadConsumables(List<int> consumableIDs)
        {
            List<Consumable> result = new();
            foreach (int id in consumableIDs)
            {
                CombatAction instance = database.CreateInstance(id, user);
                if (instance is Consumable consumable)
                {
                    result.Add(consumable);
                }
            }
            return result;
        }
        #endregion Construction

        public void ReduceCooldowns()
        {
            PhysicalAbilities.ForEach(ability => ability.ReduceCooldown());
            MagicalAbilities.ForEach(ability => ability.ReduceCooldown());
        }

        private void HandleConsume(Consumable consumable)
        {
            if (!Consumables.Contains(consumable)) return;
            Consumables.Remove(consumable);
            consumable.Consumed -= HandleConsume;
        }
    }
}

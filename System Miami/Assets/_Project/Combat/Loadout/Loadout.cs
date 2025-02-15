using System.Collections.Generic;
using UnityEngine;
using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public class Loadout
    {
        public List<AbilityPhysical> PhysicalAbilities { get; private set; } = new();
        public List<AbilityMagical> MagicalAbilities { get; private set; } = new();
        public List<Consumable> Consumables { get; private set; } = new();

        private Combatant user;

        public Loadout(
            List<int> physicalIDs,
            List<int> magicalIDs,
            List<int> consumableIDs,
            Combatant user)
        {
            this.user = user;

            PhysicalAbilities = ConvertPhysical(physicalIDs);
            MagicalAbilities  = ConvertMagical(magicalIDs);
            Consumables       = ConvertConsumable(consumableIDs);

           
            Consumables.ForEach(c => c.Consumed += HandleConsume);
        }

        private List<AbilityPhysical> ConvertPhysical(List<int> ids)
        {
            var result = new List<AbilityPhysical>();
            if (ids == null) return result;

            foreach (int id in ids)
            {
                var action = Database.Instance.CreateInstance(id, user);
                if (action is AbilityPhysical physAbility)
                {
                    result.Add(physAbility);
                }
                else
                {
                    Debug.LogWarning($"ID {id} is not a Physical Ability!");
                }
            }
            return result;
        }

        private List<AbilityMagical> ConvertMagical(List<int> ids)
        {
            var result = new List<AbilityMagical>();
            if (ids == null) return result;

            foreach (int id in ids)
            {
                var action = Database.Instance.CreateInstance(id, user);
                if (action is AbilityMagical magAbility)
                {
                    result.Add(magAbility);
                }
                else
                {
                    Debug.LogWarning($"ID {id} is not a Magical Ability!");
                }
            }
            return result;
        }

        private List<Consumable> ConvertConsumable(List<int> ids)
        {
            var result = new List<Consumable>();
            if (ids == null) return result;

            foreach (int id in ids)
            {
                var action = Database.Instance.CreateInstance(id, user);
                if (action is Consumable consumableAction)
                {
                    result.Add(consumableAction);
                }
                else
                {
                    Debug.LogWarning($"ID {id} is not a Consumable!");
                }
            }
            return result;
        }

        private void HandleConsume(Consumable consumable)
        {
            if (Consumables.Contains(consumable))
            {
                Consumables.Remove(consumable);
                consumable.Consumed -= HandleConsume;
            }
        }

        public void ReduceCooldowns()
        {
            PhysicalAbilities.ForEach(ability => ability.ReduceCooldown());
            MagicalAbilities.ForEach(ability => ability.ReduceCooldown());
        }
    }
}

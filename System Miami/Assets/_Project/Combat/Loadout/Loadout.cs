using System.Collections.Generic;
using System.Linq;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class Loadout
    {
        public List<AbilityPhysical> PhysicalAbilities
            { get; private set; } = new();
        public List<AbilityMagical> MagicalAbilities
            { get; private set; } = new();
        public List<Consumable> Consumables
            { get; private set; } = new();

        private Combatant user;

        public Loadout(
            List<NewAbilitySO> physicalPresets,
            List<NewAbilitySO> magicalPresets,
            List<ConsumableSO> consumablePresets,
            Combatant user)
        {
            this.user = user;

            if (!TryGetInstances(
                    physicalPresets,
                    out List<AbilityPhysical> physicalInstances))
            {
                return;
            }
            PhysicalAbilities = physicalInstances;

            if (!TryGetInstances(
                    magicalPresets,
                    out List<AbilityMagical> magicalInstances))
            {
                return;
            }
            MagicalAbilities = magicalInstances;

            if (!TryGetInstances(
                    consumablePresets,
                    out List<Consumable> consumableInstances))
            {
                return;
            }
            Consumables = consumableInstances;

            Consumables.ForEach(consumable
                => consumable.Consumed += HandleConsume);
        }

        public void ReduceCooldowns()
        {
            PhysicalAbilities.ForEach(ability => ability.ReduceCooldown());
            MagicalAbilities.ForEach(ability => ability.ReduceCooldown());
        }

        protected void HandleConsume(Consumable consumable)
        {
            if (!Consumables.Contains(consumable)) { return; }

            Consumables.Remove(consumable);
            consumable.Consumed -= HandleConsume;
        }


        private bool TryGetInstances(
            List<NewAbilitySO> presets,
            out List<AbilityPhysical> instances)
        {
            instances = new();

            if (presets == null)
            {
                Debug.LogWarning(
                    $"The preset list in {this} was null" +
                    $"when assigning Physical Abilities.");
                return false;
            }

            foreach (NewAbilitySO preset in presets)
            {
                if (preset.AbilityType != AbilitySystem.AbilityType.PHYSICAL)
                {
                    instances = new();
                    Debug.LogWarning(
                        $"An error occurred in {this} " +
                        $"when assigning Physical Abilities.");
                    return false;
                }

                instances.Add(new AbilityPhysical(preset, user));
            }

            return true;
        }

        private bool TryGetInstances(
            List<NewAbilitySO> presets,
            out List<AbilityMagical> instances)
        {
            instances = new();

            if (presets == null)
            {
                Debug.LogWarning(
                    $"The preset list in {this} was null" +
                    $"when assigning Magical Abilities.");
            }

            foreach (NewAbilitySO preset in presets)
            {
                if (preset.AbilityType != AbilitySystem.AbilityType.MAGICAL)
                {
                    instances = new();
                    Debug.LogWarning(
                        $"An error occurred in {this} " +
                        $"when assigning Magical Abilities.");
                    return false;
                }

                instances.Add(new AbilityMagical(preset, user));
            }

            return true;
        }

        private bool TryGetInstances(
            List<ConsumableSO> presets,
            out List<Consumable> instances)
        {
            instances = new();            

            if (presets == null)
            {
                Debug.LogWarning(
                    $"The preset list in {this} was null " +
                    $"when assigning Consumable Actions.");
            }

            foreach (ConsumableSO preset in presets)
            {
                instances.Add(new Consumable(preset, user));
            }

            return true;
        }
    }
}

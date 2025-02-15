using System.Collections.Generic;
using System.Linq;
using SystemMiami.CombatSystem;
using SystemMiami.Management;
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

        public List<CombatAction> AllCombatActions
        {get; private set; } = new();

        private Combatant user;

        #region Construction
        // ========================================================
        public Loadout(
            List<NewAbilitySO> physicalPresets,
            List<NewAbilitySO> magicalPresets,
            List<ConsumableSO> consumablePresets,
            Combatant user)
        {
            this.user = user;

            PhysicalAbilities = ConvertPhysical(physicalPresets);

            MagicalAbilities = ConvertMagical(magicalPresets);

            Consumables = ConvertConsumable(consumablePresets);

            Consumables.ForEach(consumable
                => consumable.Consumed += HandleConsume);
        }

        private List<AbilityPhysical> ConvertPhysical(List<NewAbilitySO> presets)
        {
            List<AbilityPhysical> result = new();

            if (presets == null)
            {
                Debug.LogWarning(
                    $"The preset list in {this} was null" +
                    $"when assigning Physical Abilities. " +
                    $"Returning an empty list");
                return new();
            }

            foreach (NewAbilitySO preset in presets)
            {
                if (preset.AbilityType != AbilitySystem.AbilityType.PHYSICAL)
                {
                    Debug.LogWarning(
                        $"An error occurred in {this} " +
                        $"when assigning Physical Abilities. " +
                        $"Ensure that all presets are in " +
                        $"The list corresponding to their type. " +
                        $"Returning an empty list.");
                    return new();
                }

                result.Add(new AbilityPhysical(preset, user));
            }

            return result;
        }

        private List<AbilityMagical> ConvertMagical(List<NewAbilitySO> presets)
        {
            List<AbilityMagical> result = new();

            if (presets == null)
            {
                Debug.LogWarning(
                    $"The preset list in {this} was null" +
                    $"when assigning Magical Abilities. " +
                    $"Returning an empty list.");
                return new();
            }

            foreach (NewAbilitySO preset in presets)
            {
                if (preset.AbilityType != AbilitySystem.AbilityType.MAGICAL)
                {
                    Debug.LogWarning(
                        $"An error occurred in {this} " +
                        $"when assigning Magical Abilities. " +
                        $"Ensure that all presets are in " +
                        $"The list corresponding to their type. " +
                        $"Returning an empty list.");
                    return new();
                }

                result.Add(new AbilityMagical(preset, user));
            }

            return result;
        }

        private List<Consumable> ConvertConsumable(List<ConsumableSO> presets)
        {
            List<Consumable> result = new();

            if (presets == null)
            {
                Debug.LogWarning(
                    $"The preset list in {this} was null " +
                    $"when assigning Consumable Actions. " +
                    $"Returning an empty list");
                return result;
            }

            foreach (ConsumableSO preset in presets)
            {
                result.Add(new Consumable(preset, user));
            }

            return result;
        }
        #endregion Construction

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

    }
}

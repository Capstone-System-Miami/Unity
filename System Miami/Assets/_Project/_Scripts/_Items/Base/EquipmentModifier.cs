// Authors: Lee
using UnityEngine;
using SystemMiami.CombatSystem;

namespace SystemMiami

{
    /// <summary>
    /// Represents a modifier that can be applied to equipment to enhance its stats or add effects.
    /// </summary>
    [CreateAssetMenu(fileName = "New Equipment Modifier", menuName = "Inventory/Equipment Modifier")]
    public class EquipmentModifier : Item
    {
        [Header("Modifier Properties")] public string modifierName;
        public Sprite icon;
        [TextArea] public string description;

        [Header("Modifier Attributes")]
        public EquipmentType targetEquipmentType; // The type of equipment this modifier applies to

        public StatSetSO statModifiers;
        public StatusEffect statusEffect;
        public int effectDuration; // Duration of the status effect

        /// <summary>
        /// Applies the modifier to the equipment.
        /// </summary>
        /// <param name="equipment">The equipment item to modify.</param>
        //public void ApplyModifier(Equipment equipment)
        //{
        //    // Add stat modifiers to the equipment
        //    if (statModifiers != null)
        //    {
        //        equipment.AddStatModifiers(statModifiers);
        //    }

        //    // Add status effect to the equipment
        //    if (statusEffect != null)
        //    {
        //        equipment.AddStatusEffect(statusEffect, effectDuration);
        //    }

        //    Debug.Log($"{modifierName} applied to {equipment.itemName}");
        //}

        /// <summary>
        /// Removes the modifier from the equipment.
        /// </summary>
        /// <param name="equipment">The equipment item to remove the modifier from.</param>
        //public void RemoveModifier(Equipment equipment)
        //{
        //    // Remove stat modifiers from the equipment
        //    if (statModifiers != null)
        //    {
        //        equipment.RemoveStatModifiers(statModifiers);

        //        // Remove status effect from the equipment
        //        if (statusEffect != null)
        //        {
        //            equipment.RemoveStatusEffect(statusEffect);
        //        }

        //        Debug.Log($"{modifierName} removed from {equipment.itemName}");
        //    }
        //}
    }
}

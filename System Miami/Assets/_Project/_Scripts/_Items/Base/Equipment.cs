//// Authors: Lee
//using UnityEngine;
//using System.Collections.Generic;

//namespace SystemMiami
//{
//    /// <summary>
//    /// Represents equippable items like weapons and armor.
//    /// </summary>
//    [CreateAssetMenu(fileName = "New Equipment Item", menuName = "Inventory/Equipment Item")]
//    public class Equipment : Item
//    {
//        [Header("Equipment Properties")]
//        public EquipmentType equipmentType;      // Type of equipment slot

//        // Base stat modifiers
//        public StatSetSO baseStatModifiers;

//        // Applied modifiers
//        private List<EquipmentModifier> appliedModifiers = new List<EquipmentModifier>();

//        // Combined stat modifiers (base + modifiers)
//        private StatSetSO combinedStatModifiers;

//        // Status effects granted by modifiers
//        private Dictionary<StatusEffect, int> statusEffects = new Dictionary<StatusEffect, int>();

//        public StatSetSO GetCombinedStatModifiers()
//        {
//            if (combinedStatModifiers == null)
//            {
//                RecalculateStatModifiers();
//            }
//            return combinedStatModifiers;
//        }

//        /// <summary>
//        /// Removes stat modifiers from a modifier.
//        /// </summary>
//        /// <param name="statModifiers">The stat modifiers to remove.</param>
//        public void RemoveStatModifiers(StatSetSO statModifiers)
//        {
//            if (combinedStatModifiers == null)
//                return;

//            //foreach (StatType statType in System.Enum.GetValues(typeof(StatType)))
//            //{
//            //    float modifierValue = statModifiers.GetStatValue(statType);
//            //    float currentValue = combinedStatModifiers.GetStatValue(statType);
//            //    combinedStatModifiers.SetStatValue(statType, currentValue - modifierValue);
//            //}
//        }

//        public void AddStatModifiers(StatSetSO statModifiers)
//        {
//            if (combinedStatModifiers == null)
//            {
//                combinedStatModifiers = ScriptableObject.CreateInstance<StatSetSO>();
//                CopyStatSet(baseStatModifiers, combinedStatModifiers);
//            }

//            foreach (StatType statType in System.Enum.GetValues(typeof(StatType)))
//            {
//                //float modifierValue = statModifiers.GetStatValue(statType);
//                //float currentValue = combinedStatModifiers.GetStatValue(statType);
//                //combinedStatModifiers.SetStatValue(statType, currentValue + modifierValue);
//            }
//        }

//        /// <summary>
//        /// Recalculates the combined stat modifiers.
//        /// </summary>
//        private void RecalculateStatModifiers()
//        {
//            combinedStatModifiers = ScriptableObject.CreateInstance<StatSetSO>();
//            // Add base stat modifiers
//            foreach (StatType statType in System.Enum.GetValues(typeof(StatType)))
//            {
//                float baseValue = baseStatModifiers.GetStatValue(statType);
//                combinedStatModifiers.SetStatValue(statType, baseValue);
//            }

//            // Add modifiers
//            foreach (var modifier in appliedModifiers)
//            {
//                //foreach (StatType statType in System.Enum.GetValues(typeof(StatType)))
//                //{
//                //    float modifierValue = modifier.statModifiers.GetStatValue(statType);
//                //    float currentValue = combinedStatModifiers.GetStatValue(statType);
//                //    combinedStatModifiers.SetStatValue(statType, currentValue + modifierValue);
//                //}
//            }
//        }

//        /// <summary>
//        /// Adds a status effect from a modifier.
//        /// </summary>
//        public void AddStatusEffect(StatusEffect effect, int duration)
//        {
//            statusEffects[effect] = duration;
//        }

//        /// <summary>
//        /// Removes a status effect from a modifier.
//        /// </summary>
//        public void RemoveStatusEffect(StatusEffect effect)
//        {
//            if (statusEffects.ContainsKey(effect))
//            {
//                statusEffects.Remove(effect);
//            }
//        }

//        /// <summary>
//        /// Gets the list of status effects granted by modifiers.
//        /// </summary>
//        public Dictionary<StatusEffect, int> GetStatusEffects()
//        {
//            return statusEffects;
//        }

//        /// <summary>
//        /// Utility method to copy stats from one StatSetSO to another.
//        /// </summary>
//        /// <param name="source">The source StatSetSO to copy from.</param>
//        /// <param name="destination">The destination StatSetSO to copy to.</param>
//        private void CopyStatSet(StatSetSO source, StatSetSO destination)
//        {
//            // Ensure neither the source nor destination is null
//            if (source == null || destination == null)
//            {
//                Debug.LogWarning("CopyStatSet: Source or destination StatSetSO is null.");
//                return;
//            }

//            // Iterate through each StatType and copy the values
//            //foreach (StatType statType in System.Enum.GetValues(typeof(StatType)))
//            //{
//            //    float value = source.GetStatValue(statType);
//            //    destination.SetStatValue(statType, value);
//            //}
//        }
//    }
//}

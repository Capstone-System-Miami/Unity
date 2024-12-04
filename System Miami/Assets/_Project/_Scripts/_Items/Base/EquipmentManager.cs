//// Authors: Layla
//using UnityEngine;
//using System.Collections.Generic;
//using SystemMiami.CombatSystem;


//namespace SystemMiami
//{
//    /// <summary>
//    /// Manages the fixed equipment and its modifiers for a combatant.
//    /// </summary>
//    public class EquipmentManager : MonoBehaviour
//    {
//        // Dictionary to hold fixed equipment items based on equipment type
//        [SerializeField]
//        private Dictionary<EquipmentType, Equipment> _fixedEquipment = new Dictionary<EquipmentType, Equipment>();

//        // Reference to the combatant's stats
//        private Combatant _combatant;

//        public Equipment Weapon;//placeholder
//        public Equipment Armor;//placeholder

//        private void Awake()
//        {
//            _combatant = GetComponent<Combatant>();

//            // Initialize the fixed equipment
//            InitializeFixedEquipment();
//        }

//        /// <summary>
//        /// Initializes the fixed equipment for the combatant.
//        /// </summary>
//        private void InitializeFixedEquipment()
//        {
           
//             _fixedEquipment[EquipmentType.Weapon] = Weapon;
//             _fixedEquipment[EquipmentType.Armor] = Armor;

//            // Apply base stat modifiers of the fixed equipment
//            foreach (var equipment in _fixedEquipment.Values)
//            {
//                StatSetSO statModifiers = equipment.GetCombinedStatModifiers();
//                if (statModifiers != null)
//                {
//                    _combatant.Stats.ApplyStatModifiers(statModifiers);
//                }

//            }
//        }

//        /// <summary>
//        /// Applies a modifier to the fixed equipment.
//        /// </summary>
//        /// <param name="type">The type of equipment to modify.</param>
//        /// <param name="modifier">The modifier to apply.</param>
//        public void ApplyModifier(EquipmentType type, EquipmentModifier modifier)
//        {
//            if (_fixedEquipment.ContainsKey(type))
//            {
//                Equipment equipment = _fixedEquipment[type];

//                // Apply modifier to equipment
//                equipment.AddStatModifiers(modifier.statModifiers);
                

//                // Update combatant stats
//                if (modifier.statModifiers != null)
//                {
//                    _combatant.Stats.ApplyStatModifiers(modifier.statModifiers);
//                }

                

//                Debug.Log($"{modifier.modifierName} applied to {equipment.itemName}");
//            }
//            else
//            {
//                Debug.LogWarning($"No equipment of type {type} found to apply the modifier.");
//            }
//        }

//        /// <summary>
//        /// Removes a modifier from the fixed equipment.
//        /// </summary>
//        /// <param name="type">The type of equipment to modify.</param>
//        /// <param name="modifier">The modifier to remove.</param>
//        public void RemoveModifier(EquipmentType type, EquipmentModifier modifier)
//        {
//            if (_fixedEquipment.ContainsKey(type))
//            {
//                Equipment equipment = _fixedEquipment[type];

//                // Remove modifier from equipment
//                equipment.RemoveStatModifiers(modifier.statModifiers);
//                equipment.RemoveStatusEffect(modifier.statusEffect);

//                // Update combatant stats
//                if (modifier.statModifiers != null)
//                {
//                    _combatant.Stats.RemoveStatModifiers(modifier.statModifiers);
//                }

//                // Remove status effect from combatant
//                if (modifier.statusEffect != null)
//                {
//                    _combatant.Stats.RemoveStatusEffect(modifier.statusEffect);
//                }

//                Debug.Log($"{modifier.modifierName} removed from {equipment.itemName}");
//            }
//            else
//            {
//                Debug.LogWarning($"No equipment of type {type} found to remove the modifier.");
//            }
//        }

//        /// <summary>
//        /// Gets the fixed equipment item of a specific type.
//        /// </summary>
//        /// <param name="type">The type of equipment.</param>
//        /// <returns>The equipment item or null.</returns>
//        public Equipment GetEquipment(EquipmentType type)
//        {
//            if (_fixedEquipment.ContainsKey(type))
//            {
//                return _fixedEquipment[type];
//            }
//            return null;
//        }
//    }
//}

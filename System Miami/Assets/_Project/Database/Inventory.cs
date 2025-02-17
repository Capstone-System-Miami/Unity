using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private List<int> magicalAbilityIDs = new();
        [SerializeField] private List<int> physicalAbilityIDs = new();
        [SerializeField] private List<int> consumableIDs = new();
        [SerializeField] private List<int> equipmentModIDs = new(); //TODO

        public List<int> MagicalAbilityIDs { get => magicalAbilityIDs; private set => magicalAbilityIDs = value; }
        public List<int> PhysicalAbilityIDs { get => physicalAbilityIDs; private set => physicalAbilityIDs = value; }
        public List<int> ConsumableIDs { get => consumableIDs; private set => consumableIDs = value; }
        public List<int> EquipmentModIDs { get => equipmentModIDs; private set => equipmentModIDs = value; }
        
        public event System.Action OnInventoryChanged;

        

        

        public void AddAbility(int abilityID)
        {
            if (Database.Instance.GetDataType(abilityID) == ItemType.MagicalAbility)
            {
                magicalAbilityIDs.Add(abilityID);
            }
            else
            {
                Debug.LogError("Wrong ability type! or ID is not an Ability! ensure that the ID starts with 1000");
            }

            if (Database.Instance.GetDataType(abilityID) == ItemType.PhysicalAbility)
            {
                physicalAbilityIDs.Add(abilityID);
            }
            else
            {
                Debug.LogError("Wrong ability type! ID is not an Ability! ensure that the ID starts with 1000");
            }
            
            OnInventoryChanged?.Invoke();
        }

        public void AddConsumable(int consumableID)
        {
            if (Database.Instance.GetDataType(consumableID) == ItemType.Consumable)
            {
                consumableIDs.Add(consumableID);
            }
            else
            {
                Debug.LogWarning($"ID {consumableID} is not a Consumable! ensure that the ID starts with 2000");
            }
        }
    }
}

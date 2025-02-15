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
        

        public IReadOnlyList<int> MagicalAbilityIDs => magicalAbilityIDs;
        public IReadOnlyList<int> PhysicalAbilityIDs => physicalAbilityIDs;
        public IReadOnlyList<int> ConsumableIDs => consumableIDs;
        public IReadOnlyList<int> EquipmentModIDs => equipmentModIDs; //TODO

        public void AddAbility(int abilityID)
        {
            if (Database.Instance.GetDataType(abilityID) == DataType.MagicalAbility)
            {
                magicalAbilityIDs.Add(abilityID);
            }
            else
            {
                Debug.LogError("Wrong ability type! or ID is not an Ability! ensure that the ID starts with 1000");
            }

            if (Database.Instance.GetDataType(abilityID) == DataType.PhysicalAbility)
            {
                physicalAbilityIDs.Add(abilityID);
            }
            else
            {
                Debug.LogError("Wrong ability type! ID is not an Ability! ensure that the ID starts with 1000");
            }
        }

        public void AddConsumable(int consumableID)
        {
            if (Database.Instance.GetDataType(consumableID) == DataType.Consumable)
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SystemMiami.CombatSystem
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private List<int> magicalAbilityIDs = new();
        [SerializeField] private List<int> physicalAbilityIDs = new();
        [SerializeField] private List<int> consumableIDs = new();
        [SerializeField] private List<int> equipmentModIDs = new(); //TODO
        [FormerlySerializedAs("quickslotMagicalAbilities")] [SerializeField] private List<int> quickslotMagicalAbilityIDs = new(); 
        [FormerlySerializedAs("quickslotPhysicalAbilities")] [SerializeField] private List<int> quickslotPhysicalAbilityIDs = new(); 
        [FormerlySerializedAs("quickslotConsumable")] [SerializeField] private List<int> quickslotConsumableIDs = new(); 

        public List<int> MagicalAbilityIDs { get => magicalAbilityIDs; private set => magicalAbilityIDs = value; }
        public List<int> PhysicalAbilityIDs { get => physicalAbilityIDs; private set => physicalAbilityIDs = value; }
        public List<int> ConsumableIDs { get => consumableIDs; private set => consumableIDs = value; }
        public List<int> QuickslotMagicalAbilityIDs { get => quickslotMagicalAbilityIDs; private set => quickslotMagicalAbilityIDs = value; }
        public List<int> QuickslotPhysicalAbilityIDs { get => quickslotPhysicalAbilityIDs; private set => quickslotPhysicalAbilityIDs = value; }
        public List<int> QuickslotConsumableIDs { get => quickslotConsumableIDs; private set => quickslotConsumableIDs = value; }

        
        public event System.Action OnInventoryChanged;

        protected virtual void Awake()
        {
            
        }

        

        public void AddAbility(int abilityID)
        {
            if (Database.MGR.GetDataType(abilityID) == ItemType.MagicalAbility)
            {
                magicalAbilityIDs.Add(abilityID);
            }
            else
            {
                Debug.LogError("Wrong ability type! or ID is not an Ability! ensure that the ID starts with 1000");
            }

            if (Database.MGR.GetDataType(abilityID) == ItemType.PhysicalAbility)
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
            if (Database.MGR.GetDataType(consumableID) == ItemType.Consumable)
            {
                consumableIDs.Add(consumableID);
            }
            else
            {
                Debug.LogWarning($"ID {consumableID} is not a Consumable! ensure that the ID starts with 2000");
            }
        }
        
        public  void MoveToQuickslot(int ID)
        {
            if (Database.MGR.GetDataType(ID) == ItemType.MagicalAbility)
            {
                magicalAbilityIDs.Remove(ID);
                quickslotMagicalAbilityIDs.Add(ID);
            }
            else if (Database.MGR.GetDataType(ID) == ItemType.PhysicalAbility)
            {
                physicalAbilityIDs.Remove(ID);
                quickslotPhysicalAbilityIDs.Add(ID);
            }
            else if (Database.MGR.GetDataType(ID) == ItemType.Consumable)
            {
                consumableIDs.Remove(ID);
                quickslotConsumableIDs.Add(ID);
            }
            else
            {
                Debug.LogError("ID is not valid!");
            }
        }

        public void MoveToInventory(int ID)
        {
            if (Database.MGR.GetDataType(ID) == ItemType.MagicalAbility)
            {
                quickslotMagicalAbilityIDs.Remove(ID);
                magicalAbilityIDs.Add(ID);
            }
            else if (Database.MGR.GetDataType(ID) == ItemType.PhysicalAbility)
            {
                quickslotPhysicalAbilityIDs.Remove(ID);
                physicalAbilityIDs.Add(ID);
            }
            else if (Database.MGR.GetDataType(ID) == ItemType.Consumable)
            {
                quickslotConsumableIDs.Remove(ID);
                consumableIDs.Add(ID);
            }
            else
            {
                Debug.LogError("ID is not valid!");
            }
        
            OnInventoryChanged?.Invoke();
        }
    }
}

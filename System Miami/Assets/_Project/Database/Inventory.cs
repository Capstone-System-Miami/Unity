using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using SystemMiami.CombatSystem;
using SystemMiami.Outdated;

namespace SystemMiami.InventorySystem
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private List<int> physicalAbilityIDs = new();
        [SerializeField] private List<int> magicalAbilityIDs = new();
        [SerializeField] private List<int> consumableIDs = new();
        [SerializeField] private List<int> equipmentModIDs = new(); 
        [SerializeField] private List<int> quickslotPhysicalAbilityIDs = new(); 
        [SerializeField] private List<int> quickslotMagicalAbilityIDs = new(); 
        [SerializeField] private List<int> quickslotConsumableIDs = new();
        [SerializeField] private List<int> equippedEquipmentModIDs = new();

        [SerializeField] private int credits;
        

        public List<int> PhysicalAbilityIDs { get => physicalAbilityIDs; private set => physicalAbilityIDs = value; }
        public List<int> MagicalAbilityIDs { get => magicalAbilityIDs; private set => magicalAbilityIDs = value; }
        public List<int> ConsumableIDs { get => consumableIDs; private set => consumableIDs = value; }
        public List<int> EquipmentModIDs { get => equipmentModIDs; private set => equipmentModIDs = value; }

        public List<int> QuickslotPhysicalAbilityIDs { get => quickslotPhysicalAbilityIDs; private set => quickslotPhysicalAbilityIDs = value; }
        public List<int> QuickslotMagicalAbilityIDs { get => quickslotMagicalAbilityIDs; private set => quickslotMagicalAbilityIDs = value; }
        public List<int> QuickslotConsumableIDs { get => quickslotConsumableIDs; private set => quickslotConsumableIDs = value; }
        public List<int> EquippedEquipmentModIDs { get =>  equippedEquipmentModIDs; private set =>  equippedEquipmentModIDs = value; }

        public List<int> AllValidInventoryItems
        {
            get
            {
                List<int> allIDs = new();
                allIDs.AddRange(PhysicalAbilityIDs);
                allIDs.AddRange(MagicalAbilityIDs);
                allIDs.AddRange(ConsumableIDs);

                return allIDs ?? new();
            }
        }

        public int Credits { get => credits; private set => credits = value; }

        
        public event System.Action OnInventoryChanged;


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G)) { AddCredits(100); } //  Test adding credits
        }

        public void AddToInventory(int ID)
        {
            switch (Database.MGR.GetDataType(ID))
            {
                case ItemType.PhysicalAbility:
                    physicalAbilityIDs.Add(ID);
                    break;

                case ItemType.MagicalAbility:
                    magicalAbilityIDs.Add(ID);
                    break;

                case ItemType.Consumable:
                    consumableIDs.Add(ID);
                    break;

                case ItemType.EquipmentMod:
                    equipmentModIDs.Add(ID);
                    break;

                default:
                    Debug.LogError($"Tried to add an invalid ID: {ID}", this);
                    break;
            }
            
            OnInventoryChanged?.Invoke();
        }
        
        public  void MoveToQuickslot(int ID)
        {
            switch (Database.MGR.GetDataType(ID))
            {
                case ItemType.PhysicalAbility:
                    physicalAbilityIDs.Remove(ID);
                    quickslotPhysicalAbilityIDs.Add(ID);
                    break;

                case ItemType.MagicalAbility:
                    magicalAbilityIDs.Remove(ID);
                    quickslotMagicalAbilityIDs.Add(ID);
                    break;

                case ItemType.Consumable:
                    consumableIDs.Remove(ID);
                    quickslotConsumableIDs.Add(ID);
                    break;
                case ItemType.EquipmentMod:
                    equipmentModIDs.Remove(ID);
                    equippedEquipmentModIDs.Add(ID);
                    break;
                default:
                    Debug.LogError(
                        $"Tried to move an item into a quickslot, but ID ({ID}) " +
                        $"is not valid for use in a Quickslot / Loadout!");
                    break;
            }
        }

        public void MoveToInventory(int ID)
        {
            switch (Database.MGR.GetDataType(ID))
            {
                case ItemType.PhysicalAbility:
                    quickslotPhysicalAbilityIDs.Remove(ID);
                    physicalAbilityIDs.Add(ID);
                    break;

                case ItemType.MagicalAbility:
                    quickslotMagicalAbilityIDs.Remove(ID);
                    magicalAbilityIDs.Add(ID);
                    break;

                case ItemType.Consumable:
                    quickslotConsumableIDs.Remove(ID);
                    consumableIDs.Add(ID);
                    break;
                case ItemType.EquipmentMod:
                    equippedEquipmentModIDs.Remove(ID);
                    equipmentModIDs.Add(ID);
                    break;

                default:
                    Debug.LogError(
                        $"Tried to move an item out of quickslot, but ID ({ID}) " +
                        $"is not valid for use in a Quickslot / Loadout!");
                    break;
            }

            OnInventoryChanged?.Invoke();
        }
        
        

        public void InitializeStartingAbility(CharacterClassType characterClass)
        {
            magicalAbilityIDs.Clear();
            physicalAbilityIDs.Clear();
            consumableIDs.Clear();
            equipmentModIDs.Clear();
            quickslotMagicalAbilityIDs.Clear();
            quickslotPhysicalAbilityIDs.Clear();
            quickslotConsumableIDs.Clear();
            
            
            switch (characterClass)
            {
                case CharacterClassType.MAGE:
                    AddToInventory(2000);
                    break;
                case CharacterClassType.TANK:
                    AddToInventory(1015);
                    break;
                case CharacterClassType.ROGUE:
                    AddToInventory(1019);
                    break;
                case CharacterClassType.FIGHTER:
                    AddToInventory(1012);
                    break;
                    
                    
            }
            
        }
        

        // Gain Credits from quests and other sources
        public void AddCredits(int amount)
        {
            credits += amount;
            Debug.Log($"Gained {amount} Credits! Current Credits: {credits}");
        }

        public void LoseCredits(int amount)
        {
            credits += amount;
            Debug.Log($"Lost {amount} Credits! Current Credits: {credits}");
        }

        public bool CanPurchase(IShopItem item)
        {
            return credits >= item.GetCost();
        }

        public void Purchase(IShopItem item)
        {
            credits -= item.GetCost();
            AddToInventory(item.GetDatabaseID());
        }
    }
}

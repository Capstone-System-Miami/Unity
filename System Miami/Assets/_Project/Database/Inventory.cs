using SystemMiami.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

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

        [field: SerializeField, ReadOnly] public bool subbedToDungeon { get; private set; }

        public List<int> PhysicalAbilityIDs { get => physicalAbilityIDs; private set => physicalAbilityIDs = value; }
        public List<int> MagicalAbilityIDs { get => magicalAbilityIDs; private set => magicalAbilityIDs = value; }
        public List<int> ConsumableIDs { get => consumableIDs; private set => consumableIDs = value; }
        public List<int> EquipmentModIDs { get => equipmentModIDs; private set => equipmentModIDs = value; }

        public List<int> QuickslotPhysicalAbilityIDs { get => quickslotPhysicalAbilityIDs; private set => quickslotPhysicalAbilityIDs = value; }
        public List<int> QuickslotMagicalAbilityIDs { get => quickslotMagicalAbilityIDs; private set => quickslotMagicalAbilityIDs = value; }
        public List<int> QuickslotConsumableIDs { get => quickslotConsumableIDs; private set => quickslotConsumableIDs = value; }
        public List<int> EquippedEquipmentModIDs { get => equippedEquipmentModIDs; private set => equippedEquipmentModIDs = value; }

        public bool TestMode;
        public List<int> AllInventoryItems
        {
            get
            {
                List<int> allIDs = new();
                allIDs.AddRange(PhysicalAbilityIDs);
                allIDs.AddRange(MagicalAbilityIDs);
                allIDs.AddRange(ConsumableIDs);
                return allIDs;
            }
        }

        public List<int> AllQuickslotItems
        {
            get
            {
                List<int> allQuickslotIDs = new();
                allQuickslotIDs.AddRange(quickslotPhysicalAbilityIDs);
                allQuickslotIDs.AddRange(quickslotMagicalAbilityIDs);
                allQuickslotIDs.AddRange(quickslotConsumableIDs);
                allQuickslotIDs.AddRange(equippedEquipmentModIDs);
                return allQuickslotIDs;
            }
        }

        public int Credits { get => credits; private set => credits = value; }

        public event System.Action OnInventoryChanged;

        /// <summary>
        /// NOTE: This doesn't really mean anything right now.
        /// If you want, feel free to check out <see cref="InventoryChangedEventArgs">
        /// and figure out how to integrate it. There's something there, but I can't
        /// get it rn.
        ///
        public event EventHandler<InventoryChangedEventArgs> InventoryChanged;


        #region Unity Methods
        // ====================================================================
        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void Update()
        {
            //  Test adding credits
            if (Debug.isDebugBuild && Input.GetKeyDown(KeyCode.G))
            {
                AddCredits(100);
            }
        }
        #endregion // Unity Methods ===========================================


        #region Event Subscriptions
        // ====================================================================
        private void SetTurnManagerSubscription(bool subscribe)
        {
            if (subscribe)
            {
                subbedToDungeon = true;
                TurnManager.MGR.DungeonFailed += HandleDungeonComplete;
                TurnManager.MGR.DungeonFailed += HandleDungeonFailed;
            }
            else
            {
                subbedToDungeon = false;
                TurnManager.MGR.DungeonFailed -= HandleDungeonComplete;
                TurnManager.MGR.DungeonFailed -= HandleDungeonFailed;
            }
        }
        #endregion // Event Subscriptions =====================================


        #region Event Responses
        // ====================================================================
        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == GAME.MGR.DungeonSceneName && !subbedToDungeon)
            {
                SetTurnManagerSubscription(true);
            }
        }

        private void HandleDungeonComplete()
        {
            SetTurnManagerSubscription(false);

            // TODO: Convert quickslot data back to normal inventory.
        }
        private void HandleDungeonFailed()
        {
            SetTurnManagerSubscription(false);

            // TODO: Convert quickslot data back to normal inventory.
        }
        #endregion // Event Responses ===========================================


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

        public void MoveToQuickslot(int ID)
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
            //equipmentModIDs.Clear();
            quickslotMagicalAbilityIDs.Clear();
            quickslotPhysicalAbilityIDs.Clear();
            quickslotConsumableIDs.Clear();

            if (TestMode)
            { 
               List<int> tempMagIDList = Database.MGR.GetAllItemsOfPlayerClass(ItemType.MagicalAbility).Select(x => x.ID).ToList();
               List<int> tempPhysIDList = Database.MGR.GetAllItemsOfPlayerClass(ItemType.PhysicalAbility).Select(x => x.ID).ToList();

               foreach (int id in tempMagIDList)
               {
                   AddToInventory(id);
               }
               foreach (int id in tempPhysIDList)
               {
                   AddToInventory(id);
               }

               return;
            }

            int startingAbilityID = characterClass switch
            {
                CharacterClassType.MAGE     => 2014,
                CharacterClassType.TANK     => 1015,
                CharacterClassType.ROGUE    => 1032,
                CharacterClassType.FIGHTER  => 1000,
                _                           => 1000,
            };

            int startingResourcePotionID = characterClass switch
            {
                CharacterClassType.MAGE => 3009,
                _                       => 3006
            };

            int startingGeneralHealID = 2013;
            int startingGeneralShieldID = 1045;
            int startingHealPotionID = 3000;

            Debug.Log(
                $"Init starting ability called on {gameObject} with args {characterClass}." +
                $"<color = green>Adding starting Ability " +
                $"{Database.MGR.GetDataWithJustID(startingAbilityID).Name}</color>",
                this);

            Assert.IsFalse(Database.MGR.GetDataWithJustID(startingAbilityID).failbit,
                $"Data Corrupted. Database returned a starting ItemData with a 'true' failbit.");

            AddToInventory(startingAbilityID);
            AddToInventory(startingGeneralHealID);
            AddToInventory(startingGeneralShieldID);
            AddToInventory(startingHealPotionID);
            AddToInventory(startingResourcePotionID);
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

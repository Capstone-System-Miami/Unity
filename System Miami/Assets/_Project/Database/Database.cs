using System.Collections.Generic;
using System.Linq;
using SystemMiami.AbilitySystem;
using SystemMiami.CombatRefactor;
using SystemMiami.CombatSystem;
using SystemMiami.Management;
using SystemMiami.Utilities;
using UnityEngine;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SystemMiami
{

    public class Database : Singleton<Database>
    {
        [Header("Debug Loggers")]
        [SerializeField] private dbug log;

        [SerializeField] private List<ItemData> PhysicalAbilityItemDatas = new List<ItemData>();
        [SerializeField] private List<ItemData> MagicalAbilityItemDatas = new List<ItemData>();
        [SerializeField] private List<ItemData> ConsumableItemDatas = new List<ItemData>();
        [SerializeField] private List<ItemData> EquipmentModItemDatas = new List<ItemData>();
        [SerializeField] private List<ItemData> EnemyPhysicalAbilityItemDatas = new List<ItemData>();
        [SerializeField] private List<ItemData> EnemyMagicalAbilityItemDatas = new List<ItemData>();
        [SerializeField] private List<ItemData> AllItemDatas = new List<ItemData>();


        [SerializeField] private List<NewAbilitySO> physicalAbilityEntries = new();
        [SerializeField] private List<NewAbilitySO> magicalAbilityEntries = new();
        [SerializeField] private List<ConsumableSO> consumableEntries = new();
        [SerializeField] private List<NewAbilitySO> enemyPhysicalAbilityEntries = new();
        [SerializeField] private List<NewAbilitySO> enemyMagicalAbilityEntries = new();
        [SerializeField] private List<EquipmentModSO> equipmentModEntries = new();

        private Dictionary<int, NewAbilitySO> physicalAbilityDatabase;
        private Dictionary<int, NewAbilitySO> magicalAbilityDatabase;
        private Dictionary<int, ConsumableSO> consumableDatabase;
        private Dictionary<int, NewAbilitySO> enemyPhysicalAbilityDatabase;
        private Dictionary<int, NewAbilitySO> enemyMagicalAbilityDatabase;
        private Dictionary<int, EquipmentModSO> equipmentModDatabase;

        [SerializeField] private bool characterSelection;

        private void OnEnable()
        {
            Initialize();
        }

        public void Initialize()
        {
            // Filter database to only include abilities matching the player's class type.
            Attributes playerAttributes = FindObjectOfType<PlayerManager>().GetComponent<Attributes>();
            CharacterClassType playerClassType = playerAttributes._characterClass;

            // Filter physical abilities if not in character selection screen
            //if (!characterSelection)
            //{
            //    physicalAbilityEntries = physicalAbilityEntries
            //        .Where(entry => entry.classType == playerClassType && !entry.isGeneralAbility).ToList();
            //    PhysicalAbilityItemDatas = physicalAbilityEntries.Select(so => so.itemData).ToList();


            //    // Filter magical abilities
            //    magicalAbilityEntries = magicalAbilityEntries
            //        .Where(entry => entry.classType == playerClassType && !entry.isGeneralAbility).ToList();
            //    PhysicalAbilityItemDatas = physicalAbilityEntries.Select(so => so.itemData).ToList();
            //}

            // Convert lists to dictionaries 
            physicalAbilityDatabase = physicalAbilityEntries.ToDictionary(entry => entry.itemData.ID);
            magicalAbilityDatabase = magicalAbilityEntries.ToDictionary(entry => entry.itemData.ID);
            consumableDatabase = consumableEntries.ToDictionary(entry => entry.itemData.ID);
            enemyPhysicalAbilityDatabase = enemyPhysicalAbilityEntries.ToDictionary(entry => entry.itemData.ID);
            enemyMagicalAbilityDatabase = enemyMagicalAbilityEntries.ToDictionary(entry => entry.itemData.ID);
            equipmentModDatabase = equipmentModEntries.ToDictionary(entry => entry.itemData.ID);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                PintAllItemsOfPlayerClassToUI();
            }
        }

        private void PintAllItemsOfPlayerClassToUI()
        {
            List<string> itemsOfPlayerClass =
                GetAllItemsOfPlayerClass(ItemType.PhysicalAbility).Select(phys => phys.Name).ToList();
            itemsOfPlayerClass.AddRange(GetAllItemsOfPlayerClass(ItemType.MagicalAbility).Select(mag => mag.Name).ToList());
            itemsOfPlayerClass.AddRange(GetAllItemsOfPlayerClass(ItemType.Consumable).Select(cons => cons.Name).ToList());

            UI.MGR.StartDialogue(this, true, true, true, "DATABASE CHECK", itemsOfPlayerClass.ToArray());
        }


        public List<ItemData> GetAllItemsOfPlayerClass(ItemType type)
        {
            List<ItemData> result = new();
            switch (type)
            {
                default:
                case ItemType.PhysicalAbility:
                    Assert.IsNotNull(physicalAbilityEntries);
                    Assert.IsTrue(physicalAbilityEntries.Count > 0);
                    result = physicalAbilityEntries.Select(so => so.itemData).ToList();
                    Assert.IsNotNull(result);
                    Assert.IsTrue(result.Count > 0,
                        $"{PlayerManager.MGR.GetComponent<Attributes>()._characterClass} " +
                        $"has no functioning Physical abilities.");
                    break;
                case ItemType.MagicalAbility:
                    result = magicalAbilityEntries.Select(so => so.itemData).ToList();
                    break;
                case ItemType.Consumable:
                    result = consumableEntries.Select(so => so.itemData).ToList();
                    break;
                case ItemType.EquipmentMod:
                    result = equipmentModEntries.Select(so => so.itemData).ToList();
                    break;
            }
            return result;
        }

        public ItemData GetRandomDataOfType(ItemType type)
        {
            List<int> entryIndices = new();
            int randomIndex = 0;

            switch (type)
            {
                default:
                case ItemType.PhysicalAbility:
                    foreach (int id in physicalAbilityDatabase.Keys)
                    {
                        entryIndices.Add(id);
                    }

                    randomIndex = Random.Range(0, entryIndices.Count);
                    return GetDataWithJustID(entryIndices[randomIndex]);

                case ItemType.MagicalAbility:
                    foreach (int id in magicalAbilityDatabase.Keys)
                    {
                        entryIndices.Add(id);
                    }

                    randomIndex = Random.Range(0, entryIndices.Count);
                    return GetDataWithJustID(entryIndices[randomIndex]);

                case ItemType.Consumable:
                    foreach (int id in consumableDatabase.Keys)
                    {
                        entryIndices.Add(id);
                    }

                    randomIndex = Random.Range(0, entryIndices.Count);
                    return GetDataWithJustID(entryIndices[randomIndex]);
                case ItemType.EquipmentMod:
                    foreach (int id in equipmentModDatabase.Keys)
                    {
                        entryIndices.Add(id);
                    }

                    randomIndex = Random.Range(0, entryIndices.Count);
                    return GetDataWithJustID(entryIndices[randomIndex]);
            }


            // Lee old code
            //return type switch
            //{
            //    ItemType.PhysicalAbility => physicalAbilityDatabase.ContainsKey(id) ? physicalAbilityDatabase[id].itemData : default,
            //    ItemType.MagicalAbility => magicalAbilityDatabase.ContainsKey(id) ? magicalAbilityDatabase[id].itemData : default,
            //    ItemType.Consumable => consumableDatabase.ContainsKey(id) ? consumableDatabase[id].itemData : default,
            //    _ => default
            //};
        }

        public ItemData GetDataWithJustID(int id)
        {
            int IDType = id / 1000;
            log.print("Something is trying to get an ID SLOT");
            return IDType switch
            {
                1 => physicalAbilityDatabase.ContainsKey(id) ? physicalAbilityDatabase[id].itemData : default,
                2 => magicalAbilityDatabase.ContainsKey(id) ? magicalAbilityDatabase[id].itemData : default,
                3 => consumableDatabase.ContainsKey(id) ? consumableDatabase[id].itemData : default,
                4 => equipmentModDatabase.ContainsKey(id) ? equipmentModDatabase[id].itemData : default,
                _ => default
            };
        }

        // Factory 
        /// <summary>
        /// Create an instance of a CombatAction (AbilityPhysical, AbilityMagical, or Consumable)
        /// based on the given ID. 
        /// </summary>

        public CombatAction CreateInstance(int id, Combatant user)
        {
            // Check if it's an ability
            if (physicalAbilityDatabase.TryGetValue(id, out NewAbilitySO abilityEntry))
            {
                return new AbilityPhysical(abilityEntry, user);
            }
            if (magicalAbilityDatabase.TryGetValue(id, out abilityEntry))
            {
                return new AbilityMagical(abilityEntry, user);
            }
            // Check if it's a consumable
            if (consumableDatabase.TryGetValue(id, out ConsumableSO consumableEntry))
            {
                return new Consumable(consumableEntry, user);
            }
            if (enemyPhysicalAbilityDatabase.TryGetValue(id, out abilityEntry))
            {
                return new AbilityPhysical(abilityEntry, user);
            }
            if (enemyMagicalAbilityDatabase.TryGetValue(id, out abilityEntry))
            {
                return new AbilityMagical(abilityEntry, user);
            }
            else
            {
                log.warn($"No matching CombatAction found for ID {id} in GameDatabase!");
            }

            return null;
        }

        /// <summary>
        /// Retrieves an EquipmentModSO from the dictionary by ID.
        /// Returns null if not found. 
        /// </summary>
        public EquipmentModSO GetEquipmentMod(int modID)
        {
            //don't love the way I did this but it works for now
            equipmentModDatabase.TryGetValue(modID, out var modSo);
            return modSo;
        }

        /// <summary>
        /// Gets the itemData type of that ID(for sorting purposes)
        /// </summary>
        public ItemType GetDataType(int id)
        {
            if (physicalAbilityDatabase.ContainsKey(id)) return ItemType.PhysicalAbility;
            if (magicalAbilityDatabase.ContainsKey(id)) return ItemType.MagicalAbility;
            if (consumableDatabase.ContainsKey(id)) return ItemType.Consumable;
            if (equipmentModDatabase.ContainsKey(id)) return ItemType.EquipmentMod;
            return ItemType.Consumable; //fallback
        }

        /// <summary>
        /// Retrieve all item IDs from the list that match (minLvl <= playerLvl <= maxLvl).
        /// </summary>
        public List<ItemData> FilterByLevel(List<int> itemIDs)
        {
            int playerLevel = FindObjectOfType<PlayerLevel>().CurrentLevel;
            List<ItemData> filtered = new List<ItemData>();
            foreach (int id in itemIDs)
            {
                ItemData data = GetDataWithJustID(id);
                //data.ID == 0 if not found
                if (data.ID != 0 &&
                    data.MinLevel <= playerLevel &&
                    data.MaxLevel >= playerLevel)
                {
                    filtered.Add(data);
                }
            }
            return filtered;
        }

        public List<ItemData> FilterByLevel(List<ItemData> itemData)
        {
            int playerLevel = FindObjectOfType<PlayerLevel>().CurrentLevel;
            List<ItemData> filtered = new List<ItemData>();
            foreach (ItemData data in itemData)
            {
                //data.ID == 0 if not found
                if (data.ID != 0 &&
                    data.MinLevel <= playerLevel &&
                    data.MaxLevel >= playerLevel)
                {
                    // log.print($"Before {filtered.Count} {data.ID}");
                    filtered.Add(data);
                    // log.print($"After {filtered.Count} {data.ID}");
                }
            }

            return filtered;
        }

 #if UNITY_EDITOR
        [ContextMenu("Load All Abilities & Consumables")]
        private void LoadAllSOsInProject()
        {
            string[] newAbilityGuids = AssetDatabase.FindAssets("t:NewAbilitySO");
            var allAbilities = new List<NewAbilitySO>();
            var allAbilitiesData = new List<ItemData>();

            foreach (string guid in newAbilityGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var ability = AssetDatabase.LoadAssetAtPath<NewAbilitySO>(path);
                if (ability != null)
                {
                    allAbilities.Add(ability);
                }
            }
            physicalAbilityEntries.Clear();
            magicalAbilityEntries.Clear();
            equipmentModEntries.Clear();
            enemyPhysicalAbilityEntries.Clear();
            enemyMagicalAbilityEntries.Clear();



            PhysicalAbilityItemDatas.Clear();
            MagicalAbilityItemDatas.Clear();
            ConsumableItemDatas.Clear();
            EquipmentModItemDatas.Clear();
            EnemyPhysicalAbilityItemDatas.Clear();
            EnemyMagicalAbilityItemDatas.Clear();


            foreach (var ability in allAbilities)
            {
                switch (ability.AbilityType)
                {
                    default:
                    case AbilityType.PHYSICAL:
                        if (ability.isEnemyAbility)
                        {
                            enemyPhysicalAbilityEntries.Add(ability);
                            EnemyPhysicalAbilityItemDatas.Add(ability.itemData);
                        }
                        else
                        {
                            physicalAbilityEntries.Add(ability);
                            PhysicalAbilityItemDatas.Add(ability.itemData);

                        }
                        break;

                    case AbilityType.MAGICAL:
                        if (ability.isEnemyAbility)
                        {
                            enemyMagicalAbilityEntries.Add(ability);
                            EnemyMagicalAbilityItemDatas.Add(ability.itemData);
                        }
                        else
                        {
                            magicalAbilityEntries.Add(ability);
                            MagicalAbilityItemDatas.Add(ability.itemData);

                        }
                        break;
                }
            }

            string[] consumableGuids = AssetDatabase.FindAssets("t:ConsumableSO");
            var allConsumables = new List<ConsumableSO>();

            foreach (string guid in consumableGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var consumable = AssetDatabase.LoadAssetAtPath<ConsumableSO>(path);
                if (consumable != null)
                {
                    allConsumables.Add(consumable);
                }
            }

            consumableEntries = allConsumables;
            ConsumableItemDatas = consumableEntries.Select(so => so.itemData).ToList();
            string[] guids = AssetDatabase.FindAssets("t:EquipmentModSO");
            var results = new List<EquipmentModSO>();
            foreach (string guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var modSo = AssetDatabase.LoadAssetAtPath<EquipmentModSO>(path);
                if (modSo != null) 
                {
                    results.Add(modSo); 
                }
            }
            equipmentModEntries = results;
            EquipmentModItemDatas = equipmentModEntries.Select(so => so.itemData).ToList();
            // ================ 4) Mark the asset as dirty ================
            EditorUtility.SetDirty(this);
            optional: AssetDatabase.SaveAssets();

            Debug.Log("Database updated with all NewAbilitySO and ConsumableSO assets in the project.");
        }
#endif
    }
}

using System.Collections.Generic;
using System.Linq;
using SystemMiami.AbilitySystem;
using SystemMiami.CombatRefactor;
using SystemMiami.CombatSystem;
using SystemMiami.Management;
using UnityEditor;
using UnityEngine;

namespace SystemMiami
{

    public class Database : Singleton<Database>
    {
      
       [SerializeField] private List<NewAbilitySO> physicalAbilityEntries = new();
       [SerializeField] private List<NewAbilitySO> magicalAbilityEntries = new();
       [SerializeField] private List<ConsumableSO> consumableEntries = new();
       [SerializeField] private List<NewAbilitySO> enemyPhysicalAbilityEntries = new();
       [SerializeField] private List<NewAbilitySO> enemyMagicalAbilityEntries = new();
      
      // [SerializeField] private List<EquipmentModDatabaseEntry> equipmentEntries = new();

       private Dictionary<int, NewAbilitySO> physicalAbilityDatabase;
       private Dictionary<int, NewAbilitySO> magicalAbilityDatabase;
       private Dictionary<int, ConsumableSO> consumableDatabase;
       private Dictionary<int, NewAbilitySO> enemyPhysicalAbilityDatabase;
       private Dictionary<int, NewAbilitySO> enemyMagicalAbilityDatabase;
      // private Dictionary<int, EquipmentModDatabaseEntry> equipmentModDatabase;
       
       private void OnEnable()
       {
           Initialize();
       }

       #if UNITY_EDITOR
        [ContextMenu("Load All Abilities & Consumables")]
        private void LoadAllSOsInProject()
        {
            string[] newAbilityGuids = AssetDatabase.FindAssets("t:NewAbilitySO");
            var allAbilities = new List<NewAbilitySO>();

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
            enemyPhysicalAbilityEntries.Clear();
            enemyMagicalAbilityEntries.Clear();
            foreach (var ability in allAbilities)
            {
                switch (ability.AbilityType)
                {
                    case AbilityType.PHYSICAL:
                        if (ability.isEnemyAbility)
                        {
                            enemyPhysicalAbilityEntries.Add(ability);
                        }
                        else
                        {
                            physicalAbilityEntries.Add(ability);
                            
                        }
                       
                        break;

                    case AbilityType.MAGICAL:
                        if (ability.isEnemyAbility)
                        {
                            enemyMagicalAbilityEntries.Add(ability);
                        }
                        else
                        {
                            magicalAbilityEntries.Add(ability);
                            
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

            // ================ 4) Mark the asset as dirty ================
            EditorUtility.SetDirty(this);
            optional: AssetDatabase.SaveAssets();

            Debug.Log("Database updated with all NewAbilitySO and ConsumableSO assets in the project.");
        }
#endif
       
       public void Initialize()
       {
           // Filter database to only include abilities matching the player's class type.
           Attributes playerAttributes = FindObjectOfType<PlayerManager>().GetComponent<Attributes>();
           CharacterClassType playerClassType = playerAttributes._characterClass;
           
           // Filter physical abilities
           physicalAbilityEntries = physicalAbilityEntries
               .Where(entry => entry.classType == playerClassType ).ToList();
           
           // Filter magical abilities
           magicalAbilityEntries = magicalAbilityEntries
               .Where(entry => entry.classType == playerClassType).ToList();
           
           // Convert lists to dictionaries 
           physicalAbilityDatabase = physicalAbilityEntries.ToDictionary(entry => entry.itemData.ID);
           magicalAbilityDatabase = magicalAbilityEntries.ToDictionary(entry => entry.itemData.ID);
           consumableDatabase = consumableEntries.ToDictionary(entry => entry.itemData.ID);
           enemyPhysicalAbilityDatabase = enemyPhysicalAbilityEntries.ToDictionary(entry => entry.itemData.ID);
           enemyMagicalAbilityDatabase = enemyMagicalAbilityEntries.ToDictionary(entry => entry.itemData.ID);
           //TODO equipmentModDatabase = equipmentEntries.ToDictionary(entry => entry.ID);
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
           
           return IDType switch
           {
               1 => physicalAbilityDatabase.ContainsKey(id) ? physicalAbilityDatabase[id].itemData : default,
               2 => magicalAbilityDatabase.ContainsKey(id) ? magicalAbilityDatabase[id].itemData : default,
               3 => consumableDatabase.ContainsKey(id) ? consumableDatabase[id].itemData : default,
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
           if (enemyPhysicalAbilityDatabase.TryGetValue(id, out  abilityEntry))
           {
               return new AbilityPhysical(abilityEntry, user);
           }

           if (enemyMagicalAbilityDatabase.TryGetValue(id, out abilityEntry))
           {
               return new AbilityMagical(abilityEntry, user);
           }
           else
           {
               Debug.LogWarning($"No matching CombatAction found for ID {id} in GameDatabase!");
           }

           return null;
       }
       
       /// <summary>
       /// Gets the itemData type of that ID(for sorting purposes)
       /// </summary>
       public ItemType GetDataType(int id)
       {
           if (physicalAbilityDatabase.ContainsKey(id)) return ItemType.PhysicalAbility;
           if (magicalAbilityDatabase.ContainsKey(id)) return ItemType.MagicalAbility;
           if (consumableDatabase.ContainsKey(id)) return ItemType.Consumable;
           return ItemType.Consumable; //fallback
       }

    }
}

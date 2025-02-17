using System.Collections;
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
       public static Database Instance;
       [SerializeField] private List<NewAbilitySO> physicalAbilityEntries = new();
       [SerializeField] private List<NewAbilitySO> magicalAbilityEntries = new();
       [SerializeField] private List<ConsumableSO> consumableEntries = new();
       [SerializeField] private List<NewAbilitySO> enemyAbilityEntries = new();
      
      // [SerializeField] private List<EquipmentModDatabaseEntry> equipmentEntries = new();

       private Dictionary<int, NewAbilitySO> physicalAbilityDatabase;
       private Dictionary<int, NewAbilitySO> magicalAbilityDatabase;
       private Dictionary<int, ConsumableSO> consumableDatabase;
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
            foreach (var ability in allAbilities)
            {
                switch (ability.AbilityType)
                {
                    case AbilityType.PHYSICAL:
                        physicalAbilityEntries.Add(ability);
                        break;

                    case AbilityType.MAGICAL:
                        magicalAbilityEntries.Add(ability);
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
            // optional: AssetDatabase.SaveAssets();

            Debug.Log("Database updated with all NewAbilitySO and ConsumableSO assets in the project.");
        }
#endif
       
       public void Initialize()
       {
           // Convert lists to dictionaries 
           physicalAbilityDatabase = physicalAbilityEntries.ToDictionary(entry => entry.Data.ID);
           magicalAbilityDatabase = magicalAbilityEntries.ToDictionary(entry => entry.Data.ID);
           consumableDatabase = consumableEntries.ToDictionary(entry => entry.Data.ID);
           //TODO equipmentModDatabase = equipmentEntries.ToDictionary(entry => entry.ID);
       }
       

       
       public Data GetData(int id, ItemType type)
       {
           int IDType = id / 1000;
           
           return IDType switch
           {
               
               1 => physicalAbilityDatabase.ContainsKey(id) ? physicalAbilityDatabase[id].Data : default,
               2 => magicalAbilityDatabase.ContainsKey(id) ? magicalAbilityDatabase[id].Data : default,
               3 => consumableDatabase.ContainsKey(id) ? consumableDatabase[id].Data : default,
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

           Debug.LogWarning($"No matching CombatAction found for ID {id} in GameDatabase!");
           return null;
       }
       
       /// <summary>
       /// Gets the data type of that ID(for sorting purposes)
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

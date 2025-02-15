using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SystemMiami.AbilitySystem;
using SystemMiami.CombatRefactor;
using SystemMiami.CombatSystem;
using SystemMiami.Management;
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

       
       public void Initialize()
       {
           // Convert lists to dictionaries 
           physicalAbilityDatabase = physicalAbilityEntries.ToDictionary(entry => entry.Data.ID);
           magicalAbilityDatabase = magicalAbilityEntries.ToDictionary(entry => entry.Data.ID);
           consumableDatabase = consumableEntries.ToDictionary(entry => entry.Data.ID);
           //TODO equipmentModDatabase = equipmentEntries.ToDictionary(entry => entry.ID);
       }
       
       public Data GetData(int id, DataType type)
       {
           return type switch
           {
               DataType.PhysicalAbility => physicalAbilityDatabase.ContainsKey(id) ? physicalAbilityDatabase[id].Data : default,
               DataType.MagicalAbility => magicalAbilityDatabase.ContainsKey(id) ? magicalAbilityDatabase[id].Data : default,
               DataType.Consumable => consumableDatabase.ContainsKey(id) ? consumableDatabase[id].Data : default,
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
       public DataType GetDataType(int id)
       {
           if (physicalAbilityDatabase.ContainsKey(id)) return DataType.PhysicalAbility;
           if (magicalAbilityDatabase.ContainsKey(id)) return DataType.MagicalAbility;
           if (consumableDatabase.ContainsKey(id)) return DataType.Consumable;
           return DataType.Consumable; //fallback
       }

    }
}

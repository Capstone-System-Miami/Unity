using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SystemMiami.AbilitySystem;
using SystemMiami.CombatRefactor;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami
{
    [CreateAssetMenu(fileName = "Database", menuName = "Game Database")]
    public class DatabaseSO : ScriptableObject
    {
       public static DatabaseSO instance;
       [SerializeField] private List<NewAbilitySO> abilityEntries = new();
       [SerializeField] private List<ConsumableSO> consumableEntries = new();
      // [SerializeField] private List<EquipmentModDatabaseEntry> equipmentEntries = new();

       private Dictionary<int, NewAbilitySO> abilityDatabase;
       private Dictionary<int, ConsumableSO> consumableDatabase;
      // private Dictionary<int, EquipmentModDatabaseEntry> equipmentModDatabase;
       
       private void OnEnable()
       {
           Initialize();
       }
       public void Initialize()
       {
           // Convert lists to dictionaries 
           abilityDatabase = abilityEntries.ToDictionary(entry => entry.Data.ID);
           consumableDatabase = consumableEntries.ToDictionary(entry => entry.Data.ID);
           //TODO equipmentModDatabase = equipmentEntries.ToDictionary(entry => entry.ID);
       }
       
       public Data GetData(int id, DataType type)
       {
           return type switch
           {
               DataType.Ability => abilityDatabase.ContainsKey(id) ? abilityDatabase[id].Data : default,
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
           if (abilityDatabase.TryGetValue(id, out NewAbilitySO abilityEntry))
           {
               // Distinguish between physical or magical based on the AbilityType
               switch (abilityEntry.AbilityType)
               {
                   case AbilityType.PHYSICAL:
                       return new AbilityPhysical(abilityEntry, user);

                   case AbilityType.MAGICAL:
                       return new AbilityMagical(abilityEntry, user);

                  
               }
           }
           // Check if it's a consumable
           if (consumableDatabase.TryGetValue(id, out ConsumableSO consumableEntry))
           {
               return new Consumable(consumableEntry, user);
           }

           Debug.LogWarning($"No matching CombatAction found for ID {id} in GameDatabase!");
           return null;
       }

    }
}

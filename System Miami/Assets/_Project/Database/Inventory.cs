using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami.CombatSystem //temp namespace
{
    public class Inventory : MonoBehaviour
    {
        [Header("IDs for Items in the Inventory")]
        [SerializeField] private List<int> abilityIDs;     
        [SerializeField] private List<int> consumableIDs;  

        public List<int> AbilityIDs => abilityIDs;
        public List<int> ConsumableIDs => consumableIDs;

       
        public void AddItem(int id)
        {
            // Use the Database to figure out the type
            Data data = Database.Instance.GetData(id, DataType.Ability);
            if (data.ID != 0)
            {
                abilityIDs.Add(id);
                return;
            }

            data = Database.Instance.GetData(id, DataType.Consumable);
            if (data.ID != 0)
            {
                consumableIDs.Add(id);
                return;
            }

            Debug.LogWarning($"No valid item with ID {id} found in the Database.");
        }
    }
}
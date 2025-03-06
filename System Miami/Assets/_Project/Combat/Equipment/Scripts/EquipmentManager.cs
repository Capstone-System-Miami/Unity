using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    public class EquipmentManager : MonoBehaviour
    {
        [SerializeField] private Stats stats;

        private void Awake()
        {
            if (stats == null)
            {
                stats = GetComponent<Stats>();
            }
        }
        
        /// <summary>
        /// Looks up the EquipmentModSO by ID in the database then applies its bonus to the stats
        /// </summary>
        /// <param name="modID"></param>
        public void EquipMod(int modID)
        {
            var db = Database.MGR;
            if (db == null)
            {
                Debug.LogError($"No Database found to look up mod ID={modID}!");
                return;
            }

            EquipmentModSO modSo = db.GetEquipmentMod(modID);
            if (modSo == null)
            {
                Debug.LogWarning($"No EquipmentModSO found with ID={modID} in Database!");
                return;
            }

            // Convert the ScriptableObject's bonuses into a runtime StatSet
            StatSet modStats = new StatSet(modSo.StatBonus);
            
            // Pass it to Stats
            stats.EquipMod(modID, modStats);
        }
        
        /// <summary>
        /// Removes an equipped mod by ID
        /// </summary>
        public void UnequipMod(int modID)
        {
            stats.UnequipMod(modID);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    public class TestEquipEquipmentMod : MonoBehaviour
    {
        public int modIDToEquip;
        
        
        public EquipmentManager equipmentManager;
        /// <summary>
        /// Call this from a UI Button OnClick event to equip the specified mod.
        /// </summary>
        public void OnEquipTestButton()
        {
            if (equipmentManager == null)
            {
                Debug.LogWarning("EquipmentManager reference is missing!");
                return;
            }
            
            Debug.Log($"Attempting to equip mod with ID {modIDToEquip}");
            equipmentManager.EquipMod(modIDToEquip);
        }
    }
}

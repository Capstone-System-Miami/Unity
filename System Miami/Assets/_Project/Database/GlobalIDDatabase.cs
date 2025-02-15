
using UnityEngine;

namespace SystemMiami
{
    [CreateAssetMenu(fileName = "GlobalIDDatabase", menuName = "Data/Global ID Database")]
    public class GlobalIDDatabase : ScriptableObject
    {
        [Header("Unique IDS for each type of object(THIS IS SET BY THE EDITOR WINDOW DON'T TOUCH IT)")]
        public int nextAbilityID = 1000;       // Abilities start with 1
        public int nextConsumableID = 2000;    // Consumables start with 2
        public int nextEquipmentModID = 3000;  // Equipment Mods start with 3
    }
}
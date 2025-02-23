
using UnityEngine;

namespace SystemMiami
{
    [CreateAssetMenu(fileName = "GlobalIDDatabase", menuName = "ItemData/Global ID Database")]
    public class GlobalIDDatabase : ScriptableObject
    {
        [Header("Unique IDS for each type of object(THIS IS SET BY THE EDITOR WINDOW DON'T TOUCH IT)")]
        public int nextPhysicalAbilityID = 1000;       // Abilities start with 1
        public int nextMagicalAbilityID = 2000;       // Abilities start with 2
        public int nextConsumableID = 3000;    // Consumables start with 3
        public int nextEquipmentModID = 4000;  // Equipment Mods start with 4
        public int nextEnemyAbilityID = 5000; //Enemy abilities start with 5
    }
}
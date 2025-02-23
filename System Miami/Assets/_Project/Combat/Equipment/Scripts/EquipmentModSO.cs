using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    [CreateAssetMenu(fileName = "New Equipment Mod", menuName = "Combatant/Equipment Mod")]
    public class EquipmentModSO : ScriptableObject
    {
        
        [Header("Stat Bonuses")]
        [SerializeField] public StatSetSO StatBonus;
       
        [SerializeField] public ItemData itemData;
        
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace SystemMiami
{
    [System.Serializable]
    public struct Data
    {
        public int ID;
        public Sprite Icon;
        public string Name;
        public string Description;
        [FormerlySerializedAs("dataType")] public ItemType itemType;
       

    }

   
    
    //keep everything the same but still have shops take IShopItem in case we ever need it
    
    public enum ItemType
    {
        PhysicalAbility,
        MagicalAbility,
        Consumable,
        EquipmentMod
    }
}

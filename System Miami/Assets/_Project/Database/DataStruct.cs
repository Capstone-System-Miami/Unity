using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace SystemMiami
{
    [System.Serializable]
    public struct ItemData
    {  
        public static readonly ItemData Empty = new ItemData(0, null, "", "", ItemType.PhysicalAbility);
        
        public int ID;
        public Sprite Icon;
        public string Name;
        public string Description;
        [FormerlySerializedAs("dataType")] public ItemType itemType;
       
       public ItemData (int id, Sprite icon, string name, string description, ItemType itemType)
        {
            ID = id;
            Icon = icon;
            Name = name;
            Description = description;
            this.itemType = itemType;
        }
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

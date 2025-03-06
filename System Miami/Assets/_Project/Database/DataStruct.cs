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
        public static readonly ItemData FailedData = new(
            0, null, "", "", 0, 0, 0, 0);
        
        public int ID;
        public Sprite Icon;
        public string Name;
        public string Description;
        [FormerlySerializedAs("dataType")] public ItemType itemType;
        public int Price;
        public int MinLevel;
        public int MaxLevel;
        public Rarity rarity;

        public readonly bool failbit;
       
        public ItemData(
            int id,
            Sprite icon,
            string name,
            string description,
            ItemType itemType,
            Rarity rarity,
            int minLevel,
            int maxLevel)
        {
            failbit = (id == 0);

            ID = id;
            Icon = icon;
            Name = name;
            Description = description;
            this.itemType = itemType;

            // TODO:
            // Is it intentional that none of these vvvv
            // are getting assigned properly? Double check
            // that this is the goal
            int min = minLevel;
            int max = maxLevel;
            Price = 0;
            MinLevel = 0;
            MaxLevel = 0;
            this.rarity = Rarity.Common;
        }
    }

   
    
    //keep everything the same but still have shops take IShopItem in case we ever need it
    
    public enum ItemType
    {
        PhysicalAbility = 1,
        MagicalAbility,
        Consumable,
        EquipmentMod
    }
    
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    public struct Data
    {
        public int ID;
        public Sprite Icon;
        public string AbilityName;
        public string Description;
        public DataType dataType;

    }

    public enum DataType
    {
        PhysicalAbility,
        MagicalAbility,
        Consumable,
        EquipmentMod
    }
}

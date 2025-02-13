using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatRefactor;
using UnityEngine;

namespace SystemMiami
{
    // UI data
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
        Ability,
        Consumable,
        EquipmentMod
    }
}

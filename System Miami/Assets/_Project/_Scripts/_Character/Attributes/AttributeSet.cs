// Authors: Layla Hoey
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    [CreateAssetMenu(fileName = "New Attribute Set", menuName = "Combatant/Base Attribute Set")]
    public class AttributeSet : ScriptableObject
    {
        [SerializeField] private int _strength;
        [SerializeField] private int _dexterity;
        [SerializeField] private int _constitution;
        [SerializeField] private int _wisdom;
        [SerializeField] private int _intelligence;

        public Dictionary<AttributeType, int> Dict = new Dictionary<AttributeType, int>();

        private void OnEnable()
        {
            Dict[AttributeType.STRENGTH] = _strength;
            Dict[AttributeType.DEXTERITY] = _dexterity;
            Dict[AttributeType.CONSTITUTION] = _constitution;
            Dict[AttributeType.WISDOM] = _wisdom;
            Dict[AttributeType.INTELLIGENCE] = _intelligence;
        }

    }
}

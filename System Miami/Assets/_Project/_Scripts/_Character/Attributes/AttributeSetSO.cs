// Authors: Layla Hoey
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    [CreateAssetMenu(fileName = "New Attribute Set", menuName = "Combatant/Base Attribute Set")]
    public class AttributeSetSO : ScriptableObject
    {
        public int _strength;
        public int _dexterity;
        public int _constitution;
        public int _wisdom;
        public int _intelligence;
    }

    public class AttributeSet
    {
        public Dictionary<AttributeType, int> Dict = new Dictionary<AttributeType, int>();

        public AttributeSet()
        {
            int len = CharacterEnums.ATTRIBUTE_COUNT;

            for(int i = 0; i < len; i++)
            {
                Dict[(AttributeType)i] = 0;
            }
        }

        public AttributeSet(AttributeSetSO scriptable)
        {
            Dict[AttributeType.STRENGTH] = scriptable._strength;
            Dict[AttributeType.DEXTERITY] = scriptable._dexterity;
            Dict[AttributeType.CONSTITUTION] = scriptable._constitution;
            Dict[AttributeType.WISDOM] = scriptable._wisdom;
            Dict[AttributeType.INTELLIGENCE] = scriptable._intelligence;
        }

        public AttributeSet(int[] vals)
        {
            if (vals.Length != CharacterEnums.ATTRIBUTE_COUNT)
            {
                zero(ref vals);
            }

            foreach (AttributeType attr in Dict.Keys)
            {
                Dict[attr] = vals[(int)attr];
            }
        }

        private void zero(ref int[] incoming)
        {
            incoming = new int[CharacterEnums.ATTRIBUTE_COUNT];

            for (int i = 0; i < incoming.Length; i++)
            {
                incoming[i] = 0;
            }
        }

        public int Get(AttributeType attr)
        {
            if (Dict.ContainsKey(attr))
            {
                return Dict[attr];
            }
            else
            {
                return 0;
            }
        }

        public void Set(AttributeType attr, int value)
        {
            if(Dict.ContainsKey(attr))
            {
                Dict[attr] = value;
            }
        }
    }
}

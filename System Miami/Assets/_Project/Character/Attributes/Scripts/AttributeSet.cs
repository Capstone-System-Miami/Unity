// Authors: Layla Hoey
using System.Collections.Generic;

namespace SystemMiami
{
    [System.Serializable]
    public class AttributeSet
    {
        private Dictionary<AttributeType, int> _dict = new Dictionary<AttributeType, int>();

        public AttributeSet()
        {
            int len = CharacterEnums.ATTRIBUTE_COUNT;

            for(int i = 0; i < len; i++)
            {
                _dict[(AttributeType)i] = 0;
            }
        }

        public AttributeSet(AttributeSetSO scriptable)
        {
            _dict[AttributeType.STRENGTH]       = scriptable.Strength;
            _dict[AttributeType.DEXTERITY]      = scriptable.Dexterity;
            _dict[AttributeType.CONSTITUTION]   = scriptable.Constitution;
            _dict[AttributeType.WISDOM]         = scriptable.Wisdom;
            _dict[AttributeType.INTELLIGENCE]   = scriptable.Intelligence;
        }

        public AttributeSet(AttributeSet toCopy)
        {
            _dict[AttributeType.STRENGTH]       = toCopy._dict[AttributeType.STRENGTH];
            _dict[AttributeType.DEXTERITY]      = toCopy._dict[AttributeType.DEXTERITY];
            _dict[AttributeType.CONSTITUTION]   = toCopy._dict[AttributeType.CONSTITUTION];
            _dict[AttributeType.WISDOM]         = toCopy._dict[AttributeType.WISDOM];
            _dict[AttributeType.INTELLIGENCE]   = toCopy._dict[AttributeType.INTELLIGENCE];
        }

        public AttributeSet(int[] vals)
        {
            if (vals.Length != CharacterEnums.ATTRIBUTE_COUNT)
            {
                zero(ref vals);
            }

            foreach (AttributeType attr in _dict.Keys)
            {
                _dict[attr] = vals[(int)attr];
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
            if (_dict.ContainsKey(attr))
            {
                return _dict[attr];
            }
            else
            {
                return 0;
            }
        }

        public void Set(AttributeType attr, int value)
        {
            if(_dict.ContainsKey(attr))
            {
                _dict[attr] = value;
            }
        }
    }
}

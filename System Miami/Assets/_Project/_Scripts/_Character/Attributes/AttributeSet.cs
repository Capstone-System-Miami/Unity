// Authors: Layla Hoey
using UnityEngine;

namespace SystemMiami
{
    [CreateAssetMenu(fileName = "New Attribute Set", menuName = "CharacterInfo/Base Attribute Set")]
    public class AttributeSet : ScriptableObject
    {
        public int _strength;
        public int _dexterity;
        public int _constitution;
        public int _wisdom;
        public int _intelligence;
    }
}

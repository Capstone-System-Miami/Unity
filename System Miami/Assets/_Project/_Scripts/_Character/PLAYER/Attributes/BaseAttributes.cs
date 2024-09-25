using UnityEngine;

namespace SystemMiami
{
    [CreateAssetMenu(fileName = "Base Attributes", menuName = "Base Attribute Set")]
    public class BaseAttributes : ScriptableObject
    {
        public int _strength;
        public int _dexterity;
        public int _constitution;
        public int _wisdom;
        public int _intelligence;
    }
}

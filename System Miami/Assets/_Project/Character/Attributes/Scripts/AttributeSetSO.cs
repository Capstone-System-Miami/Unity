// Authors: Layla Hoey
using UnityEngine;

namespace SystemMiami
{
    [CreateAssetMenu(fileName = "New Attribute Set", menuName = "Combatant/Base Attribute Set")]
    public class AttributeSetSO : ScriptableObject
    {
        public int Strength;
        public int Dexterity;
        public int Constitution;
        public int Wisdom;
        public int Intelligence;
    }
}

using UnityEngine;

namespace SystemMiami
{
    [CreateAssetMenu(fileName = "New Stat Set", menuName = "Combatant/Stat Modification Set")]
    public class StatSetSO : ScriptableObject
    {
        public float PhysicalPower;
        public float MagicalPower;
        public float PhysicalSlots;
        public float MagicalSlots;
        public float Stamina;
        public float Mana;
        public float MaxHealth;
        public float DamageRDX;
        public float Speed;
    }
}

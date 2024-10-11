using UnityEngine;

namespace SystemMiami
{
    public interface IHealth
    {
        void Heal();
        void Heal(float amount);
        void TakeDamage(float amount);
    }
}

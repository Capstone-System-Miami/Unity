using UnityEngine;

namespace SystemMiami
{
    public interface IDamageable
    {
        bool IsCurrentlyDamageable();
        void Damage(float amount);
    }
}

using UnityEngine;

namespace SystemMiami
{
    public interface IHealable
    {
        bool IsCurrentlyHealable();
        void Heal();
        void Heal(float amount);
        void HealPercent(float percent);
    }
}

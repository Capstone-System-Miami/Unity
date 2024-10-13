using UnityEngine;

namespace SystemMiami
{
    public interface IHealable
    {
        void Heal();
        void Heal(float amount);
    }
}

using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public interface ITargetable
    {
        void HandleBeginTargeting(Color preferredColor);
        void HandleEndTargeting(Color preferredColor);
        bool TryGetDamageable(IDamageable damageInterface);
        bool TryGetHealable(IHealable healInterface);
        bool TryGetMovable(IMovable moveInterface);
    }
}

using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public interface ITargetable
    {
        void HandleBeginTargeting(Color preferredColor);
        void HandleEndTargeting(Color preferredColor);
        bool TryGetDamageable(out IDamageable damageInterface);
        bool TryGetHealable(out IHealable healInterface);
        bool TryGetMovable(out IMovable moveInterface);
    }
}

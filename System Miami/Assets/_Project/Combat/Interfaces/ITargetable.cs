using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public interface ITargetable
    {
        void HandleBeginTargeting(Color preferredColor);
        void HandleEndTargeting(Color preferredColor);
        bool TryGetDamageable(out IDamageReciever damageInterface);
        bool TryGetHealable(out IHealReciever healInterface);
        bool TryGetMovable(out IForceMoveReciever moveInterface);
    }
}

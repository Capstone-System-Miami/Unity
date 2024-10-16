using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public interface ITargetable
    {
        void Target();
        void Target(Color color);
        void UnTarget();
        GameObject GameObject();
    }
}

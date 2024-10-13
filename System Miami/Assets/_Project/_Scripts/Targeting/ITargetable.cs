using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public interface ITargetable
    {
        void Target();
        void UnTarget();
        GameObject GameObject();
    }
}

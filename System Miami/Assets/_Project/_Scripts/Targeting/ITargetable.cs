using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public interface ITargetable
    {
        void Target();
        void Highlight(Color color);
        void UnHighlight();
        GameObject GameObject();
    }
}

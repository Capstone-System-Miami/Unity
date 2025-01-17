using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public interface IHighlightable
    {
        void Highlight();
        void Highlight(Color color);
        void UnHighlight();
        GameObject GameObject();
    }
}

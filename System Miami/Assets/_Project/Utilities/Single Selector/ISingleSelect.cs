using UnityEngine;

namespace SystemMiami
{
    public interface ISingleSelectable : ISelectable
    {
        int SelectionIndex { get; set; }
    }
}

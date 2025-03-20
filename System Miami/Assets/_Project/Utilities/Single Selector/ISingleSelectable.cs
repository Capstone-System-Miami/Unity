using System;
using UnityEngine;
using SystemMiami.Utilities;

namespace SystemMiami
{
    public interface ISingleSelectable : ISelectable
    {
        int SelectionIndex { get; set; }
    }
}

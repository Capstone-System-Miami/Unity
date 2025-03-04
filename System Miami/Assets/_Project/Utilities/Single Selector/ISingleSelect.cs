using System;
using UnityEngine;
using SystemMiami.Utilities;

namespace SystemMiami
{
    public interface ISingleSelectable : ISelectable
    {
        SingleSelector Reference { get; set; }
        int SelectionIndex { get; set; }
    }
}

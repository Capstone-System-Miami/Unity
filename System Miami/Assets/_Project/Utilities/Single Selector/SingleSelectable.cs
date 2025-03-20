using UnityEngine;

namespace SystemMiami
{
    public class SingleSelectable : MonoBehaviour, ISingleSelectable
    {
        int ISingleSelectable.SelectionIndex { get; set; }

        public bool IsSelected {get; private set; }

        public void Select()
        {
            IsSelected = true;
        }

        public void Deselect()
        {
            IsSelected = false;
        }
    }
}

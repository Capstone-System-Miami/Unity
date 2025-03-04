using UnityEngine;

namespace SystemMiami
{
    public class InventoryTabButton : SelectableButton
    {
        private InventoryTab tab;

        public void Initialize(InventoryTab tab)
        {
            this.tab = tab;
        }
    }
}

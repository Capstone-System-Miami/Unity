using SystemMiami.InventorySystem;
using SystemMiami.ui;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    public class InventoryTab : ISingleSelectable
    {
        [SerializeField] private dbug log;

        [SerializeField] private SingleSelectButton button;
        [SerializeField] private ItemGrid itemGrid;

        private InventoryUI inventoryUI;


        public SingleSelectButton Button { get { return button; } }
        public ItemGrid ItemGrid { get { return itemGrid; } }
        public ItemType ValidItemType { get; private set; }

        public bool IsSelected => (this as ISingleSelectable).Reference != null
            ? (this as ISingleSelectable).Reference.CurrentSelection == this as ISingleSelectable
            : false;

        public void Initialize(InventoryUI inventoryUI, ItemType itemType)
        {
            this.inventoryUI = inventoryUI;
            ValidItemType = itemType;
            ItemGrid.Initialize(itemType);
        }


        // ISingleSelectable implementation
        SingleSelector ISingleSelectable.Reference { get; set; }
        int ISingleSelectable.SelectionIndex { get; set; }

        void ISelectable.Select()
        {
            itemGrid.gameObject.SetActive(true);
            inventoryUI.activeGrid = ItemGrid;
        }

        void ISelectable.Deselect()
        {
            itemGrid.gameObject.SetActive(false);
        }
    }
}

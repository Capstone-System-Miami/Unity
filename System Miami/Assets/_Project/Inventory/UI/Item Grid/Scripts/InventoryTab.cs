using SystemMiami.InventorySystem;
using SystemMiami.ui;
using SystemMiami.Utilities;
using UnityEditor.UI;
using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    public class InventoryTab : ISingleSelectable
    {
        [SerializeField] private dbug log;

        [Header("Internal Refs")]
        [SerializeField] private SelectableButton button;
        [SerializeField] private ItemGrid itemGrid;

        private InventoryUI inventoryUI;
        private ItemType validItemType;
        private int selectionIndex;

        public SelectableButton Button { get { return button; } }
        public ItemGrid ItemGrid { get { return itemGrid; } }

        public void Initialize(InventoryUI inventoryUI, ItemType itemType)
        {
            this.inventoryUI = inventoryUI;
            validItemType = itemType;
            ItemGrid.Initialize(itemType);
        }

        int ISingleSelectable.SelectionIndex
        {
            get { return selectionIndex; }
            set { selectionIndex = value; }
        }

        void ISelectable.Select()
        {
            itemGrid.gameObject.SetActive(true);
            if (!button.IsSelected)
            {
                button.Select();
            }
        }

        void ISelectable.Deselect()
        {
            itemGrid.gameObject.SetActive(false);

            if (button.IsSelected)
            {
                button.Deselect();
            }
        }
    }
}

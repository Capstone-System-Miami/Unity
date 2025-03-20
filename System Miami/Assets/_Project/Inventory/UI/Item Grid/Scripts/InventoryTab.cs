using SystemMiami.InventorySystem;
using SystemMiami.ui;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami
{
    public class InventoryTab : MonoBehaviour, ISingleSelectable
    {
        [SerializeField] private dbug log;

        [SerializeField] private ItemGrid itemGrid;

        public ItemGrid ItemGrid { get { return itemGrid; } }
        public ItemType ValidItemType { get; private set; }

        public bool IsSelected { get; private set; }

        public void Initialize(ItemType itemType)
        {
            ValidItemType = itemType;
            ItemGrid.Initialize(itemType);
        }


        // ISingleSelectable implementation
        int ISingleSelectable.SelectionIndex { get; set; }

        void ISelectable.Select()
        {
            IsSelected = true;
            itemGrid.gameObject.SetActive(true);
        }

        void ISelectable.Deselect()
        {
            IsSelected = false;
            itemGrid.gameObject.SetActive(false);
        }
    }
}

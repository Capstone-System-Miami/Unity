using System.Collections.Generic;
using System.Linq;
using SystemMiami.Utilities;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace SystemMiami.ui
{
    public enum GridConstraint { NONE, ROW, COL };

    public class ItemGrid : MonoBehaviour
    {
        [SerializeField] private dbug log;

        [Header("Settings")]
        [SerializeField] private GameObject slotPrefab;

        [Header("InternalRefs")]
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private CanvasScaler scaler;


        [field: SerializeField, ReadOnly] public int Cols { get; private set; }
        [field: SerializeField, ReadOnly] public int Rows { get; private set; }
        [field: SerializeField, ReadOnly] public int ConstrainedPropCount { get; private set; }
        [field: SerializeField, ReadOnly] public int SecondaryPropCount { get; private set; }

        [field: SerializeField, ReadOnly] public ItemType ItemType { get; private set; }
        [field: SerializeField, ReadOnly] public List<InventoryItemSlot> Slots { get; private set; }

        /// <summary>
        /// This property is a Lazy Loader
        /// </summary>
        public RectTransform RT
        {
            get
            {
                if (rectTransform == null)
                {
                    rectTransform = GetComponent<RectTransform>();
                }
                return rectTransform;
            }
        }

        /// <summary>
        /// This property is a Lazy Loader
        /// </summary>
        private GridLayoutGroup GridLayoutGroup
        {
            get
            {
                if (gridLayoutGroup == null)
                {
                    gridLayoutGroup = GetComponent<GridLayoutGroup>();
                }
                return gridLayoutGroup;
            }
        }

        private GridConstraint Constraint => GridLayoutGroup.constraint switch
        {
            GridLayoutGroup.Constraint.FixedColumnCount => GridConstraint.COL,
            GridLayoutGroup.Constraint.FixedRowCount    => GridConstraint.ROW,
            _                                           => GridConstraint.NONE
        };

        public int SlotCount
        {
            get
            {
                return GridLayoutGroup.transform.childCount;
            }
        }

        /// NOTE: This is a subscription to <see cref="Inventory.InventoryChanged">,
        /// which doesn't do anything right now.
        ///
        // private void OnEnable()
        // {
        //     PlayerManager.MGR.inventory.InventoryChanged += HandleInventoryChanged;
        // }
        //
        // private void OnDisable()
        // {
        //     PlayerManager.MGR.inventory.InventoryChanged -= HandleInventoryChanged;
        // }

        private void Start()
        {
        }

        private void Update()
        {
            UpdateCounts();
            UpdateConstraints();
            UpdateSize();
        }

        public void Initialize(ItemType type)
        {
            ItemType = type;

            Slots = new List<InventoryItemSlot>();
            for (int i = 0; i < SlotCount; i++)
            {
                InventoryItemSlot slot = GridLayoutGroup.transform.GetChild(i).GetComponent<InventoryItemSlot>();

                Assert.IsNotNull(slot);
                Slots.Add(slot);
            }
        }

        // Fill each slot with the corresponding item ID,
        // until we run out of slots or IDs.
        public void FillSlots(List<int> ids)
        {
            int minCount = Mathf.Min(SlotCount, ids.Count);

            for (int i = 0; i < minCount; i++)
            {
                Assert.IsNotNull(Slots);
                Assert.IsTrue(Slots.Count >= i);
                Assert.IsNotNull(ids);
                Assert.IsTrue(ids.Count >= i);

                // log.print($"Filling slot {i} with {ids[i]}");
                if (!Slots[i].TryFill(ids[i]))
                {
                    log.error(
                        $"{name} could not fill {Slots[i]}. SKIPPING");
                    continue;
                }
            }
        }

        // Clear all slots so they don't display old itemData.
        public void ClearSlots()
        {
            for (int i = 0; i < Slots.Count; i++)
            {
                Slots[i].ClearSlot();
            }
        }


        public void AddNew(List<int> toAdd)
        {
            for (int i = 0; i < Slots.Count; i++)
            {
                if (!Slots[i].TryFill(toAdd[i]))
                {
                    Debug.Log($"Couldn't Add {Database.MGR.GetDataWithJustID(toAdd[i])}");
                }
            }
        }

        public void RemoveExisting(List<int> toRemove)
        {
            for (int i = 0; i < Slots.Count; i++)
            {
                Slots[i].ClearSlot();

                // Would anything else need to happen in here?
            }
        }

        private void UpdateCounts()
        {
            switch (Constraint)
            {
                case GridConstraint.COL:
                    Cols = GridLayoutGroup.constraintCount;
                    Rows = (GridLayoutGroup.transform.childCount % Cols == 0)
                        ? (GridLayoutGroup.transform.childCount / Cols)
                        : (GridLayoutGroup.transform.childCount / Cols) + 1;
                    break;
                case GridConstraint.ROW:
                    Rows = GridLayoutGroup.constraintCount;
                    Cols = (GridLayoutGroup.transform.childCount % Rows == 0)
                        ? (GridLayoutGroup.transform.childCount / Rows)
                        : (GridLayoutGroup.transform.childCount / Rows) + 1;
                    break;
                default:
                    break;
            }
        }

        private void UpdateConstraints()
        {
            switch (Constraint)
            {
                case GridConstraint.COL:
                    ConstrainedPropCount = Cols;
                    SecondaryPropCount = Rows;
                    break;
                case GridConstraint.ROW:
                    ConstrainedPropCount = Rows;
                    SecondaryPropCount = Cols;
                    break;
                default:
                    ConstrainedPropCount = 0;
                    SecondaryPropCount = 0;
                    break;
            }
        }

        private void UpdateSize()
        {
            DimensionInfo currentDimInfo = Constraint switch
            {
                GridConstraint.COL  => new (
                    Rows,
                    GridLayoutGroup.cellSize.y,
                    GridLayoutGroup.padding.top,
                    GridLayoutGroup.padding.bottom,
                    GridLayoutGroup.spacing.y,
                    Rows),

                GridConstraint.ROW  => new (
                    Cols,
                    GridLayoutGroup.cellSize.x,
                    GridLayoutGroup.padding.left,
                    GridLayoutGroup.padding.right,
                    GridLayoutGroup.spacing.x,
                    Cols),

                _ => new(0f, 0f, 0f, 0f, 0f, 0f)
            };

            //(amount of (secondaryElements) * size of cells(secondary))
            //+ padding (secondary1) & (secondary2)
            //+ (spacing * (amount of (secondaryElements) -1)

            float secondaryDimension = currentDimInfo.GetCurrentLength();

            RT.sizeDelta = Constraint switch
            {
                GridConstraint.COL =>
                    new Vector2 (RT.sizeDelta.x, secondaryDimension),

                GridConstraint.ROW =>
                    new Vector2 (secondaryDimension, RT.sizeDelta.y),

                _ => Vector2.zero
            };
        }

        private void HandleInventoryChanged(object sender, InventoryChangedEventArgs args)
        {
            if (args.itemType != this.ItemType) { return; }

            RemoveExisting(args.itemsRemoved);
            AddNew(args.itemsAdded);
        }

        private class DimensionInfo
        {
            public readonly float numberOfElements;
            public readonly float cellSize;
            public readonly float padding1;
            public readonly float padding2;
            public readonly float spacing;
            public readonly float numberOfSpaces;

            public DimensionInfo(
                float numberOfElements,
                float cellSize,
                float padding1,
                float padding2,
                float spacing,
                float numberOfSpaces)
            {
                this.numberOfElements = numberOfElements;
                this.cellSize = cellSize;
                this.padding1 = padding1;
                this.padding2 = padding2;
                this.spacing = spacing;
                this.numberOfSpaces = numberOfSpaces;
            }

            public float GetCurrentLength()
            {
                return
                    (numberOfElements * cellSize)
                    + padding1 /*+ padding2*/
                    + (spacing * numberOfSpaces);
            }
        }
    }
}

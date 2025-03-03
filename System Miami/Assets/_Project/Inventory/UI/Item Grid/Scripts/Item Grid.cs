using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami.ui
{
    public enum GridConstraint { NONE, ROW, COL };

    public class ItemGrid : MonoBehaviour
    {
        [Header("InternalRefs")]
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private CanvasScaler scaler;

        [field: SerializeField, ReadOnly] public int Cols { get; private set; }
        [field: SerializeField, ReadOnly] public int Rows { get; private set; }
        [field: SerializeField, ReadOnly] public float ConstrainedProp { get; private set; }
        [field: SerializeField, ReadOnly] public float SecondaryProp { get; private set; }

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

        /// <summary>
        /// This property is a Lazy Loader
        /// </summary>
        private RectTransform RT
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

        private GridConstraint Constraint => GridLayoutGroup.constraint switch
        {
            GridLayoutGroup.Constraint.FixedColumnCount => GridConstraint.COL,
            GridLayoutGroup.Constraint.FixedRowCount    => GridConstraint.ROW,
            _                                           => GridConstraint.NONE
        };

        private void Start()
        {
        }

        private void Update()
        {
            UpdateCounts();
            UpdateConstraints();
            UpdateSize();
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
                    ConstrainedProp = Cols;
                    SecondaryProp = Rows;
                    break;
                case GridConstraint.ROW:
                    ConstrainedProp = Rows;
                    SecondaryProp = Cols;
                    break;
                default:
                    ConstrainedProp = 0f;
                    SecondaryProp = 0f;
                    break;
            }
        }



        private void UpdateSize()
        {
            Vector2 result;

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

            result = Constraint switch
            {
                GridConstraint.COL =>
                    RT.sizeDelta = new Vector2 (RT.sizeDelta.x, secondaryDimension),

                GridConstraint.ROW =>
                    RT.sizeDelta = new Vector2 (secondaryDimension, RT.sizeDelta.y),

                _ => Vector2.zero
            };
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
                    + padding1 + padding2
                    + (spacing * numberOfSpaces);
            }
        }
    }
}

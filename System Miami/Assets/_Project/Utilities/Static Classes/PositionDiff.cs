using UnityEngine;

namespace SystemMiami.Utilities
{
    public readonly struct PositionDiff
    {
        public readonly int rawX;
        public readonly int rawY;

        public readonly int x;
        public readonly int y;

        public readonly int m;

        public PositionDiff(GameObject from, GameObject to)
            : this( from.transform, to.transform )
        { }

        public PositionDiff(Transform from, Transform to)
            : this( from.position, to.position )
        { }

        public PositionDiff(Vector3 from, Vector3 to)
            : this( (Vector2)from, (Vector2)to )
        { }

        public PositionDiff(Vector2 from, Vector2 to)
            : this(
                    new Vector2Int((int)from.x, (int)from.y),
                    new Vector2Int((int)to.x, (int)to.y)
            )
        { }

        public PositionDiff(Vector2Int from, Vector2Int to)
        {
            rawX = to.x - from.x;
            rawY = to.y - from.y;

            x = Mathf.Abs(rawX);
            y = Mathf.Abs(rawY);

            m = Mathf.Abs(x + y);
        }
    }
}

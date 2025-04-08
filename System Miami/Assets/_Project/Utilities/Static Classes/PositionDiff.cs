using UnityEngine;

namespace SystemMiami.Utilities
{
    public readonly struct PositionDiff
    {
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
            : this( (Vector2Int)from, (Vector2Int)to )
        { }

        public PositionDiff(Vector2Int from, Vector2Int to)
        {
            x = to.x - from.x;
            y = to.y - from.y;
            m = x + y;
        }
    }
}

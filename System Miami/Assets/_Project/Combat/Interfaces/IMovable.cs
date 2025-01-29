using UnityEngine;

namespace SystemMiami
{
    public interface IMovable
    {
        bool IsCurrentlyMovable();
        Vector2Int GetTilePos();
        bool TryMoveTo(Vector2Int tilePos);
        bool TryMoveInDirection(Vector2Int direction, int distance);
    }
}

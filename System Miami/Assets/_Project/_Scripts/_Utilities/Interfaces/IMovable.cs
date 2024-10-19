using UnityEngine;

namespace SystemMiami
{
    public interface IMovable
    {
        Vector2Int GetTilePos();
        bool TryMoveTo(Vector2Int tilePos);
        bool TryMoveInDirection(Vector2Int direction, int distance);
    }
}

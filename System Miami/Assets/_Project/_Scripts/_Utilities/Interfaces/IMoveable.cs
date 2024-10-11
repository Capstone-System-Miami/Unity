using UnityEngine;

namespace SystemMiami
{
    public interface IMoveable
    {
        Vector3Int GetTilePos();
        Vector2Int GetTilePos2D();
        Vector3 GetScreenPos();
        bool TryMoveTo(Vector3 screenPos);
        bool TryMoveTo(Vector3Int tilePos);
    }
}

using UnityEngine;

namespace SystemMiami
{
    public interface IMovable
    {
        Vector2Int GetTilePos2D();
        Vector3Int GetTilePos3D();
        Vector3 GetScreenPos();
        bool TryMoveTo(Vector3 screenPos);
        bool TryMoveTo(Vector3Int tilePos);
    }
}

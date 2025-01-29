using UnityEngine;

namespace SystemMiami.Interfaces
{
    public interface ITileOccupier
    {
        void AddTo(OverlayTile tile);
        void RemoveFrom(OverlayTile tile);
    }
}

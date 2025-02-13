// Author: Alec, Layla
namespace SystemMiami
{
    public interface ITileOccupant
    {
        OverlayTile PositionTile { get; set; }
        void SnapToPositionTile();
    }
}

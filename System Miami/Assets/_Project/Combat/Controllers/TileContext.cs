using UnityEngine;

namespace SystemMiami
{
    public struct TileContext
    {
        readonly OverlayTile Current;
        readonly OverlayTile Focus;
        readonly OverlayTile Destination;

        bool DestinationReached
        {
            get
            {
                return Current == Destination;
            }
        }

        public TileContext(
            OverlayTile current,
            OverlayTile focus,
            OverlayTile destination
            )
        {
            Current = current;
            Focus = focus;
            Destination = destination;
        }
    }
}

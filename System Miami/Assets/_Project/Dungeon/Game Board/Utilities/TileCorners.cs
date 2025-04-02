using UnityEngine;

namespace SystemMiami
{
    public class TileCorners
    {
        public readonly OverlayTile bottom;
        public readonly OverlayTile top;
        public readonly OverlayTile left;
        public readonly OverlayTile right;

        public readonly Vector2Int bottomPos;
        public readonly Vector2Int topPos;
        public readonly Vector2Int leftPos;
        public readonly Vector2Int rightPos;

        public int xMin => leftPos.x;
        public int xMax => rightPos.x;
        public int yMin => bottomPos.y;
        public int yMax => topPos.y;

        public TileCorners(
            OverlayTile bottom,
            OverlayTile top,
            OverlayTile left,
            OverlayTile right)
        {
            this.bottom = bottom;
            this.top    = top;
            this.left   = left;
            this.right  = right;

            bottomPos   = bottom.BoardPos;
            topPos      = top.BoardPos;
            leftPos     = left.BoardPos;
            rightPos    = right.BoardPos;
        }
    }
}

using UnityEngine;

namespace SystemMiami
{
    public struct TileContext
    {
        public readonly OverlayTile Current;
        public readonly OverlayTile Focus;

        public TileContext(
            OverlayTile current,
            OverlayTile focus
            )
        {
            Current = current;
            Focus = focus;
        }

        // Equality operations overloading / overriding
        public static bool operator ==(TileContext a, TileContext b)
        {
            return a.Current == b.Current
                && a.Focus == b.Focus;
        }

        public static bool operator !=(TileContext a, TileContext b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TileContext))
                return false;

            TileContext other = (TileContext)obj;
            return this == other;
        }

        public override int GetHashCode()
        {
            return (Current, Focus).GetHashCode();
        }
    }
}

// Authors: Layla Hoey
using SystemMiami.Enums;
using UnityEngine;

namespace SystemMiami.Utilities
{
    /// <summary>
    /// A struct containing 3 Vector2Ints.
    /// On construction, it stores a difference Vector based
    /// on the mapPositionA and mapPositionB given.
    /// </summary>
    public struct DirectionContext
    {
        // The incoming a coordinate
        public readonly Vector2Int BoardPositionA;

        // The incoming b coordinate
        public readonly Vector2Int BoardPositionB;

        // The Direction the object is facing
        public readonly Vector2Int DirectionVec;
        public readonly TileDir BoardDirection;

        /// <summary>
        /// An adjusted direction corresponding
        /// to a direction on the screen.
        /// </summary>
        public readonly TileDir ScreenDirection;

        /// <summary>
        /// Game board (map) coordinates one tile "forward"
        /// from BoardPositionA in whatever Direction
        /// we've calculated using the incoming points
        /// </summary>
        public readonly Vector2Int ForwardA;

        /// <summary>
        /// Game board (map) coordinates one tile "forward"
        /// from BoardPositionB in whatever Direction
        /// we've calculated using the incoming points
        /// </summary>
        public readonly Vector2Int ForwardB;


        public DirectionContext(Vector2Int boardPositionA, Vector2Int boardPositionB)
        {
            BoardPositionA = boardPositionA;
            BoardPositionB = boardPositionB;

            DirectionVec = DirectionHelper.GetDirectionVec(boardPositionA, boardPositionB);

            BoardDirection = DirectionHelper.GetBoardTileDir(DirectionVec);

            ScreenDirection = DirectionHelper.GetScreenTileDir(BoardDirection);

            ForwardA = BoardPositionA + DirectionVec;
            ForwardB = BoardPositionB + DirectionVec;
        }


        // Equality operations overloading / overriding
        public static bool operator ==(DirectionContext a, DirectionContext b)
        {
            return a.BoardPositionA == b.BoardPositionA
                && a.BoardPositionB == b.BoardPositionB;
        }

        public static bool operator !=(DirectionContext a, DirectionContext b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DirectionContext))
                return false;

            DirectionContext other = (DirectionContext)obj;
            return this == other;
        }

        public override int GetHashCode()
        {
            return (BoardPositionA, BoardPositionB).GetHashCode();
        }
    }
}

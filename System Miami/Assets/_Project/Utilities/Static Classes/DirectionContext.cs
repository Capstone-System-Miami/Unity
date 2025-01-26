// Authors: Layla Hoey
using SystemMiami.Enums;
using UnityEngine;

namespace SystemMiami.Utilities
{
    /// <summary>
    /// A struct containing helpful directional
    /// information regarding an origin coordinate and
    /// a given second coordinate.
    /// 
    /// <para>
    /// Stores:</para>
    /// <para>-Both incoming positions,</para>
    /// 
    /// <para>-The direction from PositionA to PositionB
    /// (in Vector form and enum form)</para>
    /// 
    /// <para>-Positions that are one position forward
    /// from each input tile, in the direction
    /// we've calculated.</para>
    /// 
    /// </summary>
    public struct DirectionContext
    {
        /// <summary>
        /// A board / map / grid position
        /// that we want to use as the 'origin'
        /// for this directional information.
        /// </summary>
        public readonly Vector2Int TilePositionA;

        /// <summary>
        /// A board / map / grid position
        /// that we would consider to be "forward"
        /// from TilePositionA.
        /// </summary>
        public readonly Vector2Int TilePositionB;

        /// <summary>
        /// The direction vector in TilePosition units
        /// from TilePositionA to TilePositionB.
        /// 
        /// <para>
        /// *Note that this has nothing to do with
        /// distance</para>
        /// </summary>
        public readonly Vector2Int DirectionVec;

        /// <summary>
        /// An enumerated direction that we can use
        /// for various tile-related operations.
        /// </summary>
        public readonly TileDir BoardDirection;

        /// <summary>
        /// An adjusted direction corresponding
        /// to a direction on the screen.
        /// </summary>
        public readonly ScreenDir ScreenDirection;

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
            TilePositionA = boardPositionA;
            TilePositionB = boardPositionB;

            DirectionVec = DirectionHelper.GetDirectionVec(boardPositionA, boardPositionB);

            BoardDirection = DirectionHelper.GetTileDir(DirectionVec);

            ScreenDirection = DirectionHelper.GetScreenDir(BoardDirection);

            ForwardA = TilePositionA + DirectionVec;
            ForwardB = TilePositionB + DirectionVec;
        }


        // Equality operations overloading / overriding
        //
        // We're doing this so that it doesn't check
        // Every part of the struct when we check if
        // they are the same. If the TilePosition Vector2Ints
        // are the same between the two DirectionContexts we're
        // comparing, then everything else will necessarily be the same.

        public static bool operator ==(DirectionContext a, DirectionContext b)
        {
            return a.TilePositionA == b.TilePositionA
                && a.TilePositionB == b.TilePositionB;
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
            return (TilePositionA, TilePositionB).GetHashCode();
        }
    }
}

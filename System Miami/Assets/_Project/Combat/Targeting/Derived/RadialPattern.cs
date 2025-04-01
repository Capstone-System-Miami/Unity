using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami
{
    /// <summary>
    /// A pattern that checks a ring of tiles at a specified radial distance
    /// from the origin.
    /// </summary>
    [CreateAssetMenu(
        fileName = "New Radial Pattern",
        menuName = "Combat Subaction/Targeting Patterns/Radial")]
    public class RadialPattern : TargetingPattern
    {
        [Tooltip("Radius of the pattern, in Tiles.")]
        [SerializeField] private int radius;

        [Header("Directions")]
        [SerializeField] private bool front;
        [SerializeField] private bool frontRight;
        [SerializeField] private bool right;
        [SerializeField] private bool backRight;
        [SerializeField] private bool back;
        [SerializeField] private bool backLeft;
        [SerializeField] private bool left;
        [SerializeField] private bool frontLeft;

        public override TargetSet GetTargets(DirectionContext userDirection)
        {
            List<OverlayTile> foundTiles = new();

            List<TileDir> directionsToCheck = getDirectionsToCheck();

            // The map origin & direction of
            // THIS PATTERN
            DirectionContext patternDirectionInfo = GetPatternDirection(userDirection);

            /// <summary>
            /// The set of adjacent positions
            /// RELATIVE TO THE PATTERN ORIGIN
            /// </summary>
            AdjacentPositionSet adjacent = new(patternDirectionInfo);

            /// Check the position at each direction in the pattern.
            foreach (TileDir direction in directionsToCheck)
            {
                Vector2Int checkedPosition;

                checkedPosition =
                    adjacent.AdjacentBoardPositions[direction]
                    + (adjacent.BoardDirectionVectors[direction] * (radius));

                if (MapManager.MGR.TryGetTile(
                    checkedPosition,
                    out OverlayTile checkedTile))
                {
                    foundTiles.Add(checkedTile);
                }
            }

            return new(foundTiles);
        }

        /// <summary>
        /// Returns a list of TileDirs corresponding
        /// to the values marked as true for this
        /// pattern in the inspector.
        /// </summary>
        /// <returns></returns>
        private List<TileDir> getDirectionsToCheck()
        {
            /// A list of TileDirections
            List<TileDir> result = new List<TileDir>();

            /// An array of the bools set in the inspector
            bool[] checkDirections =
            {
                front,
                frontRight,
                right,
                backRight,
                back,
                backLeft,
                left,
                frontLeft,
            };

            /// Add the moveDirection of every `true` to the result List
            for (int i = 0; i < checkDirections.Length; i++)
            {
                if (checkDirections[i])
                {
                    result.Add((TileDir)i);
                }
            }

            return result;
        }
    }
}

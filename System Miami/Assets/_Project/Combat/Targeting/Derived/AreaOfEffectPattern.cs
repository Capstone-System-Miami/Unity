// Authors: Layla, Lee
using System.Collections.Generic;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [CreateAssetMenu(
        fileName = "New AOE Pattern",
        menuName = "Combat Subaction/Targeting Patterns/Area of Effect")]
    public class AreaOfEffectPattern : TargetingPattern
    {
        [Tooltip("Radius of the pattern, in Tiles.")]
        [SerializeField] private int _tileRadius;

        [Tooltip("Whether this pattern should affect the tile at its origin (user or mouse)")]
        [SerializeField] private bool _afftectsCenter;

        [Header("Directions")]
        [SerializeField] private bool _front;
        [SerializeField] private bool _frontRight;
        [SerializeField] private bool _right;
        [SerializeField] private bool _backRight;
        [SerializeField] private bool _back;
        [SerializeField] private bool _backLeft;
        [SerializeField] private bool _left;
        [SerializeField] private bool _frontLeft;



        public override TargetSet GetTargets(DirectionContext userDirection)
        {
            List<OverlayTile> foundTiles = new();

            List<TileDir> directionsToCheck = getDirectionsToCheck();

            // The map origin & moveDirection of
            // THIS PATTERN
            DirectionContext patternDirectionInfo = GetPatternDirection(userDirection);

            /// <summary>
            /// The set of adjacent positions
            /// RELATIVE TO THE PATTERN ORIGIN
            /// </summary>
            AdjacentPositionSet adjacent = new(patternDirectionInfo);

            // For each radial in the radius
            for (int radial = 0; radial < _tileRadius; radial++)
            {
                if (radial == 0 && _afftectsCenter)
                {
                    Vector2Int checkedPosition;
                    OverlayTile checkedTile;

                    /// Check the pattern's origin
                    checkedPosition = patternDirectionInfo.TilePositionA;

                    if (MapManager.MGR.TryGetTile(checkedPosition, out checkedTile))
                    {
                        foundTiles.Add(checkedTile);
                    }
                    continue;
                }

                /// Check the position at each direction in the pattern.
                foreach (TileDir direction in directionsToCheck)
                {
                    Vector2Int checkedPosition;

                    checkedPosition =
                        adjacent.AdjacentBoardPositions[direction]
                        + (adjacent.BoardDirectionVectors[direction] * (radial));

                    if (MapManager.MGR.TryGetTile(
                        checkedPosition,
                        out OverlayTile checkedTile))
                    {
                        foundTiles.Add(checkedTile);
                    }
                }
            }

            return new (foundTiles);
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
                _front,
                _frontRight,
                _right,
                _backRight,
                _back,
                _backLeft,
                _left,
                _frontLeft,
            };

            /// Add the moveDirection of every `true` to the result List
            for (int i = 0; i < checkDirections.Length; i++)
            {
                if (checkDirections[i])
                {
                    result.Add((TileDir)i);
                }
            }

            //string report = "";
            //foreach (TileDir dir in result)
            //{
            //    report += $"Check {dir}\n";
            //}
            //Debug.Log(report);

            return result;
        }
    }
}

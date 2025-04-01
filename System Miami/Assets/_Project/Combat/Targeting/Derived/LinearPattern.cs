using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami
{
    [CreateAssetMenu(
        fileName = "New Linear Pattern",
        menuName = "Combat Subaction/Targeting Patterns/Linear")]
    public class LinearPattern : TargetingPattern
    {
        [Tooltip("Distance of the line to check the last tile of, in Tile units.")]
        [SerializeField] private int distance;

        [Header("Directions")]
        [SerializeField] private TileDir direction;

        public override TargetSet GetTargets(DirectionContext userDirection)
        {
            List<OverlayTile> foundTiles = new();

            // The map origin & direction of
            // THIS PATTERN
            DirectionContext patternDirectionInfo = GetPatternDirection(userDirection);

            AdjacentPositionSet adjacent = new(patternDirectionInfo);

            Vector2Int checkedPosition;

            checkedPosition =
                adjacent.AdjacentBoardPositions[direction]
                + (adjacent.BoardDirectionVectors[direction] * (distance));

            if (MapManager.MGR.TryGetTile(
                checkedPosition,
                out OverlayTile checkedTile))
            {
                foundTiles.Add(checkedTile);
            }

            return new(foundTiles);
        }
    }
}

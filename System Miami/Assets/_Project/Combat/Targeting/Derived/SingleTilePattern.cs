using System.Collections.Generic;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [CreateAssetMenu(
        fileName = "New Single Tile Pattern",
        menuName = "Combat Subaction/Targeting Patterns/Single-Tile")]
    public class SingleTilePattern : TargetingPattern
    {
        public override TargetSet GetTargets(DirectionContext userDirection)
        {
            List<OverlayTile> foundTiles = new();

            DirectionContext patternDirectionInfo = GetPatternDirection(userDirection);

            /// Check the pattern's origin
            Vector2Int checkedPosition = checkedPosition = patternDirectionInfo.TilePositionA;

            if (!MapManager.MGR.TryGetTile(
                checkedPosition,
                out OverlayTile checkedTile))
            {
                return new(foundTiles);
            }

            foundTiles.Add(checkedTile);
            return new(foundTiles);
        }
    }
}

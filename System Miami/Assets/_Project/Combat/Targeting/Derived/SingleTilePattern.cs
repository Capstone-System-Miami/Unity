using System.Collections.Generic;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [CreateAssetMenu(fileName = "New Single Tile Pattern", menuName = "Abilities/Targeting Pattern/Single-Tile")]
    public class SingleTilePattern : TargetingPattern
    {
        [Tooltip("The amount of targets that can be single-selected for this pattern's CombatAction")]
        [SerializeField] private int _maxTargets;
        private int _currentTargetCount;

        public int MaxTargets { get { return _maxTargets; } }

        public override Targets GetTargets(DirectionContext userDirection)
        {
            List<OverlayTile> foundTiles = new();

            // The map origin & moveDirection of
            // THIS PATTERN
            DirectionContext patternDirectionInfo = getPatternDirection(userDirection);

            // Check the pattern's origin
            Vector2Int checkedPosition = checkedPosition = patternDirectionInfo.TilePositionA;
            OverlayTile checkedTile;

            if (!MapManager.MGR.TryGetTile(checkedPosition, out checkedTile))
            {
                return new(foundTiles);
            }

            foundTiles.Add(checkedTile);
            return new(foundTiles);
        }
    }
}

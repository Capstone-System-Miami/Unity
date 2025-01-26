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

        public override void SetTargets(DirectionContext userInfo)
        {
            List<Vector2Int> checkedPositions = new List<Vector2Int>();
            List<OverlayTile> foundTiles = new List<OverlayTile>();
            List<Combatant> foundCombatants = new List<Combatant>();

            // The map origin & moveDirection of
            // THIS PATTERN
            DirectionContext patternDirectionInfo = getPatternDirection(userInfo);

            Vector2Int checkedPosition;
            OverlayTile checkedTile;
            Combatant checkedEnemy;

            // Check the pattern's origin
            checkedPosition = patternDirectionInfo.TilePositionA;

            checkedPositions.Add(checkedPosition);

            tryGetTile(checkedPosition, out checkedTile, out checkedEnemy);
            if (checkedTile != null) { foundTiles.Add(checkedTile); }
            if (checkedEnemy != null) { foundCombatants.Add(checkedEnemy); }

            StoredTargets = new Targets(checkedPositions, foundTiles, foundCombatants);
        }
    }
}

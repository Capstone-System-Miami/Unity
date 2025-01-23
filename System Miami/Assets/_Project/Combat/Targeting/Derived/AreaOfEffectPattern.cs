// Authors: Layla, Lee
using System.Collections.Generic;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [CreateAssetMenu(fileName = "New AOE Pattern", menuName = "Abilities/Targeting Pattern/Area of Effect")]
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

        /// <summary>
        /// The set of adjacent positions
        /// RELATIVE TO THE PATTERN ORIGIN
        /// </summary>
        private AdjacentPositionSet _adjacent;

        /// <summary>
        /// A list of TileDirs corresponding
        /// to the values marked as true for this
        /// pattern in the inspector.
        /// </summary>
        private List<TileDir> _directionsToCheck;


        public override void SetTargets(DirectionContext userInfo)
        {
            List<Vector2Int> checkedPositions = new List<Vector2Int>();
            List<OverlayTile> foundTiles = new List<OverlayTile>();
            List<Combatant> foundCombatants = new List<Combatant>();

            // The map origin & moveDirection of
            // THIS PATTERN
            DirectionContext patternDirectionInfo = getPatternDirection(userInfo);

            _directionsToCheck = getDirectionsToCheck();

            _adjacent = new AdjacentPositionSet(patternDirectionInfo);

            // For each radial in the radius
            for (int radial = 0; radial < _tileRadius; radial++)
            {
                if (radial == 0 && _afftectsCenter)
                {
                    Vector2Int checkedPosition;
                    OverlayTile checkedTile;
                    Combatant checkedEnemy;

                    // Check the pattern's origin
                    checkedPosition = patternDirectionInfo.MapPositionA;

                    checkedPositions.Add(checkedPosition);

                    tryGetTile(checkedPosition, out checkedTile, out checkedEnemy);
                    if (checkedTile != null) { foundTiles.Add(checkedTile); }
                    if (checkedEnemy != null) { foundCombatants.Add(checkedEnemy); }
                }

                // Check the position at each moveDirection in the pattern.
                foreach (TileDir direction in _directionsToCheck)
                {
                    if (radial == 1) Debug.Log($"{direction}");
                    Vector2Int checkedPosition;
                    OverlayTile checkedTile;
                    Combatant checkedEnemy;

                    checkedPosition = _adjacent.AdjacentPositions[direction] +
                        _adjacent.AdjacentDirectionVecs[direction] * (radial);

                    checkedPositions.Add(checkedPosition);

                    tryGetTile(checkedPosition, out checkedTile, out checkedEnemy);
                    if (checkedTile != null) { foundTiles.Add(checkedTile); }
                    if (checkedEnemy != null) { foundCombatants.Add(checkedEnemy); }
                }
            }
            
            StoredTargets = new Targets(checkedPositions, foundTiles, foundCombatants);
        }

        /// <summary>
        /// Returns a list of TileDirs corresponding
        /// to the values marked as true for this
        /// pattern in the inspector.
        /// </summary>
        /// <returns></returns>
        private List<TileDir> getDirectionsToCheck()
        {
            // A list of TileDirections
            List<TileDir> result = new List<TileDir>();

            // An array of the bools set in the inspector
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

            // Add the moveDirection of every `true` to the result List
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

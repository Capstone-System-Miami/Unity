// Authors: Layla Hoey
using SystemMiami.AbilitySystem;
using UnityEngine;
using System.Collections.Generic;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using UnityEditor.PackageManager;
using SystemMiami.Outdated;
using Unity.VisualScripting.FullSerializer;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    /// An abstract class
    /// (Meaning an Object can't be ONLY a CombatAction.
    /// If an Object is of the class CombatAction,
    /// it has to be an Object of a class that inherits from CombatAction)
    public abstract class CombatAction : ScriptableObject
    {
        /// <summary>
        /// Whether or not the action will afftect the Combatant performing the action.
        /// </summary>
        [SerializeField] private bool _affectsSelf;

        [Tooltip("Directions and distance this action will check for targets")]
        [SerializeField] private TargetingPattern _targetingPattern;

        public Color TargetedTileColor = Color.white;
        public Color TargetedCombatantColor = Color.white;

        public bool AffectsSelf { get { return _affectsSelf; } }
        public TargetingPattern TargetingPattern { get { return _targetingPattern; } }

        public Targets StoredTargets;


        /// <summary>
        /// Takes directional info about the user,
        /// and uses it as the basis for creating
        /// a new set of DirectionalInfo about
        /// the targetting pattern specific to
        /// this CombatAction.
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private DirectionalInfo getPatternDirection(DirectionalInfo userInfo)
        {
            // The coordinates (on the Board) to start checking tiles from
            Vector2Int patternOrigin;

            // The coordinates (on the board) that
            // the Pattern considers to be
            // its own local "forward".
            // The Pattern's definition of "forward" might be
            // the Board's definition of "right"
            Vector2Int patternForward;

            // If action is a AOE
            if (_targetingPattern.Type == PatternType.SELF)
            {
                patternOrigin = userInfo.MapPosition;
                patternForward = userInfo.MapForward;
            }
            else
            {
                // If this is player
                // If the mouse boardPosition is less than the range (how will we do this?)
                // Pattern origin = mouse boardPosition
                // If this is the enemy
                // If the player boardPosition is less than the range (how?)
                // Pattern origin = player boardPosition

                // Return a zero'd direction info
                // if AOE targeting failed?
                patternOrigin = userInfo.B;
                patternForward = userInfo.B + userInfo.DirectionVec;
            }

            DirectionalInfo patternInfo = new DirectionalInfo(patternOrigin, patternForward);

          //  DirectionHelper.Print(userInfo, "user");
           // DirectionHelper.Print(patternInfo, "pattern");

            return patternInfo;
        }

        private void tryGetTile(Vector2Int position, out OverlayTile tile, out Combatant character)
        {
            if (MapManager.MGR.map.ContainsKey(position))
            {
                tile = MapManager.MGR.map[position];
                character = tile.currentCharacter;
            }
            else
            {
                tile = null;
                character = null;
            }
        }

        public Targets GetUpdatedTargets(DirectionalInfo userInfo)
        {
            List<Vector2Int> boardPositions = new List<Vector2Int>();
            List<OverlayTile> validTilesList = new List<OverlayTile>();
            List<Combatant> targetCombatantsList = new List<Combatant>();

            AdjacentPositionSet patternAdjacentPositions = new AdjacentPositionSet(getPatternDirection(userInfo));

            OverlayTile tile;
            Combatant enemy;

            switch (_targetingPattern.Type)
            {
                default:
                case PatternType.SELF:
                    // check adjacent tiles
                    for (int i = 0; i <= _targetingPattern.Radius; i++)
                    {
                        List<TileDir> patternDirections = _targetingPattern.GetDirections();

                        foreach (TileDir direction in patternDirections)
                        {
                            Vector2Int boardPosition = patternAdjacentPositions.AdjacentPositions[direction] +
                                patternAdjacentPositions.AdjacentDirections[direction] * i;

                            // Add the tile at the action origin's adjacent tiles,
                            // corrected for the direction the character is facing.
                            boardPositions.Add(boardPosition);

                            tryGetTile(boardPosition, out tile, out enemy);

                            if (tile != null) { validTilesList.Add(tile); }

                            if (enemy != null) { targetCombatantsList.Add(enemy); }
                        }
                    }

                    break;

                case PatternType.AREA:
                    // check own tile
                    tryGetTile(userInfo.B, out tile, out enemy);

                    if (tile != null) { validTilesList.Add(tile); }
                    if (enemy != null) { targetCombatantsList.Add(enemy); }

                    // check adjacent tiles
                    for (int i = 0; i <= _targetingPattern.Radius; i++)
                    {
                        List<TileDir> patternDirections = _targetingPattern.GetDirections();

                        foreach (TileDir direction in patternDirections)
                        {
                            Vector2Int boardPosition = patternAdjacentPositions.AdjacentPositions[direction] +
                                patternAdjacentPositions.AdjacentDirections[direction] * i;

                            // Add the tile at the action origin's adjacent tiles,
                            // corrected for the direction the character is facing.
                            boardPositions.Add(boardPosition);

                            tryGetTile(boardPosition, out tile, out enemy);

                            if (tile != null) { validTilesList.Add(tile); }

                            if (enemy != null) { targetCombatantsList.Add(enemy); }
                        }
                    }
                    break;

                case PatternType.MOUSE:
                    // check own tile
                    tryGetTile(userInfo.B, out tile, out enemy);

                    if (tile != null) { validTilesList.Add(tile); }
                    if (enemy != null) { targetCombatantsList.Add(enemy); }
                   

                    break;
            }



            Targets updated = new Targets(boardPositions.ToArray(), validTilesList.ToArray(), targetCombatantsList.ToArray());
            StoredTargets = updated;
            return updated;
            
        }

        public abstract void Perform(Targets targets);
    }
}

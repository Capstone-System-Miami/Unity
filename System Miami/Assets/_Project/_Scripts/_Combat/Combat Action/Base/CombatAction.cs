// Authors: Layla Hoey
using SystemMiami.AbilitySystem;
using UnityEngine;
using System.Collections.Generic;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using UnityEditor.PackageManager;
using SystemMiami.Outdated;

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

        public Color TargetedTileColor;
        public Color TargetedCombatantColor;

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

            // If action is a projectile
            if (this is Projectile projectile)
            {
                // If this is player
                // If the mouse boardPosition is less than the range (how will we do this?)
                // Pattern origin = mouse boardPosition
                // If this is the enemy
                // If the player boardPosition is less than the range (how?)
                // Pattern origin = player boardPosition

                // Return a zero'd direction info
                // if projectile targeting failed?
                patternOrigin = userInfo.MapForward;
                patternForward = userInfo.MapForward + userInfo.DirectionVec;
            }
            else
            {
                patternOrigin = userInfo.MapPosition;
                patternForward = userInfo.DirectionVec;
            }

            return new DirectionalInfo(patternOrigin, patternForward);
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

            for (int i = 0; i <= _targetingPattern.Radius; i++)
            {
                List<TileDir> patternDirections = _targetingPattern.GetDirections();

                foreach (TileDir direction in patternDirections)
                {
                    Vector2Int boardPosition = patternAdjacentPositions.Adjacent[direction] * i;

                    // Add the tile at the action origin's adjacent tiles,
                    // corrected for the direction the character is facing.
                    boardPositions.Add(boardPosition);

                    tryGetTile(boardPosition, out OverlayTile tile, out Combatant enemy);

                    if (tile != null) { validTilesList.Add(tile); }

                    if (enemy != null) { targetCombatantsList.Add(enemy); }
                }
            }

            Targets updated = new Targets(boardPositions.ToArray(), validTilesList.ToArray(), targetCombatantsList.ToArray());
            StoredTargets = updated;
            return updated;
        }

        public abstract void Perform(Targets targets);
    }
}

// Authors: Layla Hoey
using System.Collections.Generic;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public class Targeting
    {
        private Combatant _combatant;
        private CombatAction _action;

        private DirectionalInfo _directionalInfo;
        private AdjacentPositionSet _adjacentPositions;

        public Targeting(Combatant user, CombatAction action)
        {
            _combatant = user;
            _action = action;
        }

        private void setTargetPositions()
        {

        }

        private OverlayTile tryGetTile(out Combatant character, Vector2Int position)
        {
            OverlayTile tile = null;

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

            return tile;
        }

        public void GetUpdatedTargets(out OverlayTile[] tiles, out Combatant[] combatants)
        {
            List<Vector2Int> targetPositions = new List<Vector2Int>();
            List<OverlayTile> targetTiles = new List<OverlayTile>();
            List<Combatant> targetCombatants = new List<Combatant>();

            Vector2Int patternOrigin;
            Vector2Int patternForward;

            // If action is a projectile
            if (_action is Projectile projectile)
            {
                // If this is player
                // If the mouse boardPosition is less than the range (how will we do this?)
                // Pattern origin = mouse boardPosition
                // If this is the enemy
                // If the player boardPosition is less than the range (how?)
                // Pattern origin = player boardPosition

                // Return a zero'd direction info
                // if projectile targeting failed?
                patternOrigin = _combatant.DirectionInfo.Forward;
                patternForward = patternOrigin + _combatant.DirectionInfo.Direction;
            }
            else
            {
                patternOrigin = _combatant.DirectionInfo.Position;
                patternForward = _combatant.DirectionInfo.Forward;
            }

            _directionalInfo = new DirectionalInfo(patternOrigin, patternForward);
            _adjacentPositions = new AdjacentPositionSet(_directionalInfo);

            for (int i = 0; i <= _action.TargetingPattern.Radius; i++)
            {
                Debug.Log($"{_action} radius is {_action.TargetingPattern.Radius},\n" +
                    $"so {_combatant.name} is adding tiles at {i}");

                List<TileDir> targetDirections = _action.TargetingPattern.GetDirections();
                foreach (TileDir dir in targetDirections)
                {
                    Vector2Int boardPosition = _adjacentPositions.Adjacent[dir] * i;

                    Debug.Log($"Adding an adjacent tile.\n" +
                        $"Direction {dir}, BoardPosition {boardPosition}");

                    // Add the tile at the action origin's adjacent tiles,
                    // corrected for the direction the character is facing.
                    targetPositions.Add(boardPosition);

                    OverlayTile tile = tryGetTile(out Combatant enemy, boardPosition);

                    if (tile != null) { targetTiles.Add(tile); }
                    if (enemy != null) { targetCombatants.Add(enemy); }
                }
            }

            tiles = targetTiles.ToArray();
            combatants = targetCombatants.ToArray();
        }
    }
}

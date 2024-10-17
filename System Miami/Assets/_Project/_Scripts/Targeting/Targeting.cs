// Authors: Layla Hoey
using System.Collections.Generic;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.Outdated
{
    //public class Targeting
    //{
    //    private Combatant _user;
    //    private CombatAction _action;

    //    private DirectionalInfo _directionalInfo;
    //    private AdjacentPositionSet _adjacentPositions;

    //    public Targeting(Combatant user, CombatAction action)
    //    {
    //        _user = user;
    //        _action = action;
    //    }

    //    public Targeting(DirectionalInfo userInfo, bool projectile)
    //    {

    //    }



    //    public Targets GetUpdatedTargets(DirectionalInfo userInfo)
    //    {
    //        List<Vector2Int> boardPositions = new List<Vector2Int>();
    //        List<OverlayTile> validTilesList = new List<OverlayTile>();
    //        List<Combatant> targetCombatantsList = new List<Combatant>();

    //        // The coordinates (on the Board) to start checking tiles from
    //        Vector2Int patternOrigin;

    //        // The coordinates (on the board) that
    //        // the Pattern considers to be
    //        // its own local "forward".
    //        // The Pattern's definition of "forward" might be
    //        // the Board's definition of "right"
    //        Vector2Int patternForward;

    //        // If action is a projectile
    //        if (_action is Projectile projectile)
    //        {
    //            // If this is player
    //            // If the mouse boardPosition is less than the range (how will we do this?)
    //            // Pattern origin = mouse boardPosition
    //            // If this is the enemy
    //            // If the player boardPosition is less than the range (how?)
    //            // Pattern origin = player boardPosition

    //            // Return a zero'd direction info
    //            // if projectile targeting failed?
    //            patternOrigin = _user.DirectionInfo.MapForward;
    //            patternForward = patternOrigin + _user.DirectionInfo.DirectionVec;
    //        }
    //        else
    //        {
    //            patternOrigin = _user.DirectionInfo.MapPosition;
    //            patternForward = _user.DirectionInfo.MapForward;
    //        }

    //        _directionalInfo = new DirectionalInfo(patternOrigin, patternForward);
    //        _adjacentPositions = new AdjacentPositionSet(_directionalInfo);

    //        for (int i = 0; i <= _action.TargetingPattern.Radius; i++)
    //        {
    //            Debug.Log($"{_action} radius is {_action.TargetingPattern.Radius},\n" +
    //                $"so {_user.name} is adding tiles at {i}");

    //            List<TileDir> targetDirections = _action.TargetingPattern.GetDirections();
    //            foreach (TileDir dir in targetDirections)
    //            {
    //                Vector2Int boardPosition = _adjacentPositions.Adjacent[dir] * i;

    //                Debug.Log($"Adding an adjacent tile.\n" +
    //                    $"Direction {dir}, BoardPosition {boardPosition}");

    //                // Add the tile at the action origin's adjacent tiles,
    //                // corrected for the direction the character is facing.
    //                boardPositions.Add(boardPosition);

    //                tryGetTile(boardPosition, out OverlayTile tile, out Combatant enemy);

    //                if (tile != null) { validTilesList.Add(tile); }

    //                if (enemy != null) { targetCombatantsList.Add(enemy); }
    //            }
    //        }

    //        tiles = validTilesList.ToArray();
    //        combatants = targetCombatantsList.ToArray();
    //    }
    //}
}

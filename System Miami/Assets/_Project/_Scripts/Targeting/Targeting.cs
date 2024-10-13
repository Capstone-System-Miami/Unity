// Authors: Layla Hoey
using System.Collections.Generic;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SystemMiami.CombatSystem
{
    // TODO
    // A component for determining which combatants will be
    // affected by the currently selected ability
    [RequireComponent(typeof(Combatant))]
    public class Targeting : MonoBehaviour
    {
        [SerializeField] Tilemap fakeGameBoard;
        [SerializeField] GameObject[] fakePlayers;

        private List<ITargetable> _targets = new List<ITargetable>();

        private Vector2Int getPlayerTilePos()
        {
            // TODO
            // get the tile position of the player on the game board
            return GetComponent<Combatant>().fakePlayerPos;
        }

        private Vector2Int getTileUnderMouse()
        {
            // TODO
            // return the game board position of the
            // tile under the mouse
            Vector3Int tilePos = Coordinates.ScreenToIso(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0);
            return new Vector2Int(tilePos.x, tilePos.y);
        }

        private List<Vector2Int> getTilesToCheck(TargetingPattern pattern)
        {
            List<Vector2Int> tilesToCheck = new List<Vector2Int>();

            Vector2Int patternOrigin;
            Vector2Int patternForward;

            switch (pattern.Origin)
            {
                default:
                case TargetOrigin.SELF:
                    patternOrigin = getPlayerTilePos();
                    patternForward = getTileUnderMouse();
                    break;
                case TargetOrigin.MOUSE:
                    patternOrigin = getTileUnderMouse();
                    patternForward = DirectionHelper.BoardDirections[TileDir.FORWARD_C];
                    break;
            }

            DirectionalInfo patternDirInfo = new DirectionalInfo(patternOrigin, patternForward);

            AdjacentPositionSet adjacent = new AdjacentPositionSet(patternDirInfo);

            for(int i = 0; i < pattern.Radius; i++)
            {
                print($"{name} radius is {pattern.Radius} so checking rad at {i}");

                foreach (TileDir dir in pattern.Directions.Keys)
                {
                    print($"{name} checking {dir} at {i}");

                    if (pattern.Directions[dir])
                    {
                        print($"Adding an adjacent tile.\n" +
                            $"Direction {dir}, BoardPosition {adjacent.Adjacent[dir] * i}");
                        tilesToCheck.Add(adjacent.Adjacent[dir] * i);
                    }
                }
            }

            return tilesToCheck;
        }

        /// <summary>
        /// TODO:
        /// Checks the specified tiles, and returns a list
        /// of ITargetable objects.
        /// </summary>
        /// <param name="pattern">
        /// Specifies the pattern to use when checking the tiles.
        /// </param>
        public List<ITargetable> GetTargets(TargetingPattern pattern)
        {
            print($"{name} getting pos");
            List<ITargetable> targets = new List<ITargetable>();

            foreach(Vector2Int pos in getTilesToCheck(pattern))
            {
                foreach(GameObject fakePlayer in fakePlayers)
                {
                    if (fakePlayer.TryGetComponent(out ITargetable target))
                    {
                        if (target.GameObject().GetComponent<Combatant>().fakePlayerPos == pos)
                        {
                            targets.Add(target);
                        }
                    }
                    else
                    {
                        print($"No player found at {pos}");
                    }
                }
            }
            return targets;
        }
    }
}

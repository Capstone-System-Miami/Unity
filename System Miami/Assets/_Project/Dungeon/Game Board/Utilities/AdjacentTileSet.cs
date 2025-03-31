using System.Collections.Generic;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public class AdjacentTileSet : AdjacentPositionSet
    {
        /// <summary>
        /// A dictionary of adjacent <c><see cref="OverlayTile"/></c> accessible
        /// by <c><see cref="TileDir"/></c>
        /// </summary>
        /// <remarks>
        /// NOTE: MAY CONTAIN NULL TILES. These can be used to
        /// detect the edges of the Game Board.</remarks>
        public readonly Dictionary<TileDir, OverlayTile> AdjacentTiles = new();

        /// <summary>
        /// A modified version of <c><see cref="AdjacentTiles"/></c> which
        /// excludes <c><see cref="OverlayTile"/></c> entries at
        /// <c><see cref="TileDir"/></c> Keys representing diagonal directions.
        /// </summary>
        public readonly Dictionary<TileDir, OverlayTile> ExcludeDiagonals = new();

        public AdjacentTileSet(OverlayTile tile)
            : this ( new DirectionContext(tile.BoardPos) )
        { }

        public AdjacentTileSet(DirectionContext info) : base(info)
        {
            foreach (TileDir direction in AdjacentBoardPositions.Keys)
            {
                // NOTE: not executing this in an if statement.
                // Intentionally not catching null OverlayTile output.
                MapManager.MGR.TryGetTile(AdjacentBoardPositions[direction], out OverlayTile tile);

                AdjacentTiles[direction] = tile;

                // remember that board forward is FORWARD_R
                if (direction == TileDir.FORWARD_R
                    || direction == TileDir.BACKWARD_R
                    || direction == TileDir.BACKWARD_L
                    || direction == TileDir.FORWARD_L)
                {
                    ExcludeDiagonals[direction] = tile;
                }
            }
        }
    }
}

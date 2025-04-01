using System.Collections.Generic;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using Unity.VisualScripting;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public enum AdjacencyType { EDGE, CORNER, BOTH };
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
        public readonly Dictionary<TileDir, OverlayTile> EdgeAdjacent = new();

        /// <summary>
        /// A modified version of <c><see cref="AdjacentTiles"/></c> which
        /// includes ONLY <c><see cref="OverlayTile"/></c> entries at
        /// <c><see cref="TileDir"/></c> Keys representing diagonal directions.
        /// </summary>
        public readonly Dictionary<TileDir, OverlayTile> CornerAdjacent = new();

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

                if (direction == TileDir.FORWARD_C
                    || direction == TileDir.MIDDLE_R
                    || direction == TileDir.BACKWARD_C
                    || direction == TileDir.MIDDLE_L)
                {
                    EdgeAdjacent[direction] = tile;
                }

                if (direction == TileDir.FORWARD_R
                    || direction == TileDir.BACKWARD_R
                    || direction == TileDir.BACKWARD_L
                    || direction == TileDir.FORWARD_L)
                {
                    CornerAdjacent[direction] = tile;
                }
            }
        }

        public Dictionary<TileDir, OverlayTile> GetAdjacent(AdjacencyType type)
        {
            return type switch {
                AdjacencyType.EDGE      => EdgeAdjacent,
                AdjacencyType.CORNER    => CornerAdjacent,
                AdjacencyType.BOTH      => AdjacentTiles,
                _                       => AdjacentTiles
            };
        }

        public void UnhighlightAll()
        {
            foreach (TileDir direction in AdjacentTiles.Keys)
            {
                if (AdjacentTiles[direction] == null) { continue; }

                AdjacentTiles[direction].UnHighlight();
            }
        }

        public void HighlightAll()
        {
            HighlightCenter(Color.black);
            HighlightEdges(Color.blue);
            HighlightCorners(Color.magenta);
        }

        public void HighlightCenter(Color color)
        {
            if (MapManager.MGR.TryGetTile(center, out OverlayTile tile))
            {
                tile.Highlight(color);
            }
        }
        public void HighlightCorners(Color color)
        {
            string dbugmsg = $"<color=cyan>this is intended to be the corners of {center}</color>\n";
            foreach (TileDir key in CornerAdjacent.Keys)
            {
                if (CornerAdjacent[key] == null) { continue; }

                dbugmsg += $"<color=magenta>| {key}:  {CornerAdjacent[key].name}</color>\n";
                CornerAdjacent[key].Highlight(color);
            }

            Debug.Log(dbugmsg, MapManager.MGR.map[center]);
        }

        public void HighlightEdges(Color color)
        {
            string dbugmsg = $"<color=green>this is intended to be the corners of {center}</color>\n";
            foreach (TileDir key in EdgeAdjacent.Keys)
            {
                if (EdgeAdjacent[key] == null) { continue; }

                dbugmsg += $"<color=blue>| {key}:  {EdgeAdjacent[key].name}</color>\n";
                EdgeAdjacent[key].Highlight(color);
            }

            Debug.Log(dbugmsg, MapManager.MGR.map[center]);
        }
    }
}

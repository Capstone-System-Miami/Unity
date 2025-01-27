// Author: Layla
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SystemMiami
{
    public class MovementPath
    {
        private TileContext tileContext;

        private PathFinder pathFinder;

        private int maxTiles;
        private List<OverlayTile> path;
        private List<OverlayTile> startExclusive;
        private List<OverlayTile> startInclusive;

        private int toRemove;
        private List<OverlayTile> removed;

        public List<OverlayTile> RawPath => new(path);

        public List<OverlayTile> ForMovement
        {
            get
            {
                if (startExclusive == null)
                {
                    startExclusive = GetTruncated(false);
                }
                return startExclusive;
            }
        }

        public List<OverlayTile> ForDrawing
        {
            get
            {
                if (startInclusive == null)
                {
                    startInclusive = GetTruncated(true);
                }
                return startInclusive;
            }
        }

        public List<OverlayTile> Removed
        {
            get
            {
                if (removed == null)
                {
                    removed = GetRemoved();
                }
                return removed;
            }
        }

        public bool IsEmpty => !ForMovement.Any();

        public MovementPath(
            OverlayTile start,
            OverlayTile end,
            int maxTiles)
                : this(
                    new TileContext(start, end),
                    maxTiles
                )
        { }

        public MovementPath(
            OverlayTile start,
            OverlayTile end)
                : this( new TileContext(start, end), -1)
        { }

        public MovementPath(TileContext tileContext, int maxTiles)
        {
            this.tileContext = tileContext;
            this.pathFinder = new();
            this.maxTiles = maxTiles;
            this.path = pathFinder.FindPath(this.tileContext);

            toRemove = path.Count - maxTiles;
        }

        public void DrawAll(
            Color withinRange,
            Color outsideRange)
        {
            DrawArrows();
            HighlightValidMoves(Color.yellow);
            HighlightInvalidMoves(Color.red);
        }

        public void UnDrawAll()
        {
            UnDrawArrows();
            Unhighlight();
        }

        public void DrawArrows()
        {
            ArrowDrawer.MGR.DrawPath(ForDrawing);
        }

        public void UnDrawArrows()
        {
            ArrowDrawer.MGR.RemoveArrows();
        }

        public void HighlightValidMoves(Color color)
        {
            foreach(OverlayTile tile in ForDrawing)
            {
                tile.Highlight(color);
            }
        }

        public void UnhighlightValidMoves()
        {
            Unhighlight(ForDrawing);
        }


        public void HighlightInvalidMoves(Color color)
        {
            foreach(OverlayTile tile in Removed)
            {
                tile.Highlight(color);
            }
        }

        public void UnhighlightInvalidMoves()
        {
            Unhighlight(Removed);
        }

        public void Unhighlight()
        {
            Unhighlight(RawPath);
            Unhighlight(ForMovement);
            Unhighlight(ForDrawing);
        }

        private void Unhighlight(List<OverlayTile> tiles)
        {
            foreach (OverlayTile tile in tiles)
            {
                tile.UnHighlight();
            }
        }

        public void DebugHighlight(List<OverlayTile> tiles)
        {
            Color[] colors =
            {
                Color.blue,
                Color.cyan,
                Color.magenta,
                Color.red,
                Color.yellow,
            };
            int i = 0;


            foreach(OverlayTile tile in tiles)
            {
                i++;
                tile?.Highlight(colors[i % 5]);
            }
        }


        /// <summary>
        /// Calculate a path to the tile arg.
        /// Path stops at either:
        /// 
        /// <para> a) the Destination tile
        ///         of the context arg </para>
        /// 
        /// <para> b) the furthest tile in
        ///         the path before reaching
        ///         the max number of tiles </para>
        /// </summary>
        /// 
        /// <param name="tiles">
        /// The tile context, including
        /// current and destination
        /// </param>
        /// <param name="maxTiles">
        /// The max size of the path
        /// to return.
        /// </param>
        private List<OverlayTile> GetTruncated(bool includeStart)
        {
            if (maxTiles < 0) { return new(path); }

            List<OverlayTile> result = new(path);

            if (toRemove > 0)
            {
                result.RemoveRange(maxTiles, toRemove);
            }

            // If the Destination tile is blocked
            // (e.g. bc theres a charac there),
            // remove it from the end.
            if (result.Count > 0 && !result[result.Count - 1].ValidForPlacement)
            {
                result.RemoveAt(result.Count - 1);
            }

            // Add tileContext.Current
            // tile if necessary
            result = includeStart ? GetStartInclusive(result) : result;

            // return an empty list if null
            return result ?? new();
        }

        private List<OverlayTile> GetRemoved()
        {
            if (maxTiles < 0) { return new(); }
            if (toRemove <= 0) { return new(); }

            List<OverlayTile> result = new(path);

            return result.GetRange(maxTiles, toRemove);
        }


        /// <summary>
        /// Get a path that includes the context's
        /// current tile.
        /// 
        /// <para>
        /// We can use this to draw arrows,
        /// or anything else we might need.</para>
        /// </summary>
        /// 
        /// <param name="path">
        /// The list of tiles to get a
        /// modified copy of.
        /// <para>
        /// (Can be truncated or not)</para>
        /// </param>
        private List<OverlayTile> GetStartInclusive(List<OverlayTile> path)
        {
            List<OverlayTile> result = new(path);

            path.Insert(0, tileContext.Current);

            // return an empty list if null
            return path ?? new();
        }
    }
}

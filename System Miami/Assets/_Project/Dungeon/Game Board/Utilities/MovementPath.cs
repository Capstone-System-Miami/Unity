// Author: Layla
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SystemMiami.Utilities;

namespace SystemMiami
{
    public class MovementPath
    {
        private OverlayTile start;
        private OverlayTile end;

        private PathFinder pathFinder;

        private int maxTiles;
        private List<OverlayTile> path;
        private List<OverlayTile> startExclusive;
        private List<OverlayTile> startInclusive;

        private int toRemove;
        private List<OverlayTile> removed;

        /// <summary>
        /// Returns a list of the valid movement tiles along the
        /// path to the target.
        /// <para>
        /// NOTE This is a "Lazy Loader"</para>
        /// </summary>
        public List<OverlayTile> ForMovement
        {
            get
            {
                string dbugMsg = $"<color=yellow>Reading (& Loading) \'ForMovement\'</color>\n";
                if (startInclusive != null)
                {
                    dbugMsg.Replace("(& Loading) ", "");
                }
                startExclusive ??= GetTruncated(false);

                dbugMsg += $"<color=red>{string.Join(" -> ", startExclusive.Select(tile => tile.name).ToArray())}</color>";
                Debug.Log(dbugMsg);
                return startExclusive;
            }
        }

        /// <summary>
        /// Returns a list of the valid movement tiles along the
        /// path to the target, INCLUDING the player's position.
        /// <para>
        /// NOTE This is a "Lazy Loader"</para>
        /// </summary>
        public List<OverlayTile> ForDrawingValid
        {
            get
            {
                startInclusive ??= GetTruncated(true);
                return startInclusive;
            }
        }

        /// <summary>
        /// Returns a list of the *invalid* movement tiles along the
        /// path to the target.
        /// <para>
        /// NOTE This is a "Lazy Loader"</para>
        /// </summary>
        public List<OverlayTile> ForDrawingInvalid
        {
            get
            {
                removed ??= GetRemoved();
                return removed;
            }
        }

        public bool ContainsValidMoves => ForMovement.Any();

        public MovementPath(
            OverlayTile start,
            OverlayTile end,
            bool forced)
                : this(start, end, forced, int.MaxValue)
        { }

        public MovementPath(
            OverlayTile start,
            OverlayTile end,
            bool forced,
            int maxTiles)
        {
            this.start = start;
            this.end = end;
            this.pathFinder = new();
            this.maxTiles = maxTiles;

            this.path = forced
                                                         // TODO
                                                         // This diag flag
                                                         // shouldn't need
                                                         // to be false.
                                                         // vvvvv 
                ? pathFinder.FindPath(this.start, this.end, false, true)
                : pathFinder.FindPath(this.start, this.end);

            toRemove = path.Count - maxTiles;
        }

        public void DrawAll(
            Color withinRange,
            Color outsideRange)
        {
            DrawArrows();
            HighlightValidMoves(withinRange);
            HighlightInvalidMoves(outsideRange);
        }

        public void UnDrawAll()
        {
            UnDrawArrows();
            Unhighlight();
        }

        public void DrawArrows()
        {
            ArrowDrawer.MGR.DrawPath(ForDrawingValid);
        }

        public void UnDrawArrows()
        {
            ArrowDrawer.MGR.RemoveArrows();
        }

        public void HighlightValidMoves(Color color)
        {
            foreach(OverlayTile tile in ForDrawingValid)
            {
                tile.Highlight(color);
            }
        }

        public void UnhighlightValidMoves()
        {
            Unhighlight(ForDrawingValid);
        }


        public void HighlightInvalidMoves(Color color)
        {
            foreach(OverlayTile tile in ForDrawingInvalid)
            {
                tile.Highlight(color);
            }
        }

        public void UnhighlightInvalidMoves()
        {
            Unhighlight(ForDrawingInvalid);
        }

        public void Unhighlight()
        {
            Unhighlight(ForDrawingValid);
            Unhighlight(ForDrawingInvalid);
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

            result.Insert(0, start);

            // return an empty list if null
            return result ?? new();
        }
    }
}

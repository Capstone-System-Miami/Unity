using System.Collections.Generic;
using System.Linq;

namespace SystemMiami.CombatRefactor
{
    // waits for input
    public abstract class MovementTileSelection : CombatantState
    {
        int speed;

        TileContext tiles;

        OverlayTile newFocus;
        OverlayTile newDestination;

        // Pathing
        protected PathFinder pathFinder = new PathFinder();
        protected List<OverlayTile> newPath = new();
        protected List<OverlayTile> arrowPath = new();

        protected bool validMove
        {
            get
            {
                return newPath.Any();
            }
        }

        protected MovementTileSelection(CombatantStateMachine machine)
            : base(machine, Phase.Movement) { }

        public override void aOnEnter()
        {
            machine.combatant.ResetTileContext();

            speed = (int)machine.combatant.Speed.Get();
        }


        public override void bUpdate()
        {
            tiles = new TileContext(
                machine.combatant.CurrentTile,
                machine.combatant.FocusTile,
                machine.combatant.DestinationTile);

            // Find a new possible focus tile
            // by the means described
            // by this method in the derived
            // classes.
            newFocus = GetNewFocus();

            // Do not proceed if the possible
            // new focus is already focused
            if (newFocus == machine.combatant.FocusTile) { return; }

            // Set the combatant's new focus tile
            machine.combatant.SetFocusTile(newFocus);

            // Calculate a new possible path
            newPath = getTruncatedPathTo(newFocus);

            // Return if the new path is empty.
            if (!newPath.Any()) { return; }

            // New possible destination is the
            // last one in the truncated path.
            newDestination = newPath.Last();

            // Return if the newDestination is null.

            // Redraw arrows to that every frame.
            arrowPath = getArrowPathTo(newDestination);
        }

        public override void cMakeDecision()
        {
            if (SkipPhase())
            {
                GoToActionSelection();
                return;
            }

            if (!validMove) { return; }

            if (SelectTile())
            {
                GoToTileConfirmation();
                return;
            }
        }

        public override void eOnExit()
        {
        }


        // Decision
        protected abstract bool SkipPhase();
        protected abstract bool SelectTile();


        // Decision outcomes
        protected abstract void GoToActionSelection();
        protected abstract void GoToTileConfirmation();


        #region // Pathing
        /// <summary>
        /// Whatever tile the combatant
        /// is currently focusing on.
        /// The methods for determining this
        /// must be defined in derived classes.
        /// </summary>
        protected abstract OverlayTile GetNewFocus();

        /// <summary>
        /// Returns a path to a tile using the pathfinder.
        /// </summary>
        protected List<OverlayTile> getPathTo(OverlayTile tile)
        {
            return pathFinder.FindPath(machine.combatant.CurrentTile, tile);
        }

        /// <summary>
        /// Calculate a path to the tile arg.
        /// Path stops at either:
        /// 
        /// <para> a) the tile arg </para>
        /// 
        /// <para> b) the furthest tile in
        ///         the path before the combatant's
        ///         speed would prevent them
        ///         from moving further.</para>
        /// </summary>
        /// 
        /// <param name="tile">
        /// The destination tile.
        /// </param>
        private List<OverlayTile> getTruncatedPathTo(OverlayTile tile)
        {
            // Generate a path to modify.
            List<OverlayTile> path = getPathTo(tile);

            // Farthest the combatant can move
            // with their current speed points.
            int truncatedLength = speed;

            // Get the difference
            int tilesToRemove = path.Count - truncatedLength;

            // If there's a valid difference,
            // remove the rest of the tiles
            if (tilesToRemove > 0)
            {
                path.RemoveRange(truncatedLength, tilesToRemove);
            }

            // If the destination is blocked
            // (e.g. bc theres a charac there),
            // remove it from the end.
            if (path.Count > 0 && !path[path.Count - 1].ValidForPlacement)
            {
                path.RemoveAt(path.Count - 1);
            }

            // return an empty list if null
            return path ?? new();
        }

        /// <summary>
        /// Get a path that includes the combatant's
        /// current tile. We can then use this
        /// to draw arrows.
        /// </summary>
        /// 
        /// <param name="tile">
        /// The destination tile.
        /// </param>
        private List<OverlayTile> getArrowPathTo(OverlayTile tile)
        {
            // Create a copy of our truncated path that contains the
            // starting position. We can use this modified list
            // To draw the arrows.
            List<OverlayTile> arrowPath = new(newPath);

            arrowPath.Insert(0, machine.combatant.CurrentTile);

            // return an empty list if null
            return arrowPath ?? new();
        }
        #endregion // ^^^ Pathing ^^^
    }
}

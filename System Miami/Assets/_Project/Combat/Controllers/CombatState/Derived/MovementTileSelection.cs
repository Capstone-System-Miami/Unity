using System.Collections.Generic;
using System.Linq;

namespace SystemMiami.CombatRefactor
{
    // waits for input
    public abstract class MovementTileSelection : CombatantState
    {
        int speed;

        protected List<OverlayTile> truncatedPath = new();
        protected List<OverlayTile> arrowPath = new();

        protected MovementTileSelection(CombatantStateMachine machine)
            : base(machine, Phase.Movement) { }

        public override void aOnEnter()
        {
            machine.combatant.ResetTileFlags();

            speed = (int)machine.combatant.Speed.Get();
        }


        public override void bUpdate()
        {
            OverlayTile newFocus = GetFocusedTile();

            if (newFocus == machine.combatant.FocusTile) { return; }

            machine.combatant.FocusTileChanged(newFocus);

            truncatedPath = getTruncatedPathTo(newFocus);

            // New possible destination is the
            // last one in the truncated path.
            OverlayTile newDestination = truncatedPath.Last();

            // Redraw arrows to that every frame.
            arrowPath = getArrowPathTo(newDestination);
        }

        public override abstract void cMakeDecision();

        public override void eOnExit()
        {
        }

        protected abstract OverlayTile GetFocusedTile();

        protected abstract bool SelectTile();
        protected abstract bool SkipPhase();

        /// <summary>
        /// Returns a path to a tile using the pathfinder.
        /// </summary>
        private List<OverlayTile> getPathTo(OverlayTile tile)
        {
            return machine.PathFinder.FindPath(machine.combatant.CurrentTile, tile);
        }

        /// <summary>
        /// Returns a path to a tile that corresponds to
        /// the combatants current Speed (movement points).
        /// If the combatant's Speed is less than the
        /// tile distance to the destination, it
        /// returns a path that will get them part of the way.
        /// </summary>
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

        private List<OverlayTile> getArrowPathTo(OverlayTile tile)
        {
            // Create a copy of our path that contains the
            // starting position. We can use this modified list
            // To draw the arrows.
            List<OverlayTile> arrowPath = new(machine.CurrentPath);

            arrowPath.Insert(0, machine.combatant.CurrentTile);

            // return an empty list if null
            return arrowPath ?? new();
        }
    }
}

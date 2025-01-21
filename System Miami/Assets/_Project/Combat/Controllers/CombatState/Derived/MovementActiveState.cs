using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class MovementActiveState : CombatState
    {
        public MovementActiveState(CombatStateMachine context)
            : base(context, Phase.Movement) { }

        /// Sets the destination tile.
        /// Validates a path to the destination.
        public override void OnEnter()
        {
            context.DestinationTile = context.FocusedTile;

            // The exact path to the focused tile.
            // If anyone can think of a use for this, it's here.
            List<OverlayTile> unmodifiedPath = getPathTo(context.DestinationTile);

            // The path leading up to the tile where the
            // combatant will be out of Speed (movement points)
            List<OverlayTile> truncatedPath = getTruncatedPathTo(context.DestinationTile);

            if (truncatedPath == null) { return; }

            // New path is the truncated one.
            context.CurrentPath = truncatedPath;

            if (truncatedPath.Count > 0)
            {
                // New Destination Tile is the
                // last one in the truncated path.
                context.DestinationTile = truncatedPath[truncatedPath.Count - 1];


                // Create a copy of our path that contains the
                // starting position. We can use this modified list
                // To draw the arrows.
                List<OverlayTile> inclusivePath = context.CurrentPath;
                inclusivePath.Insert(0, context.combatant.CurrentTile);

                DrawArrows.MGR.DrawPath(inclusivePath);
            }
            else
            {
                context.DestinationTile = context.combatant.CurrentTile;
                Debug.Log($"{context.name} tried to move but cant. " +
                    $"Their calculated path was {truncatedPath.Count} tiles long.");
            }
        }

        public override void OnExit()
        {
            context.FocusedTile?.UnHighlight(); // on phase transit

            TurnManager.MGR.NewTurnPhase(context.CurrentPhase);

            ResetTileData();

            // TODO what is this
            context.Controller.ResetFlags();
        }

        public override void Update()
        {
            if (!destinationReached())
            {
                moveAlongPath();
            }
            else
            {
                if (context.Controller.NextPhaseTriggered())
                {
                    context.SwitchState(context.actionUnequippedState);
                }
            }
        }

        /// <summary>
        /// Returns false if there is
        /// no current destination tile.
        /// Returns true if there is no current path,
        /// or if the combatant's current tile is the same
        /// as the current destination tile.
        /// </summary>
        private bool destinationReached()
        {
            return context.combatant.CurrentTile == context.DestinationTile;
        }

        /// <summary>
        /// Clears destination tile,
        /// current path, and movement bool.
        /// </summary>
        protected void clearMovement()
        {
            context.DestinationTile = context.combatant.CurrentTile;
            context.CurrentPath = null;
        }

        /// <summary>
        /// Moves the character along the calculated path.
        /// </summary>
        private void moveAlongPath()
        {
            //Debug.Log($"{name} calling move along path and tilecount is {currentPath.Count}");

            float step = context.Controller.movementSpeed * Time.deltaTime;

            //TODO
            //i want to add it here so that you have to
            //confirm the movement too so that the arrows
            //show up before you move and can show your path
            OverlayTile targetTile = context.CurrentPath[0];

            context.combatant.transform.position = Vector2.MoveTowards(context.combatant.transform.position, targetTile.transform.position, step);

            // If character is close enough to a new tile
            if (Vector2.Distance(context.combatant.transform.position, targetTile.transform.position) < 0.0001f)
            {
                // Directional info based on the current tile
                // and the one we're moving to.
                DirectionalInfo newDir = new DirectionalInfo((Vector2Int)context.combatant.CurrentTile.GridLocation, (Vector2Int)targetTile.GridLocation);

                // Let any subscribers know that we are moving along path
                context.PathTileChanged(newDir);

                targetTile.PlaceCombatant(context.combatant);
                context.CurrentPath.RemoveAt(0);
            }
        }

        /// <summary>
        /// Returns a path to a tile using the pathfinder.
        /// </summary>
        private List<OverlayTile> getPathTo(OverlayTile tile)
        {
            return context.PathFinder.FindPath(context.combatant.CurrentTile, tile);
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
            // Make a copy of the path to modify.
            List<OverlayTile> path = getPathTo(tile);

            // Farthest the combatant can move
            // with their current speed points.
            int truncatedLength = (int)context.combatant.Speed.Get();

            // Get the difference
            int tilesToRemove = path.Count - truncatedLength;

            // If there's a valid difference,
            // remove the rest of the tiles
            if (tilesToRemove > 0)
            {
                path.RemoveRange(truncatedLength, tilesToRemove);
            }

            // If the destination is blocked (e.g. bc theres a charac there),
            // remove it from the end.
            if (path.Count > 0 && !path[path.Count - 1].ValidForPlacement)
            {
                path.RemoveAt(path.Count - 1);
            }

            return path;
        }
    }
}

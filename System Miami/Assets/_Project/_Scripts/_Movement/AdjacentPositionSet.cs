// Authors: Layla Hoey
using System.Collections.Generic;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    // TODO
    // Not fully tested.
    // Should take the direction the player is facing,
    // gather the actual world tile positions of the
    // 8 surrounding directions, then adjust each one for
    // whatever direction the object is facing.
    public class AdjacentPositionSet
    {
        public Dictionary<TileDir, Vector2Int> Adjacent { get; private set; }

        // Constructors
        public AdjacentPositionSet(DirectionalInfo info)
        {
            // Initialize to right size and values
            // if the player is facing the same way as the board.
            Adjacent = DirectionHelper.BoardDirections;

            if (info.Direction == DirectionHelper.BoardDirections[TileDir.MIDDLE_L])
            {
                rotateLeft();
            }
            else if (info.Direction == DirectionHelper.BoardDirections[TileDir.MIDDLE_R])
            {
                rotateRight();
            }
            else if (info.Direction == DirectionHelper.BoardDirections[TileDir.BACKWARD_C])
            {
                rotate180();
            }
        }

        // Private
        private void rotateRight()
        {
            foreach (TileDir direction in Adjacent.Keys)
            {
                // Swap x and y, multiply new y by -1
                Adjacent[direction] = new Vector2Int(Adjacent[direction].y, -Adjacent[direction].x);
            }
        }

        private void rotateLeft()
        {
            foreach (TileDir direction in Adjacent.Keys)
            {
                // Swap x and y, multiply new x by -1
                Adjacent[direction] = new Vector2Int(-Adjacent[direction].y, Adjacent[direction].x);
            }
        }

        private void rotate180()
        {
            foreach (TileDir direction in Adjacent.Keys)
            {
                // Multiply x and y by -1
                Adjacent[direction] *= -1;
            }
        }
    }
}

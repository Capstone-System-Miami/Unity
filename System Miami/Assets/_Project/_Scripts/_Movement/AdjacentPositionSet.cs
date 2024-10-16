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
            Debug.Log($"HI, trying to create myself to {DirectionHelper.BoardDirections[TileDir.FORWARD_C]}");
            // Initialize to right size and values
            // if the player is facing the same way as the board.
            Adjacent = DirectionHelper.BoardDirections;

            // TODO
            // Right now, this doesn't work for diagonal directions.           
            if (info.Direction == DirectionHelper.BoardDirections[TileDir.MIDDLE_L])
            {
                // If player's Direction is BoardDirections' Left,
                // Rotate each position to the left
                //rotate90Left();
                rotate90Left();
            }
            else if (info.Direction == DirectionHelper.BoardDirections[TileDir.MIDDLE_R])
            {
                // If player's Direction is BoardDirections' Right,
                // Rotate each position to the right
                rotate90Right();
            }
            else if (info.Direction == DirectionHelper.BoardDirections[TileDir.BACKWARD_C])
            {
                // If player's Direction is BoardDirections' Backwards,
                // Rotate each position by 180
                rotate180();
            }
        }

        // Private
        private void rotate90Right()
        {
            Dictionary<TileDir, Vector2Int> hi = DirectionHelper.BoardDirections;

            foreach (TileDir direction in hi.Keys)
            {
                // Swap x and y, multiply new y by -1
                hi[direction] = new Vector2Int(hi[direction].y, -hi[direction].x);
            }
            Adjacent = hi;
        }

        private void rotate90Left()
        {
            Dictionary<TileDir, Vector2Int> hi = DirectionHelper.BoardDirections;

            foreach (TileDir direction in hi.Keys)
            {
                // Swap x and y, multiply new x by -1
                hi[direction] = new Vector2Int(-hi[direction].y, hi[direction].x);
            }
            Adjacent = hi;
        }

        private void rotate180()
        {
            Dictionary<TileDir, Vector2Int> hi = DirectionHelper.BoardDirections;

            foreach (TileDir direction in hi.Keys)
            {
                // Multiply x and y by -1
                hi[direction] *= -1;
            }
        }
    }
}

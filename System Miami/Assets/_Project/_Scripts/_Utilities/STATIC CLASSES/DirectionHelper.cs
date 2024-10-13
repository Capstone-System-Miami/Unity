// Authors: Layla Hoey
using System.Collections.Generic;
using SystemMiami.Enums;
using UnityEngine;

namespace SystemMiami.Utilities
{
    public static class DirectionHelper
    {
        // An unchanging dict of difference vectors for world space
        public static readonly Dictionary<TileDir, Vector2Int> BoardDirections =
            new Dictionary<TileDir, Vector2Int>()
            {
                { TileDir.FORWARD_L,   new Vector2Int(-1,  1)  },
                { TileDir.FORWARD_C,   new Vector2Int( 0,  1)  },
                { TileDir.FORWARD_R,   new Vector2Int( 1,  1)  },
                { TileDir.MIDDLE_L,    new Vector2Int(-1,  0)  },
                { TileDir.MIDDLE_C,    new Vector2Int( 0,  0)  },
                { TileDir.MIDDLE_R,    new Vector2Int( 1,  0)  },
                { TileDir.BACKWARD_L,  new Vector2Int(-1,  -1) },
                { TileDir.BACKWARD_C,  new Vector2Int( 0,  -1) },
                { TileDir.BACKWARD_R,  new Vector2Int( 1,  -1) }
            };

        /// <summary>
        /// Takes two positions, origin and forward, 
        /// and returns a difference vector (only 0s or signed 1s).
        /// </summary>
        public static Vector2Int GetDirection(Vector2Int origin, Vector2Int forward)
        {
            int x = 0;
            int y = 0;

            // In these examples, the origin object is
            // "Looking at" the same position at (-8, 2)
            // ex1. origin = (0, 0), forward = (-8, 2)
            // ex2. origin = (-4, 5), forward = (-8, 2)

            // Convert ints to floats and get the difference.
                // ex1. difference = (-8.0, 2.0)
                // ex2. difference = (-4.0, -3.0)
            Vector2 difference = forward - origin;

            // Force a magnitude of 1.
                // ex1. difference = (-1.0, 0.25)
                // ex2. difference = (-1.0, -0.75)
            difference.Normalize();

            // Round both values to int
                // ex1. x = -1, y = 0
                // ex2. x = -1, y = -1
            x = Mathf.RoundToInt(difference.x);
            y = Mathf.RoundToInt(difference.y);

            // Return the direction vector.
                // ex1. Returns (-1, 0), a vector
                // equivalent to BoardDirections[MIDDLE_L]
                // ex2. Returns (-1, -1), a vector
                // euqivalent to BoardDirections[BACKWARD_L]
            return new Vector2Int(x, y);
        }
    }

    #region STRUCTS
    /// <summary>
    /// A struct containing 3 Vector2Ints.
    /// On construction, it stores a difference Vector based
    /// on the position and forward given.
    /// </summary>
    public struct DirectionalInfo
    {
        // Position (on the game board) of a tile
        public Vector2Int Position { get; private set; }

        // Position (on the game board) of whatever
        // we consider to be "forward" from myPos.
        public Vector2Int Forward { get; private set; }

        // The difference the object is facing
        public Vector2Int Direction { get; private set; }

        public DirectionalInfo(Vector2Int position, Vector2Int forward)
        {
            Position = position;
            Forward = forward;
            Direction = DirectionHelper.GetDirection(position, forward);
        }
    }
    #endregion
}

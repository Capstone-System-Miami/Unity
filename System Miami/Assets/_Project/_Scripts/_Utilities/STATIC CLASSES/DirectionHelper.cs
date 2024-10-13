// Authors: Layla Hoey
using System.Collections.Generic;
using SystemMiami.Enums;
using UnityEngine;

namespace SystemMiami.Utilities
{
    public static class DirectionHelper
    {
        // An unchanging dict of direction vectors for world space
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
        /// and returns a direction vector (only 0s or signed 1s).
        /// </summary>
        public static Vector2Int GetDirection(Vector2Int origin, Vector2Int forward)
        {
            int x = 0;
            int y = 0;

            Vector2Int direction = forward - origin;

            // If either value in direction is not zero, divide it by
            // the absolute value of itself to make it 1 or -1.
            // i.e. Say direction.x is -8.
            // Set x to (-8 / 8). So (x = -1)
            x = direction.x == 0 ? 0 : (direction.x / Mathf.Abs(direction.x));
            y = direction.y == 0 ? 0 : (direction.y / Mathf.Abs(direction.y));

            return new Vector2Int(x, y);
        }
    }

    #region STRUCTS
    /// <summary>
    /// A struct containing 3 Vector2Ints.
    /// On construction, it stores a direction Vector based
    /// on the position and forward given.
    /// </summary>
    public struct DirectionalInfo
    {
        // Position (on the game board) of a tile
        public Vector2Int Position { get; private set; }

        // Position (on the game board) of whatever
        // we consider to be "forward" from myPos.
        public Vector2Int Forward { get; private set; }

        // The direction the object is facing
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

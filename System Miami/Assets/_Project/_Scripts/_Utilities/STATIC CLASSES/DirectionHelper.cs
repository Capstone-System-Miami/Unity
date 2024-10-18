// Authors: Layla Hoey
using System.Collections.Generic;
using SystemMiami.Enums;
using UnityEngine;
using UnityEngine.TextCore;

namespace SystemMiami.Utilities
{
    public static class DirectionHelper
    {
        // An unchanging dict of direction vectors, accessable by TileDir
        public static readonly Dictionary<TileDir, Vector2Int> MapDirectionsByEnum =
            new Dictionary<TileDir, Vector2Int>()
            {
                { TileDir.FORWARD_C,   new Vector2Int( 0,  1)  },
                { TileDir.FORWARD_R,   new Vector2Int( 1,  1)  },
                { TileDir.MIDDLE_R,    new Vector2Int( 1,  0)  },
                { TileDir.BACKWARD_R,  new Vector2Int( 1,  -1) },
                { TileDir.BACKWARD_C,  new Vector2Int( 0,  -1) },
                { TileDir.BACKWARD_L,  new Vector2Int(-1,  -1) },
                { TileDir.MIDDLE_L,    new Vector2Int(-1,  0)  },
                { TileDir.FORWARD_L,   new Vector2Int(-1,  1)  }
            };

        // An unchanging dict of TileDirs, accessable by direction vector
        public static readonly Dictionary<Vector2Int, TileDir> DirectionEnumsByVector = 
            new Dictionary<Vector2Int, TileDir>()
            {
                { new Vector2Int( 0,  1),   TileDir.FORWARD_C   },
                { new Vector2Int( 1,  1),   TileDir.FORWARD_R   },
                { new Vector2Int( 1,  0),   TileDir.MIDDLE_R    },
                { new Vector2Int( 1,  -1),  TileDir.BACKWARD_R  },
                { new Vector2Int( 0,  -1),  TileDir.BACKWARD_C  },
                { new Vector2Int(-1,  -1),  TileDir.BACKWARD_L  },
                { new Vector2Int(-1,  0),   TileDir.MIDDLE_L    },
                { new Vector2Int(-1,  1),   TileDir.FORWARD_L   }
            };

        /// <summary>
        /// Takes two positions, origin and mapPositionB, 
        /// and returns a normalized direction vector //changed
        /// </summary>
        public static Vector2Int GetDirectionVec(Vector2Int origin, Vector2Int target)
        {
            int x = target.x - origin.x;
            int y = target.y - origin.y;

            //get greatest common divisor to normalize the direction vector
            int gcd = Mathf.Abs(GCD(x, y));

            //// In these examples, the origin object is
            //// "Looking at" the same mapPositionA at (-8, 2)
            //// ex1. origin = (0, 0), mapPositionB = (-8, 2)
            //// ex2. origin = (-4, 5), mapPositionB = (-8, 2)

            //// Convert ints to floats and get the difference.
            //    // ex1. difference = (-8.0, 2.0)
            //    // ex2. difference = (-4.0, -3.0)
            //Vector2 difference = forward - origin;

            //// Force a magnitude of 1.
            //    // ex1. difference = (-1.0, 0.25)
            //    // ex2. difference = (-1.0, -0.75)
            //difference.Normalize();

            //// Round both values to int
            //    // ex1. x = -1, y = 0
            //    // ex2. x = -1, y = -1
            //x = Mathf.RoundToInt(difference.x);
            //y = Mathf.RoundToInt(difference.y);

            //// Return the direction vector.
            //    // ex1. Returns (-1, 0), a vector
            //    // equivalent to MapDirectionsByEnum[MIDDLE_L]
            //    // ex2. Returns (-1, -1), a vector
            //    // euqivalent to MapDirectionsByEnum[BACKWARD_L]
            return new Vector2Int(Mathf.Clamp(x, -1, 1), Mathf.Clamp(y, -1, 1));
        }

        /// <summary>
        /// Calculates the Greatest Common Divisor of two integers.
        /// </summary>
        private static int GCD(int a, int b)
        {
            if (b == 0)
                return a;
            return GCD(b, a % b);
        }

        /// <summary>
        /// Maps a direction vector to the corresponding TileDir enum.
        /// </summary>
        public static TileDir GetTileDirFromVector(Vector2Int directionVec)
        {
            if (DirectionEnumsByVector.TryGetValue(directionVec, out TileDir dirEnum))
            {
                return dirEnum;
            }
            else
            {
                // Handle invalid direction vector
                Debug.LogWarning($"Invalid direction vector: {directionVec}");
                return TileDir.FORWARD_C; // Default to forward
            }
        }
    }

    

    #region STRUCTS
    /// <summary>
    /// A struct containing 3 Vector2Ints.
    /// On construction, it stores a difference Vector based
    /// on the mapPositionA and mapPositionB given.
    /// </summary>
    public struct DirectionalInfo
    {
        // MapPosition (on the game board) of a tile
        public Vector2Int MapPosition { get; private set; }

        // MapPosition (on the game board) of whatever
        // we consider to be "mapPositionB" from myPos.
        public Vector2Int MapForward { get; private set; }

        // The direction the object is mapPositionB
        public Vector2Int DirectionVec { get; private set; }

        public TileDir DirectionName { get; private set; }

        public DirectionalInfo(Vector2Int mapPositionA, Vector2Int mapPositionB)
        {
            MapPosition = mapPositionA;
            DirectionVec = DirectionHelper.GetDirectionVec(mapPositionA, mapPositionB);

            TileDir directionName;
            if(DirectionHelper.DirectionEnumsByVector.TryGetValue(DirectionVec, out directionName))
            {
                DirectionName = directionName;
            }
            else
            {
                DirectionName = (TileDir)0;
            }

            MapForward = MapPosition + DirectionVec;
        }
    }
    #endregion
}

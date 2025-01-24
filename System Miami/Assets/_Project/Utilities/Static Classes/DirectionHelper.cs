// Authors: Layla Hoey
using System.Collections.Generic;
using SystemMiami.Enums;
using UnityEngine;

namespace SystemMiami.Utilities
{
    public static class DirectionHelper
    {
        // An unchanging dict of moveDirection vectors, accessable by TileDir
        public static readonly Dictionary<TileDir, Vector2Int>
            BoardDirectionVecByEnum = new()
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

        // An unchanging dict of TileDirs, accessable by moveDirection vector
        public static readonly Dictionary<Vector2Int, TileDir>
            BoardDirectionEnumByVector = new()
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

        // An unchanging dict of TileDirs representing
        // screen directions, accessable by
        // TileDir for board directions
        public static readonly Dictionary<TileDir, TileDir>
            BoardToScreenEnumConversion = new()
            {
                { TileDir.FORWARD_C,   TileDir.FORWARD_L  },
                { TileDir.FORWARD_R,   TileDir.FORWARD_C  },
                { TileDir.MIDDLE_R,    TileDir.FORWARD_R  },
                { TileDir.BACKWARD_R,  TileDir.MIDDLE_R   },
                { TileDir.BACKWARD_C,  TileDir.BACKWARD_R },
                { TileDir.BACKWARD_L,  TileDir.BACKWARD_C },
                { TileDir.MIDDLE_L,    TileDir.BACKWARD_L },
                { TileDir.FORWARD_L,   TileDir.MIDDLE_L   }
            };

        /// <summary>
        /// Takes two positions, origin and mapPositionB, 
        /// and returns a difference vector (only 0s or signed 1s).
        /// </summary>
        public static Vector2Int GetDirectionVec(Vector2Int origin, Vector2Int forward)
        {
            int x = 0;
            int y = 0;

            // In these examples, the origin object is
            // "Looking at" the same mapPositionA at (-8, 2)
            // ex1. origin = (0, 0), mapPositionB = (-8, 2)
            // ex2. origin = (-4, 5), mapPositionB = (-8, 2)

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

            // Return the moveDirection vector.
            // ex1. Returns (-1, 0), a vector
            // equivalent to MapDirectionsByEnum[MIDDLE_L]
            // ex2. Returns (-1, -1), a vector
            // equivalent to MapDirectionsByEnum[BACKWARD_L]
            return new Vector2Int(x, y);
        }

        public static TileDir GetBoardTileDir(Vector2Int directionVec)
        {
            if (BoardDirectionEnumByVector.TryGetValue(directionVec, out TileDir screenDir))
            {
                return screenDir;
            }
            else
            {
                return TileDir.FORWARD_C;
            }
        }

        public static TileDir GetScreenTileDir(TileDir directionVec)
        {
            if (BoardToScreenEnumConversion.TryGetValue(directionVec, out TileDir dirEnum))
            {
                return dirEnum;
            }
            else
            {
                return TileDir.FORWARD_C;
            }
        }

        public static void Print(DirectionContext dirInfo, string objectName)
        {
            Debug.LogWarning($"{objectName}|  MapOrigin {dirInfo.BoardPositionA}, MapFWD {dirInfo.ForwardA}, " +
                $"MapDir{dirInfo.DirectionVec}, DirName {dirInfo.BoardDirection}");
        }

        public static void Print(Dictionary<TileDir, Vector2Int> dirDict, string objectName)
        {
            string report = $"{objectName}\n";

            foreach(TileDir dir in dirDict.Keys)
            {
                report += $"{dir}: {dirDict[dir]}\n";
            }

            Debug.LogWarning(report);
        }
    }
}

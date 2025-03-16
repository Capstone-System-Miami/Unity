// Authors: Layla Hoey
using System.Collections.Generic;
using SystemMiami.Enums;
using UnityEngine;
using UnityEngine.Assertions;

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

        // An unchanging dict of screen-relative
        // directions, accessible by isometric TileDir
        public static readonly Dictionary<TileDir, ScreenDir>
            BoardToScreenEnumConversion = new()
            {
                { TileDir.FORWARD_C,   ScreenDir.UP_L   },
                { TileDir.FORWARD_R,   ScreenDir.UP     },
                { TileDir.MIDDLE_R,    ScreenDir.UP_R   },
                { TileDir.BACKWARD_R,  ScreenDir.R      },
                { TileDir.BACKWARD_C,  ScreenDir.DOWN_R },
                { TileDir.BACKWARD_L,  ScreenDir.DOWN_C },
                { TileDir.MIDDLE_L,    ScreenDir.DOWN_L },
                { TileDir.FORWARD_L,   ScreenDir.L      }
            };

        /// <summary>
        /// Takes two positions, tilePosA and tilePosB, 
        /// and returns a difference vector (only 0s or signed 1s).
        /// </summary>
        public static Vector2Int GetDirectionVec(Vector2Int tilePosA, Vector2Int tilePosB)
        {
            int x = 0;
            int y = 0;

            // In these examples,
            // the origin object at tilePosA is
            // "looking at" the same tilePosB at (-8, 2)
            //
            // ex1. | tilePosA = ( 0, 0) | tilePosB = (-8, 2)
            // ex2. | tilePosA = (-4, 5) | tilePosB = (-8, 2)

            // Convert ints to floats and get the difference.
            // ex1. | difference = (-8.0,  2.0)
            // ex2. | difference = (-4.0, -3.0)
            Vector2 difference = tilePosB - tilePosA;

            // Force a magnitude of 1.
            // ex1. | difference = (-1.0,  0.25)
            // ex2. | difference = (-1.0, -0.75)

            // i guess this doesnt work the way you'd expect
            // keeping it here incase changing it breaks everything.
            // VV
            //difference.Normalize();
            // ^^
            difference = ScaledClamp(difference);

            // Round both values to int
            // ex1. | x = -1, y = 0
            // ex2. | x = -1, y = -1
            x = Mathf.RoundToInt(difference.x);
            y = Mathf.RoundToInt(difference.y);

            // Return the direction vector.
            // ex1. Returns (-1, 0), a vector
            // equivalent to MapDirectionsByEnum[MIDDLE_L]
            // ex2. Returns (-1, -1), a vector
            // equivalent to MapDirectionsByEnum[BACKWARD_L]
            return new Vector2Int(x, y);
        }

        public static Vector2 ScaledClamp(Vector2 vec)
        {
            return ScaledClamp(vec, 1f);
        }

        public static Vector2 ScaledClamp(Vector2 vec, float targetRadius)
        {
            float currentRadius = Mathf.Max(Mathf.Abs(vec.x), Mathf.Abs(vec.y));
            float scaleDivisor = currentRadius / targetRadius;
            return vec / scaleDivisor;
        }

        public static Vector2Int GetNormalized(Vector2Int intVec)
        {
            Vector2 floatVec = ((Vector2)intVec);
            Vector2 floatVecScaled = ScaledClamp(floatVec);

            Vector2Int result = new(
                Mathf.RoundToInt(floatVecScaled.x),
                Mathf.RoundToInt(floatVecScaled.y)
            );

            return result;
        }

        public static TileDir GetTileDir(Vector2Int directionVec)
        {
            directionVec = GetNormalized(directionVec);

            Debug.Log($"{directionVec}");

            Assert.IsTrue(BoardDirectionEnumByVector.ContainsKey(directionVec));

            return BoardDirectionEnumByVector[directionVec];
        }

        public static TileDir GetTileDir(ScreenDir screenDir)
        {
            foreach(TileDir key in BoardToScreenEnumConversion.Keys)
            {
                if (BoardToScreenEnumConversion[key] == screenDir)
                {
                    return key;
                }
            }

            // Default
            return TileDir.FORWARD_C;
        }

        public static ScreenDir GetScreenDir(TileDir tileDir)
        {
            return BoardToScreenEnumConversion[tileDir];
        }

        public static void Print(DirectionContext dirInfo, string objectName)
        {
            Debug.LogWarning(
                $"{objectName}\n" +
                $"| A           {dirInfo.TilePositionA}\n" +
                $"| B           {dirInfo.TilePositionB}\n" +
                $"| MapDir      {dirInfo.DirectionVec}\n" +
                $"| DirName     {dirInfo.BoardDirection}\n" +
                $"| ScreenDir   {dirInfo.ScreenDirection}\n" +
                $"| Fwd_A       {dirInfo.ForwardA}\n" +
                $"| Fwd_B       {dirInfo.ForwardB}\n");
        }

        public static void PrintAndHighlight(DirectionContext dirInfo, string objectName)
        {
            Print(dirInfo, objectName);

            Vector2Int[] poss = dirInfo.getlist();
            OverlayTile[] tiles = getlist(poss);

            Color[] colors =
            {
                Color.blue,
                Color.red,
                Color.cyan,
                Color.magenta
            };

            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i]?.Highlight(colors[i]);
            }
        }

        public static void Unhighlight(OverlayTile[] tiles)
        {
            for(int i = 0; i < tiles.Length; i++)
            {
                tiles[i]?.UnHighlight();
            }
        }

        public static OverlayTile[] getlist(Vector2Int[] posits)
        {
            OverlayTile[] result = new OverlayTile[4];

            for (int i = 0; i < 4; i++)
            {
                MapManager.MGR.map.TryGetValue(posits[i], out OverlayTile newBoi);
                result[i] = newBoi;
            }

            return result;
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

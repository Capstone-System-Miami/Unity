// Author: Layla Hoey
using UnityEngine;

namespace SystemMiami
{
    // Holy shit, an enum so long it deserves its own script
    // Oh man... it's a doozy
    public enum ExitDirections
    {
        // 1 DirectionVec
        NorthOnly, WestOnly, SouthOnly, EastOnly,

        // 2 DirectionVec
        NorthWest, NorthEast,
        SouthWest, SouthEast,
        NorthSouth, WestEast,

        // 3 DirectionVec
        NorthWestSouth, NorthEastSouth,
        NorthWestEast, SouthWestEast,

        AllDirections, NoDirections
    }
}

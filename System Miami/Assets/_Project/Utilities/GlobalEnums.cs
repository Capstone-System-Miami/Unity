using UnityEngine;

namespace SystemMiami.Enums
{
    /// <summary>
    /// An enum of ISOMETRIC directions,
    /// starting from forward center,
    /// ( i.e. coords (0, 1) )
    /// moving clockwise.
    /// </summary>
    public enum TileDir
    {
        FORWARD_C,
        FORWARD_R,
        MIDDLE_R,
        BACKWARD_R,
        BACKWARD_C,
        BACKWARD_L,
        MIDDLE_L,
        FORWARD_L,
    }

    public enum ScreenDir
    {
        UP,
        UP_R,
        R,
        DOWN_R,
        DOWN_C,
        DOWN_L,
        L,
        UP_L,
    }
}

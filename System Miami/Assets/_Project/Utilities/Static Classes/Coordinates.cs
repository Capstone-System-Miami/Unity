// Authors: Layla Hoey
using UnityEngine;

namespace SystemMiami.Utilities
{
    public enum BlockHeight { NONE, HALF, FULL };

    // A helper class for transitions between
    // Screen and World/Tile spaces
    public static class Coordinates
    {
        /// <summary>
        /// Returns the isometric height distance from the
        /// center of the sprite to the frontmost corner of the sprite.
        /// This will have to change if the standard pivot (BOTTOM) is changed.
        /// </summary>
        public static float GetZOffset(BlockHeight height)
        {
            float zOffset = height switch
            {
                BlockHeight.NONE => .5f,
                BlockHeight.HALF => .25f,
                BlockHeight.FULL => 0f,
                _ => 0f
            };

            return zOffset;
        }

        /// <summary>
        /// TODO: Rename IsoToWorld during next maintenance.
        /// Takes an isometric tile _gridPosition,
        /// and returns the _gridPosition of the
        /// tile origin in screen space.
        /// </summary>
        public static Vector3 IsoToScreen(Vector3Int cellPosition)
        {
            Vector3 result;

            float xScreen, yScreen;
            int xIso, yIso, zIso;

            xIso = cellPosition.x;
            yIso = cellPosition.y;
            zIso = cellPosition.z;

            xScreen = ((xIso - yIso) * .5f);
            yScreen = ((xIso + yIso) * .25f) + GetScreenHeightOf(zIso);

            result = new Vector3(xScreen, yScreen);

            //Debug.Log($"Iso to Screen: {result}");
            return result;
        }

        /// <summary>
        /// TODO: Rename IsoToWorld during next maintenance.
        /// Takes an isometric tile _gridPosition,
        /// and returns the _gridPosition of the
        /// tile origin in screen space.
        /// This variant takes block height into account,
        /// returning the _gridPosition of the frontmost corner of the block.
        /// </summary>
        public static Vector3 IsoToScreen(Vector3Int cellPosition, BlockHeight height)
        {
            Vector3 screenPosRaw = IsoToScreen(cellPosition);

            Vector3 result = new Vector3(screenPosRaw.x, screenPosRaw.y - GetZOffset(height));

            //Debug.Log ($"height: {height}, result: {result}");
            return result;
        }

        /// <summary>
        /// TODO: Rename WorldToIso during next maintenance.
        /// Takes a Vector3 screen _gridPosition (which should have a z of zero),
        /// and the zIndex of the tile and returns the isometric tile _gridPosition.
        /// zIndex must be known for this to function as expected.
        public static Vector3Int ScreenToIso(Vector3 screenPos, int zIndex)
        {
            Vector3Int result;

            float xScreen, yScreen;
            float xIso, yIso;

            xScreen = screenPos.x;
            yScreen = screenPos.y;
            
            xIso = xScreen + (yScreen * 2);

            // In order to find the correct y _gridPosition in isometric space,
            // we need to remove however much of the screen y _gridPosition is the
            // result of simulated height
            yIso = ((yScreen * 2) - xScreen) - GetScreenHeightOf(zIndex);

            result = new Vector3Int((int)xIso, (int)yIso, zIndex);

            //Debug.Log($"Screen to iso\n" +
            //    $"In: x {xScreen}, y {yScreen}, zInd {zIndex}\n" +
            //    $"Out: x {result.x}, y {result.y}, z {result.z}");

            return result;
        }

        /// <summary>
        /// TODO: Rename GetWorldHeight during next maintenance.
        /// </summary>
        public static float GetScreenHeightOf(int zIndex)
        {
            return zIndex * .25f;
        }

        public static float GetZIndexOf(float screenHeight)
        {
            return screenHeight * 4;
        }

        public static float GetHeightInTiles(int zIndex)
        {
            return zIndex * .5f;
        }
    }
}

using UnityEngine;

namespace SystemMiami.Utilities
{
    public static class Coordinates
    {
        /// <summary>
        /// Takes an isometric tile position, 
        /// along witht the supposed height of the tile layer,
        /// and returns the position of the
        /// tile origin in screen space.
        /// </summary>
        /// <param name="isoPos"></param>
        /// <param name="isoHeight"></param>
        /// <returns></returns>
        public static Vector3 IsoToScreen(Vector2Int isoPos, float isoHeight)
        {
            Vector3 result;

            float xIso, yIso, zIso;
            float xScreen, yScreen;

            xIso = isoPos.x;
            yIso = isoPos.y;
            zIso = isoHeight;

            xScreen = ((xIso - yIso) * .5f);
            yScreen = ((xIso + yIso) * .25f) + HeightIsoToScreen(zIso);

            result = new Vector3(xScreen, yScreen);
            //Debug.Log("Iso to World: " + result + "\n" +
            //    "World back to Iso: " + WorldToIso(result));

            return result;
        }

        /// <summary>
        /// Takes a Vector3 screen position (which should have a z of zero),
        /// and returns the isometric tile position.
        /// The function assumes that the isometric height is zero, so
        /// any height considerations have to happen before or after this function.
        /// </summary>
        /// <param name="screenPos"></param>
        /// <returns></returns>
        public static Vector2Int ScreenToIso(Vector3 screenPos)
        {
            Vector2Int result;

            float xScreen, yScreen;
            float xIso, yIso;

            xScreen = screenPos.x;
            yScreen = screenPos.y;
            xIso = xScreen + (yScreen * 2);
            yIso = (yScreen * 2) - xScreen;

            result = new Vector2Int((int)xIso, (int)yIso);
            return result;
        }

        public static float HeightIsoToScreen(float isoHeight)
        {
            float result;

            result = isoHeight * .5f;

            return result;
        }
    }
}

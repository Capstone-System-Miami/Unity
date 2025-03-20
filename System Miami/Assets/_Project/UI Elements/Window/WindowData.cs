using UnityEngine;

namespace SystemMiami
{
    public class WindowData
    {
        public Camera cam;
        public Vector3 screenSpaceOpenPos;
        public Vector3 worldSpaceOpenPos;

        public WindowData(Camera cam, Vector2 screenSpaceOpenPos)
        {
            this.cam = cam;
            this.screenSpaceOpenPos = screenSpaceOpenPos;
            this.worldSpaceOpenPos = cam.ScreenToWorldPoint(screenSpaceOpenPos);
        }

        public WindowData(Camera cam, Vector3 worldSpaceOpenPos)
        {
            this.cam = cam;
            this.screenSpaceOpenPos = cam.WorldToScreenPoint(worldSpaceOpenPos);
            this.worldSpaceOpenPos = worldSpaceOpenPos;
        }
    }
}

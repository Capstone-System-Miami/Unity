using UnityEngine;

namespace SystemMiami.ui
{
    public static class UiHelpers
    {
        static public bool TryGetCanvasInParents(Transform transform, out Canvas result)
        {
            Transform searchTarget = transform;

            int maxDepth = 100;
            int depth = 0;

            while ((++depth < maxDepth) && (searchTarget != null))
            {
                // Update search target
                searchTarget = searchTarget.parent;

                if (searchTarget.TryGetComponent(out result))
                {
                    return true;
                }
            }

            result = null;
            return false;
        }
    }
}

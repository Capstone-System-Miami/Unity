using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SystemMiami.ui
{
    public class QuitGame : MonoBehaviour
    {
        public void Go()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }
#else
            Application.Quit();
#endif
        }
    }
}

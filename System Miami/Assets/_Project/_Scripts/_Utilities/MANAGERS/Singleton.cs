using UnityEngine;

namespace SystemMiami.Management
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private static T _mgr;

        [SerializeField] private bool _preserveBetweenScenes;

        public static T MGR { get { return _mgr; } }

        protected virtual void Awake()
        {
            if (_mgr != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _mgr = this as T;

                if (_preserveBetweenScenes)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
        }
    }
}

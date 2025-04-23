using UnityEngine;

namespace SystemMiami.Management
{
    public abstract class Singleton : MonoBehaviour {
        public abstract GameObject GetMe();
    }

    public abstract class Singleton<T> : Singleton where T : MonoBehaviour
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

        public override GameObject GetMe()
        {
            return MGR.gameObject;
        }
    }
}

using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    public class HighlightableClassSet<T> where T : class
    {
        [SerializeField] private T _normal;
        [SerializeField] private T _highlighted;

        public T Normal { get { return _normal; } }
        public T Highlighted { get { return _highlighted; } }

        public HighlightableClassSet(T val)
        {
            _normal = val;
            _highlighted = val;
        }

        public HighlightableClassSet(T normal, T highlighted)
        {
            _normal = normal;
            _highlighted = highlighted;
        }
    }
}

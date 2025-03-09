using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    public class HighlightableStructSet<T> where T : struct
    {
        [SerializeField] private T _normal;
        [SerializeField] private T _highlighted;

        public T Normal { get { return _normal; } }
        public T Highlighted { get { return _highlighted; } }

        public HighlightableStructSet()
        {
            _normal = default;
            _highlighted = default;
        }

        public HighlightableStructSet(T val)
        {
            _normal = val;
            _highlighted = val;
        }

        public HighlightableStructSet(T normal, T highlighted)
        {
            _normal = normal;
            _highlighted = highlighted;
        }
    }
}

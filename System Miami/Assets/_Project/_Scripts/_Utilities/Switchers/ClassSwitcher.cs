// Layla H
using UnityEngine;

namespace SystemMiami
{
    /// <summary>
    /// Allows getting and setting with built in
    /// memory of the previous value.
    /// Use with ints, floats, etc.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class ClassSwitcher<T> where T : class
    {
        [SerializeField] private T _default;
        private T _current;
        private T _buffer;

        public ClassSwitcher()
        {
            _current = _default;
            _buffer = _default;
        }

        public ClassSwitcher(T defaultSetting)
        {
            _default = defaultSetting;
            _current = _default;
            _buffer = _default;
        }

        public T Get()
        {
            return _current;
        }
        public void Set(T val)
        {
            _buffer = _current;
            _current = val;
        }

        public void Revert()
        {
            T temp = _current;
            _current = _buffer;
            _buffer = temp;
        }

        public void Reset()
        {
            _buffer = _current;
            _current = _default;
        }
    }
}

/// Layla
using System.Reflection;
using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    public class PoolElement<T> where T : Object
    {
        [Tooltip("If the pool's size exceeds " +
            "to the sum of Max Counts in the pool, " +
            "remaining spaces will be filled with the Default. " +
            "the first element in the pool with this box checked will be " +
            "set as the pool's Default, and will ignore " +
            "this box on remaining elements.")]
        [SerializeField] private bool _isDefault;
        [SerializeField] private T _elementPrefab;
        [SerializeField] private int _maxCount;

        private int _count;
        private bool _initialized = false;

        /// <summary>
        /// Takes everything from the incoming arg except
        /// the _count, which is reset to max during construction.
        /// </summary>
        public PoolElement(PoolElement<T> toCopy)
        {
            _isDefault = toCopy._isDefault;
            _elementPrefab = toCopy._elementPrefab;
            _maxCount = toCopy._maxCount;

            initialize();
        }

        public bool TryGet(out T prefab)
        {
            if(!canGet())
            {
                prefab = null;
                return false;
            }

            _count--;
            prefab = _elementPrefab;
            return true;
        }

        public bool IsDefault(out T prefab)
        {
            if (!_isDefault)
            {
                prefab = null;
                return false;
            }

            prefab = _elementPrefab;
            return true;
        }

        private bool canGet()
        {
            if (!_initialized)
            {
                initialize();
            }

            return _count > 0;
        }

        private void initialize()
        {
            _count = _maxCount;
            _initialized = true;
        }
    }
}

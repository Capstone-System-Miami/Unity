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
        [SerializeField] private int _minCount;
        [SerializeField] private int _maxCount;

        private int _availableRemaining;
        private int _requiredRemaining;
        private bool _initialized = false;

        /// <summary>
        /// Takes everything from the incoming arg except
        /// the _availableRemaining, which is reset to max during construction.
        /// </summary>
        public PoolElement(PoolElement<T> toCopy)
        {
            _isDefault = toCopy._isDefault;
            _elementPrefab = toCopy._elementPrefab;
            _minCount = toCopy._minCount;
            _maxCount = toCopy._maxCount;

            initialize();
        }

        public bool TryGetRequired(out T[] prefabs)
        {
            prefabs = new T[_requiredRemaining];

            for (int i = 0; i < prefabs.Length; i++)
            {
                prefabs[i] = _elementPrefab;
                _requiredRemaining--;
                _availableRemaining--;
            }
            return prefabs.Length > 0;
        }

        public bool TryGet(out T prefab)
        {
            if(!canGet())
            {
                prefab = null;
                return false;
            }

            _availableRemaining--;
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

            _availableRemaining--;
            prefab = _elementPrefab;
            return true;
        }

        private bool canGet()
        {
            if (!_initialized)
            {
                initialize();
            }

            return _availableRemaining > 0;
        }

        private void initialize()
        {
            _availableRemaining = _maxCount;
            _requiredRemaining = _minCount;
            _initialized = true;
        }
    }
}

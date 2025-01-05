using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    public class EnemyPoolElement
    {
        [Tooltip("If the pool's size exceeds " +
            "to the sum of Max Counts in the pool, " +
            "remaining spaces will be filled with the Default. " +
            "the first element in the pool with this box checked will be " +
            "set as the pool's Default, and will ignore " +
            "this box on remaining elements.")]
        [SerializeField] private bool _isDefualt;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _maxCount;

        private int _count;
        private bool _initialized = false;

        public bool TryGet(out GameObject prefab)
        {
            if(!canSpawn())
            {
                prefab = null;
                return false;
            }

            _count--;
            prefab = _prefab;
            return true;
        }

        public bool IsDefault(out GameObject prefab)
        {
            if (!_isDefualt)
            {
                prefab = null;
                return false;
            }

            prefab = _prefab;
            return true;
        }

        private bool canSpawn()
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

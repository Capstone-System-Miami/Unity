using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    public class Pool<T> where T : Object
    {
        [SerializeField] private int _minCount;
        [SerializeField] private int _maxCount;
        [SerializeField] private List<PoolElement<T>> _elements = new();

        private int _count;
        private List<T> _generatedList = new();
        private T _defaultElement;

        private bool _initialized = false;

        /// <summary>
        /// Will create a new copy of the EnemyPool arg
        /// with the same min and max enemies,
        /// but will generate a new enemy list with a newly randomized count.
        /// </summary>
        /// <param name="toCopy"></param>
        public Pool(Pool<T> toCopy)
        {
            _minCount = toCopy._minCount;
            _maxCount = toCopy._maxCount;
            
            for (int i = 0; i < toCopy._elements.Count; i++)
            {
                // Creating a copy of each element will reset the _count
                _elements.Add(new PoolElement<T>(toCopy._elements[i]));
            }

            _defaultElement = toCopy.getDefault();

            initialize();
        }

        public List<T> GetFinalizedList()
        {
            if (!_initialized)
            {
                initialize();
            }

            return _generatedList;
        }

        /// <summary>
        /// Calculates a random number of enemies in the range,
        /// finds and stores the default enemy,
        /// and generates a list with size of _count
        /// </summary>
        private void initialize()
        {
            _count = Random.Range(_minCount, _maxCount + 1);

            _defaultElement = getDefault();

            _generatedList = generateList();

            _initialized = true;
        }

        private List<T> generateList()
        {
            // So we don't modify the original list
            List<PoolElement<T>> elementsCopy = new(_elements);

            List<T> result = new();

            int minIndex = 0;
            int maxIndex = elementsCopy.Count;
            int randomIndex;

            // For each prefab we want to spawn
            for (int i = 0; i < _count; i++)
            {
                // Update max
                maxIndex = elementsCopy.Count;

                // If max is 0, use the default element for the remainder of the loop.
                if (maxIndex == 0)
                {
                    result.Add(getDefault());
                    continue;
                }

                // Get a random index for accessing the elements list
                randomIndex = Random.Range(minIndex, maxIndex);

                // If the element at the index is valid
                if (elementsCopy[randomIndex].TryGet(out T prefab))
                {
                    // Add a prefab of it to the result list
                    result.Add(prefab);
                }
                else
                {
                    // It's cashed
                    elementsCopy.RemoveAt(randomIndex);
                    
                    // Add a default prefab
                    result.Add(getDefault());
                }
            }

            return result;
        }

        private T getDefault()
        {
            if (_defaultElement != null)
            {
                return _defaultElement;
            }

            for (int i = 0; i < _elements.Count; i++)
            {
                if (_elements[i].IsDefault(out T prefab))
                {
                    _defaultElement = prefab;
                    return _defaultElement;
                }
            }

            Debug.LogWarning($"EnemyPool({this}) could not find a default element.");
            return null;
        }
    }
}

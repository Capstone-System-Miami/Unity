using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    public class EnemyPool
    {
        [SerializeField] private int _minEnemyCount;
        [SerializeField] private int _maxEnemyCount;
        [SerializeField] private List<EnemyPoolElement> _elements = new();

        private int _enemyCount;
        private List<GameObject> _enemiesToSpawn = new();
        private GameObject _defaultEnemy;

        private bool _initialized = false;

        /// <summary>
        /// Will create a new copy of the EnemyPool arg
        /// with the same min and max enemies,
        /// but will generate a new enemy list with a newly randomized count.
        /// </summary>
        /// <param name="toCopy"></param>
        public EnemyPool(EnemyPool toCopy)
        {
            _minEnemyCount = toCopy._minEnemyCount;
            _maxEnemyCount = toCopy._maxEnemyCount;
            _elements = toCopy._elements;

            _defaultEnemy = toCopy.getDefault();

            initialize();
        }

        public List<GameObject> GetPrefabsToSpawn()
        {
            if (!_initialized)
            {
                initialize();
            }

            return _enemiesToSpawn;
        }

        /// <summary>
        /// Calculates a random number of enemies in the range,
        /// finds and stores the default enemy,
        /// and generates a list with size of _count
        /// </summary>
        private void initialize()
        {
            _enemyCount = Random.Range(_minEnemyCount, _maxEnemyCount);

            _defaultEnemy = getDefault();

            _enemiesToSpawn = generateList();

            _initialized = true;
        }

        private List<GameObject> generateList()
        {
            // So we don't modify the original list
            List<EnemyPoolElement> elementsCopy = new(_elements);

            List<GameObject> result = new();

            int minIndex = 0;
            int maxIndex = elementsCopy.Count;
            int randomIndex;

            // For each prefab we want to spawn
            for (int i = 0; i < _enemyCount; i++)
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
                if (elementsCopy[randomIndex].TryGet(out GameObject prefab))
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

        private GameObject getDefault()
        {
            if (_defaultEnemy != null)
            {
                return _defaultEnemy;
            }

            for (int i = 0; i < _elements.Count; i++)
            {
                if (_elements[i].IsDefault(out GameObject prefab))
                {
                    _defaultEnemy = prefab;
                    return _defaultEnemy;
                }
            }

            Debug.LogWarning($"EnemyPool({this}) could not find a default element.");
            return null;
        }
    }
}

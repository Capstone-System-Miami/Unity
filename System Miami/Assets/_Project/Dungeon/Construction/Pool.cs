/// Layla
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SystemMiami.Dungeons;
using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    public class Pool<T> where T : Object
    {
        [SerializeField] private int _minCount;
        [SerializeField] private int _maxCount;
        [SerializeField] private List<PoolElement<T>> _elements;

        private T _defaultPrefab;

        /// <summary>
        /// Resets the counts of all Pool elements
        /// Finds and stores the default element
        /// Returns a newly generated list of elements.
        /// </summary>
        public List<T> GetNewList()
        {
            resetElementCounts();

            if (!tryGetDefault(out _defaultPrefab))
            {
                Debug.LogWarning($"No Default Element Fount in pool ( {this} )");
            }
            
            int count = Random.Range(_minCount, _maxCount + 1);

            return getListOfSize(count);
        }

        private void resetElementCounts()
        {
            /// Creating a copy of each element will reset their available counts
            for (int i = 0; i < _elements.Count; i++)
            {
                _elements[i] = new PoolElement<T>(_elements[i]);
            }
        }

        /// <summary>
        /// If no default prefab was found, this returns false,
        /// and outputs a null object of type `T`.
        /// If a default prefab WAS found, this returns true,
        /// and outputs the prefab.
        /// </summary>
        private bool tryGetDefault(out T defaultPrefab)
        {
            if (_defaultPrefab != null)
            {
                defaultPrefab = _defaultPrefab;
                return true;
            }

            for (int i = 0; i < _elements.Count; i++)
            {
                if (_elements[i].IsDefault(out T element))
                {
                    defaultPrefab = element;
                    return true;
                }
            }

            defaultPrefab = null;
            return false;
        }

        private List<T> getListOfSize(int size)
        {
            /// The list of actual raw objects we're constructing.
            List<T> result = new();

            /// A local copy of the PoolElements list, so we can leave
            /// the member variable _elements alone.
            /// Elements will be checked to see if they still have
            /// any _count remaining, and removed from
            /// this local copy of the list if they've been depleted.
            List<PoolElement<T>> validElements = new(_elements);

            Debug.Log("Elements before loop:\n" +
                string.Join("\n", validElements.Select(e => db.GetInfo(e, BindingFlags.NonPublic | BindingFlags.Instance))));

            /// Smallest index that contains a PoolElement.
            int validElementsMinIndex = 0;

            /// Largest index that contains a PoolElement.
            int validElementsMaxIndex = validElements.Count - 1;

            /// The random index we will be reassigning each iteration
            /// to index out validPoolElements list.
            int randomIndex = 0;

            /// For each iteration in our result list target size.
            for (int i = 0; i < size; i++)
            {
                /// Update max index, in case we removed
                /// any PoolElements last iteration.
                validElementsMaxIndex = validElements.Count - 1;

                /// If the largest PoolElement we can access is at position 0,
                if (validElementsMaxIndex == 0)
                {
                    /// We don't have anything left to add. But we still
                    /// want to leave with a list of the given size, so
                    /// we'll use the remainder of the loop to
                    /// fill the list with the default element.
                    result.Add(_defaultPrefab);
                    continue;
                }

                /// Get a random index for accessing the PoolElements list
                randomIndex = Random.Range(validElementsMinIndex, validElementsMaxIndex + 1);

                /// If TryGet() returns true for the PoolElement at
                /// the random index, `prefab` will contain a copy of
                /// whatever actual object is stored in the PoolElement.
                /// If the PoolElement's TryGet() returns false,
                /// the new variable `prefab` will contain null.
                if (!_elements[randomIndex].TryGet(out T prefab))
                {
                    /// it's cashed, so remove it from validPoolElements
                    validElements.RemoveAt(randomIndex);

                    /// Add a default prefab instead
                    result.Add(_defaultPrefab);

                    Debug.Log(
                        $"{this} Added a default prefab ({_defaultPrefab.name})\n" +
                        getLoopInfo(validElementsMinIndex, validElementsMaxIndex, randomIndex, i) +
                        $"Updated Elements:\n" +
                        string.Join( "\n", validElements.Select( e => db.GetInfo(e, BindingFlags.NonPublic | BindingFlags.Instance) ) )
                        );

                    continue;
                }

                /// Add a prefab of it to the result list
                result.Add(prefab);

                Debug.Log(
                    $"{this} Added a prefab ({prefab.name})\n" +
                    getLoopInfo(validElementsMinIndex, validElementsMaxIndex, randomIndex, i) +
                    $"Updated Elements:\n" +
                    string.Join("\n", validElements.Select(e => db.GetInfo(e, BindingFlags.NonPublic | BindingFlags.Instance)))
                    );
            }

            return result;
        }

        //private List<T> getListOfSize(int size)
        //{
        //    List<T> result = new();
        //    List<PoolElement<T>> validElements = new(_elements);

        //    Debug.LogError("Elements: " + string.Join("\n", validElements.Select(e => e.GetInfo())));

        //    if (validElements.Count <= 0)
        //    {
        //        Debug.LogError($"No valid elements found on {this}");
        //    }

        //    int validElementsMinIndex = 0;
        //    int validElementsMaxIndex = validElements.Count - 1;
        //    int randomIndex = 0;

        //    for (int i = 0; i < size; i++)
        //    {
        //        validElementsMaxIndex = validElements.Count - 1;
        //        if (validElementsMaxIndex == 0)
        //        {
        //            result.Add(_defaultPrefab);
        //            continue;
        //        }

        //        randomIndex = Random.Range(validElementsMinIndex, validElementsMaxIndex + 1);

        //        /// Enhance Debugging
        //        if (i == 0) // Log state at the first iteration
        //        {
        //            Debug.LogError("At first iteration:" + getLoopInfo(validElementsMinIndex, validElementsMaxIndex, randomIndex, i));
        //        }

        //        if (!validElements[randomIndex].TryGet(out T prefab))
        //        {
        //            validElements.RemoveAt(randomIndex);
        //            result.Add(_defaultPrefab);
        //            continue;
        //        }

        //        result.Add(prefab);
        //        Debug.LogError(getLoopInfo(validElementsMinIndex, validElementsMaxIndex, randomIndex, i));
        //    }

        //    /// Log the final results
        //    Debug.LogError("Result list: " + string.Join(", ", result.Select(e => e.name)));

        //    return result;
        //}


        private string getLoopInfo(int minIndex, int maxIndex, int randomIndex, int iteration)
        {
            return $"LoopInfo\n" +
                $"| Iteration: {iteration}\n" +
                $"| minIndex: {minIndex} | maxIndex: {maxIndex}\n" +
                $"| randIndex: {randomIndex}\n";
        }
    }
}

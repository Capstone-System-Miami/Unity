/// Layla
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SystemMiami.Utilities;
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

        public T DefaultPrefab { get { return _defaultPrefab; } }

        /// <summary>
        /// Resets the counts of all Pool elements
        /// Finds and stores the default element
        /// Returns a newly generated list of elements.
        /// </summary>
        public List<T> GetNewList()
        {
            List<T> result = new();

            resetElementCounts();

            if (!tryGetDefault(out _defaultPrefab))
            {
                Debug.LogWarning($"No Default Element Found in pool ( {this} )");
            }

            result = getRequired();

            int availableCountRemaining = Mathf.Clamp((_maxCount - result.Count), 0, _maxCount);

            // if result is greater than min, this will be neg
            int requiredCountRemaining = Mathf.Clamp((_minCount - result.Count), 0, _minCount);

            if (availableCountRemaining == 0)
            {
                return result;
            }

            int randomCountRemaining = Random.Range(requiredCountRemaining, availableCountRemaining);

            result.AddRange(getListOfSize(randomCountRemaining));
            return result;
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
        /// Gets, concats, & returns the required number of elements
        /// from each <see cref="PoolElement{T}">.
        /// </summary>
        private List<T> getRequired()
        {
            List<T> result = new();

            for (int i = 0; i < _elements.Count; i++)
            {
                if (!_elements[i].TryGetRequired(out T[] req))
                {
                    continue;
                }

                result.AddRange(req);
            }

            return result;
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

            //Debug.Log(
            //    $"Beginning of {this}'s getListOfSize() func\n" +
            //    getPoolInfo(validElements)
            //    );

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
                        $"A Pool<{_defaultPrefab.GetType()}> Added a default prefab ({_defaultPrefab.name})\n"
                        // + getLoopInfo(validElementsMinIndex, validElementsMaxIndex, randomIndex, i) +
                        // $"Updated Elements:\n" +
                        // string.Join("\n", validElements.Select(e => DebugHelpers.GetInfo(e, BindingFlags.NonPublic | BindingFlags.Instance)))
                        );

                    continue;
                }

                /// Add a prefab of it to the result list
                result.Add(prefab);

                //Debug.Log(
                //    $"{this} Added a prefab ({prefab.name})\n" +
                //    getLoopInfo(validElementsMinIndex, validElementsMaxIndex, randomIndex, i) +
                //    getPoolInfo(validElements)
                //    );
            }

            //Debug.Log(
            //    $"End of {this}'s getListOfSize() func\n" +
            //    getPoolInfo(validElements));

            return result;
        }

        private string getPoolInfo(List<PoolElement<T>> elements)
        {
            BindingFlags binding = BindingFlags.NonPublic | BindingFlags.Instance;

            int element = 0;

            return $"Elements:\n" +
            string.Join("\n",
                    elements.Select(e => $"{element++}:\n" + DebugHelpers.GetInfo(e, binding))
                    );
        }


        private string getLoopInfo(int minIndex, int maxIndex, int randomIndex, int iteration)
        {
            return $"LoopInfo\n" +
                $"| Iteration: {iteration}\n" +
                $"| minIndex: {minIndex} | maxIndex: {maxIndex}\n" +
                $"| randIndex: {randomIndex}\n";
        }
    }
}

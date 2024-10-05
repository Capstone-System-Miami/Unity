// Author: Layla Hoey
using UnityEngine;

namespace SystemMiami
{
    // This selection logic isn't meant to be permanent, just a demo
    // Maybe this just all belongs in the Generator anyway.
    // If so, you can just use it as a literal storage container
    // for organizing prefabs
    [CreateAssetMenu(fileName = "Intersection Pool", menuName = "Intersection Pool")]
    public class IntersectionPool : ScriptableObject
    {
        [SerializeField] private GameObject[] _intersectionPrefabs;
        [SerializeField] private int _maxInstances;

        private int[] _currentInstances;
        private bool _maxedOut;

        public ExitDirections Directions;

        private bool isValid(int index)
        {
            if (_currentInstances[index] >= _maxInstances)
            {
                return false;
            }

            /*
            intersection = _intersectionPrefabs[index].GetComponent<Intersection>()

            if (intersection.GetSomeCondition != theRightCondition)
            {
                return false;
            }

            etc
            */

            return true;
        }

        private bool maxedOut()
        {
            for (int i = 0; i < _intersectionPrefabs.Length; i++)
            {
                if (!isValid(i)) { return true; }
            }

            return false;
        }

        public GameObject GetRandomPrefab()
        {
            if (maxedOut()) { return null; }

            int randomIndex;

            do {
                randomIndex = Random.Range(0, _intersectionPrefabs.Length);
            } while (!isValid(randomIndex));

            _currentInstances[randomIndex]++;

            return _intersectionPrefabs[randomIndex];
        }
    }
}

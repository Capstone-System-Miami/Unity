// Author: Layla Hoey
using UnityEngine;

namespace SystemMiami
{
    // The Generator script can then find an appropriate prefab like this
    public class IntersectionPool_Driver : MonoBehaviour
    {

        [SerializeField] private IntersectionPool _n;
        [SerializeField] private IntersectionPool _w;
        [SerializeField] private IntersectionPool _s;
        [SerializeField] private IntersectionPool _e;
        [SerializeField] private IntersectionPool _nw;
        [SerializeField] private IntersectionPool _sw;
        [SerializeField] private IntersectionPool _se;
        [SerializeField] private IntersectionPool _ns;
        [SerializeField] private IntersectionPool _we;
        [SerializeField] private IntersectionPool _nws;
        [SerializeField] private IntersectionPool _nes;
        [SerializeField] private IntersectionPool _nwe;
        [SerializeField] private IntersectionPool _swe;
        [SerializeField] private IntersectionPool _all;
        [SerializeField] private IntersectionPool _none;

        private IntersectionPool[] _intersectionPools;

        private void Awake()
        {
            initializePrefabArray();
        }

        private void initializePrefabArray()
        {
            int directionCombinations = System.Enum.GetNames(typeof(ExitDirections)).Length;

            _intersectionPools[(int)ExitDirections.NorthOnly] = _n;
            _intersectionPools[(int)ExitDirections.WestOnly] = _w;
            _intersectionPools[(int)ExitDirections.SouthOnly] = _s;
            _intersectionPools[(int)ExitDirections.EastOnly] = _e;
            _intersectionPools[(int)ExitDirections.NorthWest] = _nw;
            _intersectionPools[(int)ExitDirections.SouthWest] = _sw;
            _intersectionPools[(int)ExitDirections.SouthEast] = _se;
            _intersectionPools[(int)ExitDirections.NorthSouth] = _ns;
            _intersectionPools[(int)ExitDirections.WestEast] = _we;
            _intersectionPools[(int)ExitDirections.NorthWestSouth] = _nws;
            _intersectionPools[(int)ExitDirections.NorthEastSouth] = _nes;
            _intersectionPools[(int)ExitDirections.NorthWestEast] = _nwe;
            _intersectionPools[(int)ExitDirections.SouthWestEast] = _swe;
            _intersectionPools[(int)ExitDirections.AllDirections] = _all;
            _intersectionPools[(int)ExitDirections.NoDirections] = _none;
        }

        public GameObject GetIntersectionOfType(ExitDirections exitDirections)
        {
            return _intersectionPools[(int)exitDirections].GetRandomPrefab();
        }
    }
}

using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami
{
    public class FieldReserializerTestComponent : MonoBehaviour, IRemoveUnderScores
    {
        System.Type IRemoveUnderScores.Type => GetType();

        [SerializeField] private int _testInt = 0;
        [SerializeField] private string _testString = "";
        [SerializeField] private Vector3 _testVec = Vector3.zero;
        [SerializeField] private GameObject _testPrefab = null;
        [SerializeField] private TestSerializableClass _testSerializableClass;

        [SerializeField] private int testInt = 0;
        [SerializeField] private string testString = "";
        [SerializeField] private Vector3 testVec = Vector3.zero;
        [SerializeField] private GameObject testPrefab = null;
        [SerializeField] private TestSerializableClass testSerializableClass;
    }

    [System.Serializable]
    public class TestSerializableClass : IRemoveUnderScores
    {
        System.Type IRemoveUnderScores.Type => GetType();

        [SerializeField] private int _testIntInner;
        [SerializeField] private string _testStringInner;
        [SerializeField] private Vector3 _testVecInner;
        [SerializeField] private GameObject _testPrefabInner;

        [SerializeField] private int testIntInner;
        [SerializeField] private string testStringInner;
        [SerializeField] private Vector3 testVecInner;
        [SerializeField] private GameObject testPrefabInner;
    }
}

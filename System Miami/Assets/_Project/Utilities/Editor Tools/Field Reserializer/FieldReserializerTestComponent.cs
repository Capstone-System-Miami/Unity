using System.Collections.Generic;
using SystemMiami.CustomEditor;
using UnityEngine;

namespace SystemMiami.Utilities
{
    /// NOTE:
    /// For testing the FieldReserializer editor tool.
    /// To use, uncomment the code below. Fill in values in the inspector, and save.
    /// Then, open the tool, find a prefab that uses this class,
    /// and apply the tool. The old values will be replaced with the new ones.
    /// This way, you can rename or reset fields without losing references on prefabs.
    /// Then you can delete all of the old fields and the dictionary, and remove
    /// the IFieldReserializerInterface.
    public class FieldReserializerTestComponent : MonoBehaviour, IFieldReserializer
    {
        static Dictionary<string, string> oldFieldName_newFieldName = new()
        {
            {"_testRenameEditor" , "newGuy7000_hellYeah"},
            {"_testInt" , "testInt"},
            {"_testString" , "testString"},
            {"_testVec" , "testVec"},
            {"_testPrefab" , "testPrefab"},
            {"_testSerializableClass" , "testSerializableClass"},
        };
        Dictionary<string, string> IFieldReserializer.OldFieldName_NewFieldName()
        {
            return oldFieldName_newFieldName;
        }

        // [Header("Test Old Vals")]
        // [SerializeField] private int _testInt = 0;
        // [SerializeField] private string _testString = "";
        // [SerializeField] private Vector3 _testVec = Vector3.zero;
        // [SerializeField] private GameObject _testPrefab = null;
        // [SerializeField, Space(5)] private TestSerializableClass _testSerializableClass;
        // [SerializeField] private GameObject _testRenameEditor;
        // [Space(10)]
        //
        // [Header("Test New Vals")]
        // [SerializeField] private int testInt = 0;
        // [SerializeField] private string testString = "";
        // [SerializeField] private Vector3 testVec = Vector3.zero;
        // [SerializeField] private GameObject testPrefab = null;
        // [SerializeField, Space(5)] private TestSerializableClass testSerializableClass;
        // [SerializeField] private GameObject newGuy7000_hellYeah;
        //
        // ~FieldReserializerTestComponent()
        // {
        //     Debug.Log("FieldReserializerTestComponent destroyed");
        // }
    }

    [System.Serializable]
    public class TestSerializableClass : IFieldReserializer
    {
        static Dictionary<string, string> oldFieldName_newFieldName = new()
        {
            {"_testIntInner" , "testIntInner"},
            {"_testStringInner" , "testStringInner"},
            {"_testVecInner" , "testVecInner"},
            {"_testPrefabInner" , "testPrefabInner"},
        };

        [SerializeField] private int _testIntInner;
        [SerializeField] private string _testStringInner;
        [SerializeField] private Vector3 _testVecInner;
        [SerializeField] private GameObject _testPrefabInner;

        [SerializeField] private int testIntInner;
        [SerializeField] private string testStringInner;
        [SerializeField] private Vector3 testVecInner;
        [SerializeField] private GameObject testPrefabInner;

        Dictionary<string, string> IFieldReserializer.OldFieldName_NewFieldName()
        {
            return oldFieldName_newFieldName;
        }
    }
}

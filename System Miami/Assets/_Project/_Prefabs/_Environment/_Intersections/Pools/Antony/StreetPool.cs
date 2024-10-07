using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Street Pool", menuName = "Street Pool")]
public class StreetPool : ScriptableObject
{
    public StreetType streetType;
    public GameObject[] streetPrefabs;
}
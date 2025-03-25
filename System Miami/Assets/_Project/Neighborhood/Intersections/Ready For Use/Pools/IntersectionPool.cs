using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Made By Antony

[CreateAssetMenu(fileName = "New Street Pool", menuName = "Street Pool")]
public class IntersectionPool : ScriptableObject
{
    public IntersectionType streetType;
    public GameObject[] streetPrefabs;
}
using System.Collections;
using System.Collections.Generic;
using SystemMiami.Management;
using UnityEngine;

namespace SystemMiami
{
    public class DontDestroy : MonoBehaviour
    {
        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}

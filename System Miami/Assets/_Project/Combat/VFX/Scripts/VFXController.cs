using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    public class VFXController : MonoBehaviour
    {
        public float lifetime;

        public void Update()
        {
            Destroy(this.gameObject,lifetime);
        }
    }
}

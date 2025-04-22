using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    public class LevelUpText : MonoBehaviour
    {
        public void Update()
        {
            if(GameObject.FindGameObjectWithTag("Menu") && gameObject.activeSelf)
            {
              gameObject.SetActive(false);
            }
        }
        
       
    }
}

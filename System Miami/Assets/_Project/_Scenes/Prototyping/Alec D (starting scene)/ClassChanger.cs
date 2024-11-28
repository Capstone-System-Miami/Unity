using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami
{
    public class ClassChanger : MonoBehaviour
    {
        public CharacterClassType classType;
        public GameObject player;
        private Attributes attributes;

        private void Start()
        {
            attributes = player.GetComponent<Attributes>();
            
        }


        public void OnClick()
        {
            attributes.SetClass(classType);
        }
    }
}

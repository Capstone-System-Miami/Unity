using System.Collections;
using System.Collections.Generic;
using SystemMiami.InventorySystem;
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami
{
    public class ClassChanger : MonoBehaviour
    {
        public CharacterClassType classType;
        public GameObject player;
        private Attributes attributes;
        [SerializeField] private Inventory playerInventory;

        private void Start()
        {
            
            attributes = player.GetComponent<Attributes>();
            playerInventory = player.GetComponent<Inventory>();
            
        }


        public void OnClick()
        {
            attributes.SetClass(classType);
            playerInventory.InitializeStartingAbility(classType);
        }
    }
}

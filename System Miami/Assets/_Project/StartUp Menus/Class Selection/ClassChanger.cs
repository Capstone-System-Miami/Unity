using SystemMiami.InventorySystem;
using UnityEngine;
using SystemMiami.Drivers;

namespace SystemMiami
{
    public class ClassChanger : MonoBehaviour
    {
        public CharacterClassType classType;
        public GameObject player;
        private Attributes attributes;
        private Inventory playerInventory;
        private CharClassAnimationDriver animDriver;

        private void Start()
        {
            attributes = player.GetComponent<Attributes>();
            playerInventory = player.GetComponent<Inventory>();
            animDriver = player.GetComponent<CharClassAnimationDriver>();
            animDriver.SetUseExistingClass(true);
        }

        public void OnClick()
        {
            attributes.SetClass(classType);
            playerInventory.InitializeStartingAbility(classType);
            animDriver.SetPlayerStandardAnims();
        }
    }
}

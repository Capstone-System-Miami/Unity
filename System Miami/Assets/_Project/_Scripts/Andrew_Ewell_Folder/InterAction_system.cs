using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    public class InterAction_system : MonoBehaviour
    {
        public GameObject Actor; //GameObject as Player or NPC
        public KeyCode interact = KeyCode.F; //change if needed


        //Code uses Tag call InterActable 
        //checks if InterActable enters Actor zone
        //Checks InterActalbe tag
        public void OnTriggerEnter2D(Collider2D co)
        {
            IInteractable interactable = co.GetComponent<IInteractable>();
            interactable.PlayerEnter();

        }
        private void OnTriggerStay2D(Collider2D co)
        {
            IInteractable interactable = co.GetComponent<IInteractable>();
            if (Input.GetKeyDown(interact))
            {
                interactable.Interact();
            }

        }
        public void OnTriggerExit2D(Collider2D co)
        {
            IInteractable interactable = co.GetComponent<IInteractable>();
            interactable.PlayerExit();
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    public class InteractionChecker : MonoBehaviour
    {
        public KeyCode interact = KeyCode.F; //change if needed


        //Code uses Tag call InterActable 
        //checks if InterActable enters Actor zone
        //Checks InterActalbe tag
        public void OnTriggerEnter2D(Collider2D co)
        {
            print("enter called");
            IInteractable interactable = co.GetComponent<IInteractable>();
            interactable.PlayerEnter();
        }
        private void OnTriggerStay2D(Collider2D co)
        {
            print("player here");
            IInteractable interactable = co.GetComponent<IInteractable>();
            if (Input.GetKey(interact))
            {
                print("f pressed");
                interactable.Interact();
            }

        }
        public void OnTriggerExit2D(Collider2D co)
        {
            print("exit called");
            IInteractable interactable = co.GetComponent<IInteractable>();
            interactable.PlayerExit();
        }
        
    }
}

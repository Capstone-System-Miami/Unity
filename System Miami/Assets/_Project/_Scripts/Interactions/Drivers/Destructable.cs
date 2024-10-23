//// Author: Layla Hoey
//using UnityEngine;
//using UnityEngine.Tilemaps;

//namespace SystemMiami
//{
//    // This is a driver script, meant to test InteractionChecker.
//    // It's also an example of how to use/implement interfaces.
//    // It is not necessarily practical.

//    // You can right click IInteractable
//    // and select "Peek Definition" to see the whole interface

//    public class Destructable : InteractionTrigger
//    {
//        public override void PlayerEnter()
//        {
//            base.PlayerEnter();
//            _tilemap.color = _interactableColor;
//        }

//        public override void Interact()
//        {
//            base.Interact();
//        }

//        public override void PlayerExit()
//        {
//            _tilemap.color = _nonInteractableColor;
//            base.PlayerExit();
//        }
//    }
//}

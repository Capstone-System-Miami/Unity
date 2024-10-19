// Author: Layla Hoey
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SystemMiami
{
    // This is a driver script, meant to test InteractionChecker.
    // It's also an example of how to use/implement interfaces.
    // It is not necessarily practical.

    // You can right click IInteractable
    // and select "Peek Definition" to see the whole interface

    public class Destructable : MonoBehaviour, IInteractable
    {
        [SerializeField] private Color _interactableColor;

        private Tilemap _tilemap;
        private Color _nonInteractableColor;

        private void Start()
        {
            _tilemap = GetComponent<Tilemap>();

            // SetWith the non interactable color to whatever
            // the tilemap color was set to when we pressed play.
            _nonInteractableColor = _tilemap.color;
        }


        // Because we've declared that this script is using IInteractable,
        // We have to implement (define) what we "promised" we would
        // in the interface definition.
        #region IInteractable Implementation

        // When this is called, change the color to the one
        // defined in the inspector
        public void PlayerEnter()
        {
            _tilemap.color = _interactableColor;
        }

        // When this is called, destroy the game object
        public void Interact()
        {
            Destroy(gameObject);
        }

        // When this is called, change the color to the one
        // defined in Start()
        public void PlayerExit()
        {
            _tilemap.color = _nonInteractableColor;
        }
        #endregion
    }
}

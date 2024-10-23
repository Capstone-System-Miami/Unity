// Author: Layla Hoey, Andrew
using Unity.VisualScripting;
using UnityEngine;

namespace SystemMiami
{
    public class InteractionChecker : MonoBehaviour
    {
        [SerializeField] private bool _debugMessages;

        [SerializeField] private KeyCode _interactKey;

        private IInteractable _storedInteraction;

        private void Update()
        {
            if (Input.GetKeyDown(_interactKey))
            {
                _storedInteraction?.Interact();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_debugMessages)
                { print("enter called"); }

            // InParent will search for it in the collision first,
            // then loop up through parents until it finds one
            IInteractable collidedInteraction = collision.GetComponentInParent<IInteractable>();
            
            if (collidedInteraction == null)
            {
                if (_debugMessages)
                    { print($"Entering { collision.name }, but it is not an interactable object"); }

                return;
            }

            // If there is already a stored interaction, boot/override it.
            _storedInteraction?.PlayerExit();
            _storedInteraction = collidedInteraction;
            _storedInteraction.PlayerEnter();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (_debugMessages) { print("exit called"); }

            // InParent will search for it in the collision first,
            // then loop up through parents until it finds one
            IInteractable collidedInteraction = collision.GetComponentInParent<IInteractable>();

            if (collidedInteraction == null)
            {
                if (_debugMessages)
                { print($"Leaving {collision.name}, but it is not an interactable object"); }

                return;
            }
            
            // Let the stored interaction know we're leaving
            _storedInteraction?.PlayerExit();

            // Stop storing it
            _storedInteraction = null;
        }
        
    }
}

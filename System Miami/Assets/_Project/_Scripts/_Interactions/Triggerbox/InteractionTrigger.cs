using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace SystemMiami
{
    [RequireComponent(typeof(Collider2D))]
    public class InteractionTrigger : MonoBehaviour, IInteractable
    {        
        [SerializeField] private UnityEvent OnEnter;
        [SerializeField] public UnityEvent OnInteract;
        [SerializeField] private UnityEvent OnExit;

        [SerializeField] private string _promptAction;

        // Because we've declared that this script is using IInteractable,
        // We have to implement (define) what we "promised" we would
        // in the interface definition.
        #region IInteractable Implementation

        // When this is called, change the color to the one
        // defined in the inspector
        public virtual void PlayerEnter()
        {
            OnEnter.Invoke();
        }

        // When this is called, destroy the game object
        public virtual void Interact()
        {
            OnInteract.Invoke();
        }

        // When this is called, change the color to the one
        // defined in Start()
        public virtual void PlayerExit()
        {
            OnExit.Invoke();
        }

        public virtual string GetActionPrompt()
        {
            return _promptAction;
        }
        #endregion
    }
}


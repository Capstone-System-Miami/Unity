
// - Modified to allow NPC interactions - Johnny Sosa

using UnityEngine;
using UnityEngine.Events;

namespace SystemMiami
{
    [RequireComponent(typeof(Collider2D))]
    public class InteractionTrigger : MonoBehaviour, IInteractable
    {
        [SerializeField] public bool IsInteractionEnabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }

        [SerializeField] private UnityEvent OnEnter;
        [SerializeField] public UnityEvent OnInteract;
        [SerializeField] private UnityEvent OnExit;
        [SerializeField] private string _promptAction;

        private bool isInteractionEnabled = true;

        private void OnDisable()
        {
            PlayerExit();
        }

        public virtual void PlayerEnter()
        {
            if (!IsInteractionEnabled) { return; }
            OnEnter?.Invoke();
        }

        public virtual void Interact()
        {
            if (!IsInteractionEnabled) { return; }
            OnInteract?.Invoke();
        }

        public virtual void PlayerExit()
        {
            OnExit?.Invoke();
        }

        public virtual string GetActionPrompt()
        {
            return _promptAction;
        }
    }
}

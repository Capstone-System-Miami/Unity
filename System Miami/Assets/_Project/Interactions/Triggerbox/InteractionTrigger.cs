
// - Modified to allow NPC interactions - Johnny Sosa

using UnityEngine;
using UnityEngine.Events;

namespace SystemMiami
{
    [RequireComponent(typeof(Collider2D))]
    public class InteractionTrigger : MonoBehaviour, IInteractable
    {
        [SerializeField] private UnityEvent OnEnter;
        [SerializeField] public UnityEvent OnInteract;
        [SerializeField] private UnityEvent OnExit;
        [SerializeField] private string _promptAction;

        private NPC npc; // Reference to NPC script

        private void Start()
        {
            npc = GetComponent<NPC>(); // Check if NPC script is attached
        }

        public virtual void PlayerEnter()
        {
            OnEnter?.Invoke();
        }

        public virtual void Interact()
        {
            OnInteract?.Invoke();

            if (npc != null)
            {
                npc.StartDialogue(); // Trigger NPC dialogue
            }
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

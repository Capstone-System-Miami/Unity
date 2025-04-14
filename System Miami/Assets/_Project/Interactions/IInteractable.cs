using UnityEngine;

namespace SystemMiami
{
    public interface IInteractable
    {
        bool IsInteractionEnabled { get; }
        void Interact();
        void PlayerEnter();
        void PlayerExit();
        string GetActionPrompt();
    }
}

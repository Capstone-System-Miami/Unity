using UnityEngine;

namespace SystemMiami
{
    public interface IInteractable
    {
        void Interact();
        void PlayerEnter();
        void PlayerExit();
    }
}

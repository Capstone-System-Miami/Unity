using UnityEngine;
using SystemMiami.Management;

// Layla
namespace SystemMiami.ui
{
    public class GoToCredits : MonoBehaviour
    {
        public void Go()
        {
            GAME.MGR?.GoToCredits();
        }
    }
}

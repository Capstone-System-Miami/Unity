using UnityEngine;
using SystemMiami.Management;

// Layla
namespace SystemMiami.ui
{
    public class GoToMain : MonoBehaviour
    {
        public void Go()
        {
            GAME.MGR?.GoToMain();
        }
    }
}

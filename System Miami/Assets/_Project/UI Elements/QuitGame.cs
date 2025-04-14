using SystemMiami.Management;
using UnityEngine;

// Layla
namespace SystemMiami.ui
{
    public class QuitGame : MonoBehaviour
    {
        public void Go()
        {
            GAME.MGR.Quit();
        }
    }
}

// Layla
using SystemMiami.Management;
using UnityEngine;

namespace SystemMiami
{
    public class PauseGame : MonoBehaviour
    {
        public void Go()
        {
            GAME.MGR.PauseGame();
        }
    }
}

using UnityEngine;
using SystemMiami.Management;

// Layla
namespace SystemMiami.ui
{
    public class GoToSettings : MonoBehaviour
    {
        public void Go()
        {
            GAME.MGR?.GoToSettings();
        }
    }
}

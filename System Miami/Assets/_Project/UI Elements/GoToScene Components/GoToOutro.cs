using UnityEngine;
using SystemMiami.Management;

// Layla
namespace SystemMiami.ui
{
    public class GoToOutro : MonoBehaviour
    {
        public void Go()
        {
            GAME.MGR?.GoToOutro();
        }
    }
}

using UnityEngine;
using SystemMiami.Management;

// Layla
namespace SystemMiami.ui
{
    public class GoToIntro : MonoBehaviour
    {
        public void Go()
        {
            GAME.MGR?.GoToIntro();
        }
    }
}

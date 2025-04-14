
using UnityEngine;
using SystemMiami.Management;

// Layla
namespace SystemMiami.ui
{
    public class GoToCharacterSelect : MonoBehaviour
    {
        public void Go()
        {
            GAME.MGR.GoToCharacterSelect();
        }
    }
}

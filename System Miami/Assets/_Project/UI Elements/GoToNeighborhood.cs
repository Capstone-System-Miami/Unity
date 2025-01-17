using UnityEngine;
using SystemMiami.Management;

namespace SystemMiami.ui
{
    public class GoToNeighborhood : MonoBehaviour
    {
        public void Go()
        {
            GAME.MGR.GoToNeighborhood();
        }
    }
}
